using DoorWebAPI.Models;

namespace DoorWebAPI.Interfaces
{
    public interface IPermissionService
    {
        Task<GeneralResponse> Get(long permId);
        Task<GeneralResponse> Get(long doorId, string role);
        Task<GeneralResponse> GetAll();
        Task<GeneralResponse> GetAllByDoor(long doorId);
        Task<GeneralResponse> GetAllByRole(string role);
        Task<GeneralResponse> Add(Permission perm);
        Task<GeneralResponse> Delete(long permId);
        Task<GeneralResponse> Delete(long doorId, string role);
    }
}
