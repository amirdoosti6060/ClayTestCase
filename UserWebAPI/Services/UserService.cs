using Microsoft.EntityFrameworkCore;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;

namespace UserWebAPI.Services
{
    public class UserService: IUserService
    {
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(UserDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<GeneralResponse> Get(long id)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync<User>();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {id} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetAll()
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.Users.ToListAsync();

            if ((response.Data as List<User>)!.Count == 0)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = "No user found!";
            }

            return response;
        }

        public async Task<GeneralResponse> Add(AddUserRequest addUserRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            User user = new User
            {
                Email = addUserRequest.Email,
                Password = addUserRequest.Password,
                FullName = addUserRequest.FullName,
                Role = addUserRequest.Role
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            response.Data = user.Id;

            return response;
        }

        public async Task<GeneralResponse> Update(long id, UpdateUserRequest updateUserRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var foundUser = await _dbContext.Users
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (foundUser == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {id} not found!";
            }
            else
            { 
                foundUser.Email = updateUserRequest.Email;
                foundUser.Password = updateUserRequest.Password;
                foundUser.FullName = updateUserRequest.FullName;
                foundUser.Role = updateUserRequest.Role;

                _dbContext.Users.Update(foundUser);
                await _dbContext.SaveChangesAsync();
                response.Data = id;
            }

            return response;
        }

        public async Task<GeneralResponse> Delete(long id)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var user = await _dbContext.Users
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {id} not found!";
            }
            else
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                response.Data = id;
            }

            return response;
        }
    }
}
