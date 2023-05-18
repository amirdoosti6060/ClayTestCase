using HistoryWebAPI.Controllers;
using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using HistoryWebAPI.Test.Helper;
using HistoryWebAPI.Test.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HistoryWebAPI.Test.Systems.Controllers
{
    public class TestHistoryController: IDisposable
    {
        private readonly HistoryDbContext _dbContext;
        private readonly HistoryController _historyController;

        public TestHistoryController()
        {
            var options = new DbContextOptionsBuilder<HistoryDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new HistoryDbContext(options);
            _dbContext.Database.EnsureCreated();

            var moqLoggerHistoryService = new Mock<ILogger<HistoryService>>();
            var moqLoggerHistoryController = new Mock<ILogger<HistoryController>>();

            var historyService = new HistoryService(_dbContext, moqLoggerHistoryService.Object);
            _historyController = new HistoryController(historyService, moqLoggerHistoryController.Object);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


        [Fact]
        public async Task Get_ByUserId_Ok()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                userId = histories[0].UserId
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task Get_ByDoorId_Ok()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                doorId = histories[0].DoorId
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task Get_ByRole_Ok()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                role = histories[0].Role
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task Get_ByDate_Ok()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                year = histories[0].TimeStamp.Year,
                month = histories[0].TimeStamp.Month,
                day = histories[0].TimeStamp.Day
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            var generalResponse = resp.GetGeneralResponse();
            Assert.Equal(StatusCodes.Status200OK, generalResponse!.ErrorCode);
            Assert.True((generalResponse.Data as List<History>)!.Count > 0);
        }

        [Theory]
        [InlineData(2)]
        public async Task Get_ByDate_and_Top_Ok(int _top)
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                year = histories[0].TimeStamp.Year,
                month = histories[0].TimeStamp.Month,
                day = histories[0].TimeStamp.Day,
                top = _top
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            var generalResponse = resp.GetGeneralResponse();
            Assert.Equal(StatusCodes.Status200OK, generalResponse!.ErrorCode);
            Assert.Equal(_top, (generalResponse.Data as List<History>)!.Count);
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_All_Ok(int _top)
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                doorId = histories[0].DoorId,
                userId = histories[0].UserId,
                year = histories[0].TimeStamp.Year,
                month = histories[0].TimeStamp.Month,
                day = histories[0].TimeStamp.Day,
                role = histories[0].Role,
                top = _top
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            var generalResponse = resp.GetGeneralResponse();
            Assert.Equal(StatusCodes.Status200OK, generalResponse!.ErrorCode);
            Assert.Equal(_top, (generalResponse.Data as List<History>)!.Count);
        }

        [Theory]
        [InlineData(0)]
        public async Task Get_NotFound(long _doorId)
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest
            {
                doorId = _doorId
            };

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task Get_BadRequest()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            HistoryRequest historyRequest = new HistoryRequest();

            //Act
            var resp = await _historyController.Get(historyRequest);

            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, resp.GetGeneralResponse()!.ErrorCode);
        }
    }
}
