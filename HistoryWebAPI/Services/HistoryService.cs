using HistoryWebAPI.Interfaces;
using HistoryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoryWebAPI.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly HistoryDbContext _dbContext;

        public HistoryService(HistoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GeneralResponse> GetByDate(int year, int month, int day)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            DateTime selDate = new DateTime(year, month, day);
            response.Data = await _dbContext.History
                                .Where(e => e.TimeStamp.Date.Equals(selDate.Date))
                                .ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Requested history for date {year}/{month}/{day} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetByDoorId(long doorId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.History
                                .Where(e => e.DoorId == doorId)
                                .ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Requested history for door {doorId} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetByRole(string role)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.History
                                .Where(e => e.Role == role)
                                .ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Requested history for role {role} not found!";
            }

            return response;
        }

        public async Task<GeneralResponse> GetByUserId(long userId)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            response.Data = await _dbContext.History
                                .Where(e => e.UserId == userId)
                                .ToListAsync();

            if (response.Data == null)
            {
                response.ErrorCode = StatusCodes.Status404NotFound;
                response.ErrorMessage = $"Requested history for user {userId} not found!";
            }

            return response;
        }

        public async Task Add(DoorUnlockInfo doorLockInfo)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            History history = new History
            {
                DoorId = doorLockInfo.DoorId,
                DoorName = doorLockInfo.DoorName,
                HardwareId = doorLockInfo.HardwareId,
                UserId = doorLockInfo.UserId,
                FullName = doorLockInfo.FullName,
                Email = doorLockInfo.Email,
                Role = doorLockInfo.Role,
                ActionStatus = doorLockInfo.ActionStatus,
                TimeStamp = doorLockInfo.TimeStamp
            };

            _dbContext.History.Add(history);
            if (await _dbContext.SaveChangesAsync() <= 0)
            {
                response.ErrorCode = StatusCodes.Status400BadRequest;
                response.ErrorMessage = $"Unable to add history!";
            }
            else
                response.Data = history.Id;

            //return response;
        }
    }
}
