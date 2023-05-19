using DoorWebAPI.Models;
using DoorWebAPI.Services;
using DoorWebAPI.Test.Systems.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using RabbitMQServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionWebAPI.Test.Systems.Services
{
    public class TestPermissionService : IDisposable
    {
        private readonly DoorDbContext _dbContext;
        private readonly PermissionService _permissionService;

        public TestPermissionService()
        {
            var options = new DbContextOptionsBuilder<DoorDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new DoorDbContext(options);
            _dbContext.Database.EnsureCreated();
            var moqLoggerPermissionService = new Mock<ILogger<PermissionService>>();

            _permissionService = new PermissionService(_dbContext,
                moqLoggerPermissionService.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_ByPermissionId_Success(long permissionId)
        {
            //Arrange
            await _dbContext.Permissions.AddRangeAsync(PermissionMockData.GetPermissions());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionService.Get(permissionId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_ByPermissionId_NotFound(long permissionId)
        {
            //Arrange
            await _dbContext.Permissions.AddRangeAsync(PermissionMockData.GetPermissions());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _permissionService.Get(permissionId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
            Assert.NotNull(resp.Message);
        }

        [Fact]
        public async Task Get_ByDoorId_Success()
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
            var resp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
        }

        [Fact]
        public async Task Get_ByRole_Success()
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
            var resp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
        }

        [Fact]
        public async Task Get_ByRoleAndDoorId_Success()
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
            var resp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
        }

        [Fact]
        public async Task Get_All_Success()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.NotNull(resp.Data);
            Assert.Equal(permissions.Count, (resp.Data as List<Permission>)!.Count);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            //Arrange
            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }

        [Fact]
        public async Task Add_Success()
        {
            //Arrange
            Permission permission = PermissionMockData.GetPermissions()[0];
            AddPermissionRequest addUpdatePermissionRequest = new AddPermissionRequest
            {
                DoorId = permission.DoorId,
                Role = permission.Role
            };
            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionService.Add(addUpdatePermissionRequest);
            var getresp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Single((getresp.Data as List<Permission>)!);
        }

        [Fact]
        public async Task Add_BadRequest()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();
            Permission permission = permissions[0];
            AddPermissionRequest addUpdatePermissionRequest = new AddPermissionRequest
            {
                DoorId = permission.DoorId,
                Role = permission.Role
            };

            //Act
            var resp = await _permissionService.Add(addUpdatePermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, resp.Code);
        }

        [Fact]
        public async Task Delete_Success()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();
            var nPermissions = permissions.Count;
            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionService.Delete(permissions[0].Id);
            var getresp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Equal(nPermissions - 1, (getresp.Data as List<Permission>)!.Count);
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
            var resp = await _permissionService.Delete(permissionId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }

        [Fact]
        public async Task Delete_ByDoorIdAndRole_Success()
        {
            //Arrange
            var permissions = PermissionMockData.GetPermissions();
            await _dbContext.Permissions.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();
            var nPermissions = permissions.Count;
            GetPermissionRequest getPermissionRequest = new GetPermissionRequest();

            //Act
            var resp = await _permissionService.Delete(permissions[0].DoorId, permissions[0].Role);
            var getresp = await _permissionService.Get(getPermissionRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Equal(nPermissions - 1, (getresp.Data as List<Permission>)!.Count);
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
            var resp = await _permissionService.Delete(permissionId, role);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }
    }
}
