using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQServiceLib;
using System.Net;
using System.Text;

namespace DoorWebAPI.Services
{
    public class DoorService: IDoorService
    {
        private readonly DoorDbContext _dbContext;
        private readonly IBus _bus;
        private readonly ILogger<DoorService> _logger;
        private readonly LockHandlerSettings _lockHandlerSettings;

        public DoorService(DoorDbContext dbContext, IBus bus, 
            ILogger<DoorService> logger,
            IOptions<LockHandlerSettings> lockHandlerSettings)
        {
            _dbContext = dbContext;
            _bus = bus;
            _logger = logger;
            _lockHandlerSettings = lockHandlerSettings.Value;
        }

        public async Task<GeneralResponse> Get(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Doors
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();

            if (response.Data == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Door {doorId} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAll()
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Doors.ToListAsync();

            if ((response.Data as List<Door>)!.Count == 0)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = "No door found!";
            }

            return response;
        }

        public async Task<GeneralResponse> Add(AddUpdateDoorRequest appUpdateDoorRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            Door door = new Door
            {
                Name = appUpdateDoorRequest.Name,
                HardwareId = appUpdateDoorRequest.HardwareId,
                ModifiedAt = DateTime.Now
            };

            _dbContext.Doors.Add(door);
            await _dbContext.SaveChangesAsync();
            response.Data = door.Id;

            return response;
        }

        public async Task<GeneralResponse> Update(long doorId, AddUpdateDoorRequest appUpdateDoorRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            var foundDoor = await _dbContext.Doors
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();
            if (foundDoor == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Door {doorId} not found!";
            }
            else
            {
                foundDoor.Name = appUpdateDoorRequest.Name;
                foundDoor.HardwareId = appUpdateDoorRequest.HardwareId;
                foundDoor.ModifiedAt = DateTime.Now;

                _dbContext.Doors.Update(foundDoor);
                await _dbContext.SaveChangesAsync();
                response.Data = doorId;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            var door = await _dbContext.Doors
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();
            if (door == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Door {doorId} not found!";
            }
            else
            {
                _dbContext.Doors.Remove(door);
                await _dbContext.SaveChangesAsync();
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
            Door? door = await _dbContext.Doors
                .Where(e => e.Id == doorId)
                .FirstOrDefaultAsync();

            return door;
        }

        private async Task<HttpStatusCode> Unlock(string hardwareId)
        {
            var status = HttpStatusCode.BadRequest;

            try
            {
                var client = new HttpClient();
                var url = _lockHandlerSettings.Url.Replace("{hardwareid}", hardwareId);
                var unlockResp = await client.GetAsync(url);

                status = unlockResp.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in opening door");
            }

            return status;
        }

        private void ToGeneralResponse(
            HttpStatusCode status, 
            string email, string role, long doorid,
            ref GeneralResponse resp)
        {
            switch (status)
            {
                case HttpStatusCode.Unauthorized:
                    resp.Code = StatusCodes.Status401Unauthorized;
                    resp.Message = $"User {email} with role {role} is not authorized to unlock door {doorid}";
                    resp.Data = "unauthorized";
                    break;

                case HttpStatusCode.OK:
                    resp.Code = StatusCodes.Status200OK;
                    resp.Message = $"Door {doorid} Unlocked!";
                    resp.Data = "ok";
                    break;

                case HttpStatusCode.RequestTimeout:
                    resp.Code = StatusCodes.Status408RequestTimeout;
                    resp.Message = $"Door {doorid} didn't respond!";
                    resp.Data = "timeout";
                    break;

                case HttpStatusCode.BadRequest:
                    resp.Code = StatusCodes.Status400BadRequest;
                    resp.Message = $"Something is wrong with door {doorid}!";
                    resp.Data = "badrequest";
                    break;

                case HttpStatusCode.NotFound:
                    resp.Code = StatusCodes.Status404NotFound;
                    resp.Message = $"Door {doorid} not found!";
                    resp.Data = "notfound";
                    break;

                default:
                    resp.Code = StatusCodes.Status404NotFound;
                    resp.Message = $"Unknown error happend!";
                    resp.Code = "unknown";
                    break;
            }

        }

        public async Task<GeneralResponse> Unlock(UserInfo? userInfo, long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };
            Door? door = await GetDoorInfo(doorId);

            if (!await UserAuthorized(userInfo!, doorId))
            {
                ToGeneralResponse(HttpStatusCode.Unauthorized, userInfo!.Email, 
                    userInfo.Role, doorId, ref response);
            }
            else
            {
                try
                {
                    var status = await Unlock(door!.HardwareId);
                    ToGeneralResponse(status, userInfo!.Email,
                        userInfo.Role, doorId, ref response);

                }
                catch
                {
                    ToGeneralResponse(HttpStatusCode.RequestTimeout, userInfo!.Email,
                        userInfo.Role, doorId, ref response);
                }
            }

            // Send message to history
            DoorUnlockInfo doorUnlockInfo = new DoorUnlockInfo
            {
                DoorId = doorId,
                DoorName = door != null? door.Name: "",
                HardwareId = door != null ? door.HardwareId: "",
                UserId = userInfo!.Id,
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                Role = userInfo.Role,
                ActionStatus = (response.Data as string)!,
                TimeStamp = DateTime.Now
            };

            await _bus.SendAsync<DoorUnlockInfo>("historyQueue", doorUnlockInfo);

            return response;
        }
    }
}
