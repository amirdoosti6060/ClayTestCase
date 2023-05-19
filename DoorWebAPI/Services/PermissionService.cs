using DoorWebAPI.Interfaces;
using DoorWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DoorWebAPI.Services
{
    public class PermissionService: IPermissionService
    {
        private readonly DoorDbContext _dbContext;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(DoorDbContext dbContext, ILogger<PermissionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<GeneralResponse> Get(long permId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Permissions.Where(e => e.Id == permId)
                .FirstOrDefaultAsync<Permission>();

            if (response.Data == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Permission {permId} not found!";
            }

            return response;
        }

        private string GetOperator(string? str)
        {
            return string.IsNullOrEmpty(str) ? "" : " AND ";
        }

        private string BuildQuery(GetPermissionRequest getPermissionRequest)
        {
            string qry = "";
            if (getPermissionRequest.doorId != null)
                qry += $"doorId = {getPermissionRequest.doorId}";
            if (!string.IsNullOrEmpty(getPermissionRequest.role))
                qry += GetOperator(qry) + $"role = \"{getPermissionRequest.role}\"";

            return qry;
        }

        public async Task<GeneralResponse> Get(GetPermissionRequest getPermissionRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            string qry = BuildQuery(getPermissionRequest);

            if (string.IsNullOrEmpty(qry))
                response.Data = await _dbContext.Permissions.ToListAsync<Permission>();
            else
                response.Data = await _dbContext.Permissions.Where(qry).ToListAsync();

            if ((response.Data as List<Permission>)!.Count == 0)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = "Requested permission not found!";
            }

            _logger.LogDebug($"Generated query: {qry}!");

            return response;
        }

        public async Task<GeneralResponse> Add(AddPermissionRequest addPermRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
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
                response.Code = StatusCodes.Status400BadRequest;
                response.Message = $"Permission ({perm.DoorId},{perm.Role}) already exist!";
            }
            else
            {
                _dbContext.Permissions.Add(perm);
                await _dbContext.SaveChangesAsync();
                response.Data = perm.Id;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long permId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            var permission = await _dbContext.Permissions
                .Where(e => e.Id == permId)
                .FirstOrDefaultAsync();
            if (permission == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Permission {permId} not found!";
            }
            else
            {
                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
                response.Data = permId;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long doorId, string role)
        {
            GeneralResponse response = new GeneralResponse()
            {
                Code = StatusCodes.Status200OK
            };

            var permission = await _dbContext.Permissions
                .Where(e => e.DoorId == doorId && e.Role == role)
                .FirstOrDefaultAsync();

            if (permission == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Message = $"Permission ({doorId},{role}) not found!";
            }
            else
            {
                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
                response.Data = permission.Id;
            }

            return response;
        }
    }
}
