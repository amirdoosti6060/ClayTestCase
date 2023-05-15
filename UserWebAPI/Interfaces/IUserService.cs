using UserWebAPI.Models;

namespace UserWebAPI.Interfaces
{
    public interface IUserService
    {
        Task<GeneralResponse> Get(long id);
        Task<GeneralResponse> GetAll();
        Task<GeneralResponse> Add(AddUserRequest addUserRequest);
        Task<GeneralResponse> Update(long id, UpdateUserRequest updateUserRequest);
        Task<GeneralResponse> Delete(long id);
    }
}
