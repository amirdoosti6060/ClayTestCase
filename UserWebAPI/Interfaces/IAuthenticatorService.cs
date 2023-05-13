using UserWebAPI.Models;

namespace UserWebAPI.Interfaces
{
    public interface IAuthenticatorService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
    }
}
