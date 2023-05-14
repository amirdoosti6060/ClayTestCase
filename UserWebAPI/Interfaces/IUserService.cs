using UserWebAPI.Models;

namespace UserWebAPI.Interfaces
{
    public interface IUserService
    {
        Task<GeneralResponse> Get(long id);
        Task<GeneralResponse> GetAll();
        Task<GeneralResponse> Add(User user);
        Task<GeneralResponse> Update(long id, User user);
        Task<GeneralResponse> Delete(long id);
    }
}
