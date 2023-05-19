using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserWebAPI.Models;
using UserWebAPI.Services;
using UserWebAPI.Test.Systems.MockData;

namespace UserWebAPI.Test.Systems.Services
{
    public class TestAuthenticatorService: IDisposable
    {
        private readonly UserDbContext _dbContext;
        private readonly AuthenticatorService _authenticatorService;
        private readonly JwtSettings _jwtSettings;

        public TestAuthenticatorService()
        {
            _jwtSettings = new JwtSettings
            {
                Key = "TestKeyForJwtTestKeyForJwtTestKeyForJwtTestKeyForJwtTestKeyForJwt",
                AccessTokenValidityInMinute = "120",
                Audience = "Audience",
                Issuer = "Issuer"
            };

            var options = new DbContextOptionsBuilder<UserDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new UserDbContext(options);
            _dbContext.Database.EnsureCreated();

            var moqLoggerService = new Mock<ILogger<AuthenticatorService>>();

            _authenticatorService = new AuthenticatorService(
                _dbContext, 
                Options.Create(_jwtSettings),
                moqLoggerService.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Login_Success()
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
            var resp = await _authenticatorService.Login(loginRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
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
            var resp = await _authenticatorService.Login(loginRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }
    }
}
