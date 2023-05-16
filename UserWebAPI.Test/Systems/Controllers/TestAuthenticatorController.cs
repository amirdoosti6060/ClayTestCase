using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    public class TestAuthenticatorController: IDisposable
    {
        private readonly UserDbContext _dbContext;
        private readonly AuthenticatorController _authenticatorController;
        private readonly JwtSettings _jwtSettings;

        public TestAuthenticatorController()
        {
            _jwtSettings = new JwtSettings
            {
                JwtSettings_Key = "TestKeyForJwtTestKeyForJwtTestKeyForJwtTestKeyForJwtTestKeyForJwt",
                JwtSettings_AccessTokenValidityInMinute = "120",
                JwtSettings_Audience = "Audience",
                JwtSettings_Issuer = "Issuer"
            };

            var options = new DbContextOptionsBuilder<UserDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new UserDbContext(options);
            _dbContext.Database.EnsureCreated();

            AuthenticatorService authenticatorService = 
                new AuthenticatorService(_dbContext, Options.Create(_jwtSettings));

            _authenticatorController = new AuthenticatorController(authenticatorService);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


        [Fact]
        public async Task Login_Ok()
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = users[0].Email,
                Password = users[0].Password
            };

            //Act
            var resp = await _authenticatorController.Login(loginRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Theory]
        [InlineData("Email_NotFound", "Password_NotFound")]
        public async Task Login_NotFound(string email, string password)
        {
            //Arrange
            var users = UserMockData.GetUsers();
            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            //Act
            var resp = await _authenticatorController.Login(loginRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public void Get_Ok()
        {
            //Arrange

            //Act
            var resp = _authenticatorController.Get();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp!.ErrorCode);
        }
    }
}
