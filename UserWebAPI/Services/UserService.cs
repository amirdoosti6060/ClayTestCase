using Microsoft.EntityFrameworkCore;
using UserWebAPI.Interfaces;
using UserWebAPI.Models;

namespace UserWebAPI.Services
{
    public class UserService: IUserService
    {
        private readonly UserDbContext _dbContext;

        public UserService(UserDbContext dbContext)
        {
            _dbContext = dbContext;
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

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = "No user found!";
            }

            return response;
        }

        public async Task<GeneralResponse> Add(User user)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            _dbContext.Users.Add(user);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                response.ErrorCode = StatusCodes.Status400BadRequest;
                response.ErrorMessage = $"Unable to add user!";
            }
            else
                response.Data = user.Id;

            return response;
        }

        public async Task<GeneralResponse> Update(long id, User user)
        {
            int nupdate = 0;
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            var foundUser = await _dbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (foundUser == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {id} not found!";
            }
            else
            { 
                foundUser.Email = user.Email;
                foundUser.Password = user.Password;
                foundUser.FullName = user.FullName;
                foundUser.Role = user.Role;

                _dbContext.Users.Update(foundUser);
                nupdate = await _dbContext.SaveChangesAsync();

                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to update user {id} !";
                }
                else
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

            var user = await _dbContext.Users.Where(e => e.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"User {id} not found!";
            }
            else
            {
                _dbContext.Users.Remove(user);
                var nupdate = await _dbContext.SaveChangesAsync();
                if (nupdate <= 0)
                {
                    response.ErrorCode = StatusCodes.Status400BadRequest;
                    response.ErrorMessage = $"Unable to delete user {id}";
                }
                else
                    response.Data = id;
            }

            return response;
        }
    }
}
