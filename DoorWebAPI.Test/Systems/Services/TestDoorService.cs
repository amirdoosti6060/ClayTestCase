using DoorWebAPI.Models;
using DoorWebAPI.Services;
using DoorWebAPI.Test.Systems.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQServiceLib;
using RabbitMQ.Client;

namespace DoorWebAPI.Test.Systems.Services
{
    public class TestDoorService : IDisposable
    {
        private readonly DoorDbContext _dbContext;
        private readonly DoorService _doorService;

        public TestDoorService()
        {
            var options = new DbContextOptionsBuilder<DoorDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new DoorDbContext(options);
            _dbContext.Database.EnsureCreated();
            var moqLoggerDoorService = new Mock<ILogger<DoorService>>();
            var moqIModel = new Mock<IModel>();
            var moqIBus = new Mock<RabbitBus>(moqIModel.Object);
            LockHandlerSettings lockHandlerSettings = new LockHandlerSettings
            {
                Url = "https://localhost:7019/api/Unlock?hardwareid={hardwareid}"
            };

            _doorService = new DoorService(_dbContext,
                moqIBus.Object,
                moqLoggerDoorService.Object,
                Options.Create(lockHandlerSettings));
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetAll_MatchCount_Success()
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.NotNull(resp.Data);
            Assert.Equal(doors.Count, (resp.Data as List<Door>)!.Count);
        }

        [Fact]
        public async Task GetAll_NotFound()
        {
            //Arrange

            //Act
            var resp = await _doorService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_Success(long doorId)
        {
            //Arrange
            await _dbContext.Doors.AddRangeAsync(DoorMockData.GetDoors());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorService.Get(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_NotFound(long doorId)
        {
            //Arrange
            await _dbContext.Doors.AddRangeAsync(DoorMockData.GetDoors());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorService.Get(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
            Assert.NotNull(resp.Message);
        }

        [Fact]
        public async Task Add_Success()
        {
            //Arrange
            Door door = DoorMockData.GetDoors()[0];
            AddUpdateDoorRequest addUpdateDoorRequest = new AddUpdateDoorRequest
            {
                Name = door.Name,
                HardwareId = door.HardwareId
            };

            //Act
            var resp = await _doorService.Add(addUpdateDoorRequest);
            var getresp = await _doorService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Single((getresp.Data as List<Door>)!);
        }

        [Fact]
        public async Task Update_Success()
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            Door door = doors[0];
            string changedName = door.Name + "Chg";
            string changedHardwareId = door.HardwareId + "Chg";
            AddUpdateDoorRequest addUpdateDoorRequest = new AddUpdateDoorRequest
            {
                Name = changedName,
                HardwareId = changedHardwareId
            };

            //Act
            var resp = await _doorService.Update(door.Id, addUpdateDoorRequest);
            var getresp = await _doorService.Get(door.Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Equal(changedName, (getresp.Data as Door)!.Name);
            Assert.Equal(changedHardwareId, (getresp.Data as Door)!.HardwareId);
        }

        [Theory]
        [InlineData(0)]
        public async Task Update_NotFound(long userId)
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            Door door = doors[0];
            AddUpdateDoorRequest addUpdateDoorRequest = new AddUpdateDoorRequest
            {
                Name = door.Name + "Chg",
                HardwareId = door.HardwareId + "Chg"
            };

            //Act
            var resp = await _doorService.Update(userId, addUpdateDoorRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }

        [Fact]
        public async Task Delete_Success()
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();
            var nDoors = doors.Count;

            //Act
            var resp = await _doorService.Delete(doors[0].Id);
            var getresp = await _doorService.GetAll();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.Code);
            Assert.Equal(StatusCodes.Status200OK, getresp.Code);
            Assert.Equal(nDoors - 1, (getresp.Data as List<Door>)!.Count);
        }

        [Theory]
        [InlineData(0)]
        public async Task Delete_NotFound(long doorId)
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorService.Delete(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }
        /*
        [Fact]
        public async Task Unlock_Success()
        {
            //Arrang
            var users = UserMockData.GetUsers();
            var doors = DoorMockData.GetDoors();
            UserInfo userInfo = new UserInfo
            {
                Id = users[0].Id,
                Email = users[0].Email,
                FullName = users[0].FullName,
                Role = users[0].Role
            };

            // Act
            var resp = await _doorService.Unlock(userInfo, doors[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.Code);
        }*/
    }
}