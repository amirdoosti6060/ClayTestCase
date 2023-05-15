using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DoorWebAPI.Services
{
    public class PermissionService: IPermissionService
    {
        private readonly DoorDbContext _dbContext;

        public PermissionService(DoorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GeneralResponse> Get(long id)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions.Where(e => e.Id == id)
                .FirstOrDefaultAsync<Permission>();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Permission {id} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> Get(long doorId, string role)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions
                .Where(e => e.DoorId == doorId && e.Role == role)
                .FirstOrDefaultAsync<Permission>();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Permission ({doorId},{role}) not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAll()
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions.ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = "No permission found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAllByDoor(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions.Where(e => e.DoorId == doorId)
                .ToListAsync<Permission>();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"No permission exists for door ID={doorId} !";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAllByRole(string role)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions
                .Where(e => e.Role == role)
                .ToListAsync<Permission>();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"No permission exists for role={role} !";
            }

            return response;
        }

        public async Task<GeneralResponse> Add(AddPermissionRequest addPermRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            Permission perm = new Permission
            {
                DoorId = addPermRequest.DoorId,
                Role = addPermRequest.Role
            };

            var permission = await _dbContext.Permissions
                .Where(e => e.DoorId == perm.DoorId && e.Role == perm.Role)
                .FirstOrDefaultAsync();
            if (permission != null)
            {
                response.ErrorCode = StatusCodes.Status400BadRequest;
                response.ErrorMessage = $"Permission ({perm.DoorId},{perm.Role}) already exist!";
            }
            else
            {
                _dbContext.Permissions.Add(perm);
                if (await _dbContext.SaveChangesAsync() <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to add permission!";
                }
                else
                    response.Data = perm.Id;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long permId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var permission = await _dbContext.Permissions
                .Where(e => e.Id == permId)
                .FirstOrDefaultAsync();
            if (permission == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Permission {permId} not found!";
            }
            else
            {
                _dbContext.Permissions.Remove(permission);
                var nupdate = await _dbContext.SaveChangesAsync();
                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to delete permission {permId}";
                }
                else
                    response.Data = permId;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long doorId, string role)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var permission = await _dbContext.Permissions
                .Where(e => e.DoorId == doorId && e.Role == role)
                .FirstOrDefaultAsync();

            if (permission == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Permission ({doorId},{role}) not found!";
            }
            else
            {
                _dbContext.Permissions.Remove(permission);
                var nupdate = await _dbContext.SaveChangesAsync();
                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to delete permission ({doorId},{role})";
                }
                else
                    response.Data = permission.Id;
            }

            return response;
        }
    }
}
