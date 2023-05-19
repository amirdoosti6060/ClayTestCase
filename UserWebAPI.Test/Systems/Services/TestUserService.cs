using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserWebAPI.Models;
using UserWebAPI.Services;
using UserWebAPI.Test.Systems.MockData;
using Xunit.Abstractions;

namespace UserWebAPI.Test.Systems.Services
{
    public class TestUserService : IDisposable
    {
        private readonly UserDbContext _dbContext;
        private readonly UserService _userService;

        public TestUserService()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new UserDbContext(options);
            _dbContext.Database.EnsureCreated();

            var moqLoggerUserService = new Mock<ILogger<UserService>>();
            _userService = new UserService(_dbContext, moqLoggerUserService.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_Success(long userId)
        {
            //Arrange
            await _dbContext.Users.AddRangeAsync(UserMockData.GetUsers());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userService.Get(userId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_NotFound(long userId)
        {
            //Arrange
            await _dbContext.Users.AddRangeAsync(UserMockData.GetUsers());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userService.Get(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
            Assert.NotNull(resp.ErrorMessage);
        }

        [Fact]
        public async Task GetAll_Success_MatchCountOfUsers()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
            Assert.NotNull(resp.Data);
            Assert.Equal(users.Count, (resp.Data as List<User>)!.Count);
        }

        [Fact]
        public async Task GetAll_NotFound()
        {
            //Arrange

            //Act
            var resp = await _userService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            //Arrange
            User user = UserMockData.GetUsers()[0];
            AddUserRequest addUserRequest = new AddUserRequest
            {
                Email = user.Email,
                FullName = user.FullName,
                Password = user.Password,
                Role = user.Role
            };

            //Act
            var resp = await _userService.Add(addUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Fact]
        public async Task Update_Success()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            User user = users[0];
            UpdateUserRequest updateUserRequest = new UpdateUserRequest
            {
                Email = user.Email,
                FullName = user.FullName + "Change",
                Password = user.Password,
                Role = user.Role
            };

            //Act
            var resp = await _userService.Update(user.Id, updateUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task Update_NotFound(long userId)
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            User user = users[0];
            UpdateUserRequest updateUserRequest = new UpdateUserRequest
            {
                Email = user.Email,
                FullName = user.FullName + "Change",
                Password = user.Password,
                Role = user.Role
            };

            //Act
            var resp = await _userService.Update(userId, updateUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }

        [Fact]
        public async Task Delete_Success()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userService.Delete(users[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task Delete_NotFound(long userId)
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userService.Delete(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }
    }
}
