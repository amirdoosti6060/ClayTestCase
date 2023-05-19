using DoorWebAPI.Models;

namespace DoorWebAPI.Interfaces
{
    public interface IPermissionService
    {
        Task<GeneralResponse> Get(long permId);
        Task<GeneralResponse> Get(GetPermissionRequest getPermissionRequest);
        Task<GeneralResponse> Add(AddPermissionRequest addPermRequest);
        Task<GeneralResponse> Delete(long permId);
        Task<GeneralResponse> Delete(long doorId, string role);
    }
}
