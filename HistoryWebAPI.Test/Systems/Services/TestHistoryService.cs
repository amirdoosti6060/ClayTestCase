using HistoryWebAPI.Models;
using HistoryWebAPI.Services;
using HistoryWebAPI.Test.MockData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace HistoryWebAPI.Test.Systems.Services
{
    public class TestHistoryService : IDisposable
    {
        private readonly HistoryDbContext _dbContext;
        private readonly HistoryService _historyService;

        public TestHistoryService()
        {
            var options = new DbContextOptionsBuilder<HistoryDbContext>()
                                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                                .Options;
            _dbContext = new HistoryDbContext(options);
            _dbContext.Database.EnsureCreated();

            _historyService = new HistoryService(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Fact]
        public async Task GetByDate_Success()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByDate(dt.Year, dt.Month, dt.Day);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(10, 10, 10)]
        public async Task GetByDate_NotFound(int year, int month, int day)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByDate(year, month, day);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
            Assert.NotNull(resp.ErrorMessage);
        }

        [Fact]
        public async Task GetByDoorId_Success()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByDoorId(histories[0].DoorId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetByDoorId_NotFound(long doorId)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByDoorId(doorId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }

        [Fact]
        public async Task GetByRole_Success()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByRole(histories[0].Role);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData("NoRole")]
        public async Task GetByRole_NotFound(string role)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByRole(role);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }

        [Fact]
        public async Task GetByUserId_Success()
        {
            //Arrange
            DateTime dt = DateTime.Now;
            var histories = HistoryMockData.GetHistories();
            await _dbContext.History.AddRangeAsync(histories);
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByUserId(histories[0].UserId);

            //Assert
            Assert.Equal(StatusCodes.Status200OK, resp.ErrorCode);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetByUserId_NotFound(long userId)
        {
            //Arrange
            await _dbContext.History.AddRangeAsync(HistoryMockData.GetHistories());
            await _dbContext.SaveChangesAsync();

            //Act
            var resp = await _historyService.GetByUserId(userId);

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, resp.ErrorCode);
        }

        [Fact]
        public async Task Add_Success()
        {
            //Arrange
            var histories = HistoryMockData.GetHistories();

            var doorLockInfo = new DoorUnlockInfo
            {
               DoorId = histories[0].DoorId,
               DoorName = histories[0].DoorName,
               HardwareId = histories[0].HardwareId,
               UserId = histories[0].UserId,
               Email = histories[0].Email,
               FullName = histories[0].FullName,
               Role = histories[0].Role,
               ActionStatus = histories[0].ActionStatus,
               TimeStamp = histories[0].TimeStamp
            };

            //Act
            await _historyService.Add(doorLockInfo);

            //Assert
            Assert.True(true);
        }
    }
}
