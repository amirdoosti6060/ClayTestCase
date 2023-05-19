using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserWebAPI.Controllers;
using UserWebAPI.Models;
using UserWebAPI.Services;
using UserWebAPI.Test.Helper;
using UserWebAPI.Test.Systems.MockData;

namespace UserWebAPI.Test.Systems.Controllers
{
    public class TestUserController: IDisposable
    {
        private readonly UserDbContext _dbContext;
        private readonly UserController _userController;

        public TestUserController()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new UserDbContext(options);
            _dbContext.Database.EnsureCreated();

            var moqLoggerUserService = new Mock<ILogger<UserService>>();
            var moqLoggerUserController = new Mock<ILogger<UserController>>();

            UserService userService = new UserService(_dbContext, moqLoggerUserService.Object);
            _userController = new UserController(userService, moqLoggerUserController.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_Ok(long userId)
        {
            //Arrange
            await _dbContext.Users.AddRangeAsync(UserMockData.GetUsers());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userController.Get(userId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_NotFound(long userId)
        {
            //Arrange
            await _dbContext.Users.AddRangeAsync(UserMockData.GetUsers());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userController.Get(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }
        
        [Fact]
        public async Task Get_All_Ok()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userController.Get();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }
        
        [Fact]
        public async Task GetAll_NotFound()
        {
            //Arrange

            //Act
            var resp = await _userController.Get();

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }
        
        [Fact]
        public async Task Add_Ok()
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
            var resp = await _userController.Post(addUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }
        
        [Fact]
        public async Task Update_Ok()
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
            var resp = await _userController.Put(user.Id, updateUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
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
            var resp = await _userController.Put(userId, updateUserRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Delete_Ok()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _userController.Delete(users[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
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
            var resp = await _userController.Delete(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }
    }
}
