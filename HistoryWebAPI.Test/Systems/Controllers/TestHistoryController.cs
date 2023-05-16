using HistoryWebAPI.Controllers;
using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using HistoryWebAPI.Test.Helper;
using HistoryWebAPI.Test.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HistoryWebAPI.Test.Systems.Controllers
{
    public class TestHistoryController: IDisposable
    {
        private readonly HistoryDbContext _dbContext;
        private readonly HistoryService _historyService;
        private readonly HistoryController _historyController;

        public TestHistoryController()
        {
            var options = new DbContextOptionsBuilder<HistoryDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new HistoryDbContext(options);
            _dbContext.Database.EnsureCreated();

            _historyService = new HistoryService(_dbContext);
            _historyController = new HistoryController(_historyService);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


        [Fact]
        public async Task GetByDate_Ok()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByDate(dt.Year, dt.Month, dt.Day);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Theory]
        [InlineData(10, 10, 10)]
        public async Task GetByDate_NotFound(int year, int month, int day)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByDate(year, month, day);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task GetByDoorId_Ok()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByDoorId(histories[0].DoorId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetByDoorId_NotFound(long doorId)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByDoorId(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task GetByRole_Ok()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByRole(histories[0].Role);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Theory]
        [InlineData("NoRole")]
        public async Task GetByRole_NotFound(string role)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByRole(role);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Fact]
        public async Task GetByUserId_Ok()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByUserId(histories[0].UserId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.GetGeneralResponse()!.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetByUserId_NotFound(long userId)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyController.GetByUserId(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.GetGeneralResponse()!.ErrorCode);
        }

    }
}
