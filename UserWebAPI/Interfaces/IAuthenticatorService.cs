using UserWebAPI.Models;

namespace UserWebAPI.Interfaces
{
    public interface IAuthenticatorService
    {
        Task<GeneralResponse> Login(LoginRequest loginRequest);
    }
}
