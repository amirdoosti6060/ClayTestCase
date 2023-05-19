using DoorWebAPI.Controllers;
using DoorWebAPI.Models;
using DoorWebAPI.Services;
using DoorWebAPI.Test.Helper;
using DoorWebAPI.Test.Systems.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionWebAPI.Test.Systems.Controllers
{
    public class TestPermissionController : IDisposable
    {
        private readonly DoorDbContext _dbContext;
        private readonly PermissionController _permissionController;

        public TestPermissionController()
        {
            var options = new DbContextOptionsBuilder<DoorDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new DoorDbContext(options);
            _dbContext.Database.EnsureCreated();
            var moqLoggerPermissionService = new Mock<ILogger<PermissionService>>();
            var moqLoggerPermissionController = new Mock<ILogger<PermissionController>>();

            var permissionService = new PermissionService(_dbContext,
                moqLoggerPermissionService.Object);
            _permissionController = new PermissionController(permissionService,
                moqLoggerPermissionController.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task Get_ByPermissionId_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Get(permissions[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_ByPermissionId_NotFound(long permId)
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Get(permId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }


        [Fact]
        public async Task Get_ByDoorId_OK()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            GetPermissionRequest getPermissionRequest = new GetPermissionRequest
            {
                doorId = permissions[0].DoorId
            };

            //Act
            var resp = await _permissionController.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Get_ByRole_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            GetPermissionRequest getPermissionRequest = new GetPermissionRequest
            {
                role = permissions[0].Role
            };

            //Act
            var resp = await _permissionController.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Get_ByRoleAndDoorId_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            GetPermissionRequest getPermissionRequest = new GetPermissionRequest
            {
                doorId = permissions[0].DoorId,
                role = permissions[0].Role
            };

            //Act
            var resp = await _permissionController.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Get_All_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionController.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            //Arrange
            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionController.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Add_Ok()
        {
            //Arrange
            Permission permission = PermissionMockData.GetPermissions()[0];
            AddPermissionRequest addUpdatePermissionRequest = new AddPermissionRequest
            {
                DoorId = permission.DoorId,
                Role = permission.Role
            };

            //Act
            var resp = await _permissionController.Post(addUpdatePermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Delete_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Delete(permissions[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Delete_NotFound(long permissionId)
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Delete(permissionId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Delete_ByDoorIdAndRole_Ok()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Delete(permissions[0].Id, permissions[0].Role);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(0, "bad_role")]
        public async Task Delete_ByDoorIdAndRole_NotFound(long permissionId, string role)
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionController.Delete(permissionId, role);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }
    }
}