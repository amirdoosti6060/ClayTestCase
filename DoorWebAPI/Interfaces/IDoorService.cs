using DoorWebAPI.Models;

namespace DoorWebAPI.Interfaces
{
    public interface IDoorService
    {
        Task<GeneralResponse> Get(long doorId);
        Task<GeneralResponse> GetAll();
        Task<GeneralResponse> Add(AddUpdateDoorRequest appUpdateDoorRequest);
        Task<GeneralResponse> Update(long doorId, AddUpdateDoorRequest appUpdateDoorRequest);
        Task<GeneralResponse> Delete(long doorId);
        Task<GeneralResponse> Unlock(UserInfo userInfo, long doorId);
    }
}
