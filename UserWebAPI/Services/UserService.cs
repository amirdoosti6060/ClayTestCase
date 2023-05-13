namespace UserWebAPI.Services
{
    public interface IUserService
    {
        /*
        public Task<User?> FindUser(UserIdentity userIdentity);
        public Task<ResponseLogin> Login(UserLogin userLogin);
        public Task<ResponseGeneral> Logout(UserIdentity userIdentifier);
        public Task<ResponseLogin> RefreshAccessToken(UserRefresh userRefresh);
        public Task<ResponseGetAllUsers> GetAllUsers();
        public ValueTask<ResponseAddUser> AddUser(User user);
        public ValueTask<ResponseGeneral> ActivateUser(UserIdentity userIdentity);
        public ValueTask<ResponseGeneral> ActivateUser(string activationString);
        public ValueTask<ResponseGeneral> DisableUser(UserIdentity userIdentity);
        public Task<ResponseGeneral> UpdateUser(User user);
        public Task<ResponseGeneral> DeleteUser(UserIdentity userIdentity);
        public Task<ResponseGeneral> SendActivationEmail(string host, UserIdentity userIdentity);
        public Task<ResponseGeneral> ChangePassword(UserChangePassword userChangePassword);
        public Task<ResponseGeneral> SendForgetPasswordEmail(string passwordPageUrl, UserIdentity userIdentity);
        public Task<ResponseGeneral> ChangeForgottenPassword(UserChangeForgottenPassword userChangeForgotternPassword);
        public Task<ResponseUserInfo> GetUserInfo(UserIdentity userIdentity);
        public Task<ResponseGeneral> UpdateUserRole(UserChangeRole userChangeRole);
        */
    }

    public class UserService: IUserService
    {
        public UserService()
        {

        }


    }
}
