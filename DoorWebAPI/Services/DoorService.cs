using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using HistoryWebAPI.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQServiceLib;

namespace DoorWebAPI.Services
{
    public class DoorService: IDoorService
    {
        private readonly DoorDbContext _dbContext;
        private readonly IBus _bus;

        public DoorService(DoorDbContext dbContext, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
        }

        public async Task<GeneralResponse> Get(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Door
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Door {doorId} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAll()
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Door.ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = "No door found!";
            }

            return response;
        }

        public async Task<GeneralResponse> Add(AddUpdateDoorRequest appUpdateDoorRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            Door door = new Door
            {
                Name = appUpdateDoorRequest.Name,
                HardwareId = appUpdateDoorRequest.HardwareId,
                ModifiedAt = DateTime.Now
            };

            _dbContext.Door.Add(door);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                response.ErrorCode = StatusCodes.Status400BadRequest;
                response.ErrorMessage = $"Unable to add door!";
            }
            else
                response.Data = door.Id;

            return response;
        }

        public async Task<GeneralResponse> Update(long doorId, AddUpdateDoorRequest appUpdateDoorRequest)
        {
            int nupdate = 0;
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var foundDoor = await _dbContext.Door
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();
            if (foundDoor == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Door {doorId} not found!";
            }
            else
            {
                foundDoor.Name = appUpdateDoorRequest.Name;
                foundDoor.HardwareId = appUpdateDoorRequest.HardwareId;
                foundDoor.ModifiedAt = DateTime.Now;

                _dbContext.Door.Update(foundDoor);
                nupdate = await _dbContext.SaveChangesAsync();

                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to update door {doorId} !";
                }
                else
                    response.Data = doorId;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var door = await _dbContext.Door
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();
            if (door == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Door {doorId} not found!";
            }
            else
            {
                _dbContext.Door.Remove(door);
                var nupdate = await _dbContext.SaveChangesAsync();
                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to delete permission {doorId}";
                }
                else
                    response.Data = doorId;
            }

            return response;
        }

        private async ValueTask<bool> UserAuthorized(UserInfo userInfo, long doorId)
        {
            Permission? perm = await _dbContext.Permissions
                .Where(e => e.DoorId == doorId && e.Role == userInfo.Role)
                .FirstOrDefaultAsync<Permission>();

            return (perm != null);
        }

        private async Task<Door?> GetDoorInfo(long doorId)
        {
            Door? door = await _dbContext.Door
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();

            return door;
        }

        public async Task<GeneralResponse> Unlock(UserInfo? userInfo, long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };
            string status = "";

            Door? door = await GetDoorInfo(doorId);

            if (!await UserAuthorized(userInfo!, doorId))
            {
                response.ErrorCode = StatusCodes.Status401Unauthorized;
                response.ErrorMessage = $"User {userInfo.Email} with role {userInfo.Role} is not authorized to unlock door {doorId}";
                status = "Unauthorized";
            }
            else
            {
                LockHardware lockHw = new LockHardware();
                CancellationTokenSource tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(1500));
                CancellationToken token = tokenSource.Token;
                try
                {
                    status = await lockHw.UnLock(door!.HardwareId, token);
                    switch (status)
                    {
                        case "Timeout":
                            response.ErrorCode = StatusCodes.Status408RequestTimeout;
                            response.ErrorMessage = $"Door {doorId} didn't respond!";
                            break;
                        case "Fail":
                            response.ErrorCode = StatusCodes.Status406NotAcceptable;
                            response.ErrorMessage = $"Door {doorId} didn't respond!";
                            break;
                    }
                } 
                catch
                {
                    status = "Timeout";
                    response.ErrorCode = StatusCodes.Status408RequestTimeout;
                    response.ErrorMessage = $"Door {doorId} didn't respond!";
                }
            }

            DoorUnlockInfo doorUnlockInfo = new DoorUnlockInfo
            {
                DoorId = doorId,
                DoorName = door != null? door.Name: "",
                HardwareId = door != null ? door.HardwareId: "",
                UserId = userInfo.Id,
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                Role = userInfo.Role,
                ActionStatus = status,
                TimeStamp = DateTime.Now
            };

            await _bus.SendAsync<DoorUnlockInfo>("historyQueue", doorUnlockInfo);

            return response;
        }
    }
}
