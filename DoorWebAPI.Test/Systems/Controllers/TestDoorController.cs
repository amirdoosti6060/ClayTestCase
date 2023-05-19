using DoorWebAPI.Controllers;
using DoorWebAPI.Models;
using DoorWebAPI.Services;
using DoorWebAPI.Test.Helper;
using DoorWebAPI.Test.Systems.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorWebAPI.Test.Systems.Controllers
{
    public class TestDoorController: IDisposable
    {
        private readonly DoorDbContext _dbContext;
        private readonly DoorController _doorController;

        public TestDoorController()
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

            var doorService = new DoorService(_dbContext,
                moqIBus.Object,
                moqLoggerDoorService.Object,
                Options.Create(lockHandlerSettings));

            _doorController = new DoorController(doorService);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


        [Fact]
        public async Task GetAll_MatchCount_Ok()
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorController.Get();

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task GetAll_NotFound()
        {
            //Arrange

            //Act
            var resp = await _doorController.Get();

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_Ok(long doorId)
        {
            //Arrange
            await _dbContext.Doors.AddRangeAsync(DoorMockData.GetDoors());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorController.Get(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_NotFound(long doorId)
        {
            //Arrange
            await _dbContext.Doors.AddRangeAsync(DoorMockData.GetDoors());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorController.Get(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Add_Ok()
        {
            //Arrange
            Door door = DoorMockData.GetDoors()[0];
            AddUpdateDoorRequest addUpdateDoorRequest = new AddUpdateDoorRequest
            {
                Name = door.Name,
                HardwareId = door.HardwareId
            };

            //Act
            var resp = await _doorController.Post(addUpdateDoorRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Update_Ok()
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
            var resp = await _doorController.Put(door.Id, addUpdateDoorRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
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
            var resp = await _doorController.Put(userId, addUpdateDoorRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }

        [Fact]
        public async Task Delete_Ok()
        {
            //Arrange
            var doors = DoorMockData.GetDoors();
            await _dbContext.Doors.AddRangeAsync(doors);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _doorController.Delete(doors[0].Id);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.Code);
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
            var resp = await _doorController.Delete(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.Code);
        }
    }
}
