using HistoryWebAPI.Interfaces;
using HistoryWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Serilog;

namespace HistoryWebAPI.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly HistoryDbContext _dbContext;
        private readonly ILogger<HistoryService> _logger;

        public HistoryService(HistoryDbContext dbContext, ILogger<HistoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        private string GetOperator(string? str)
        {
            return string.IsNullOrEmpty(str) ? "": " AND ";
        }

        private string BuildQuery(HistoryRequest historyRequest)
        {
            string qry = "";
            if (historyRequest.userId != null)
                qry += $"userId = {historyRequest.userId}";
            if (historyRequest.doorId != null)
                qry += GetOperator(qry) + $"doorId = {historyRequest.doorId}";
            if (!string.IsNullOrEmpty(historyRequest.role))
                qry += GetOperator(qry) + $"role = \"{historyRequest.role}\"";
            if (historyRequest.year != null ||
                historyRequest.month != null ||
                historyRequest.day != null)
            {
                DateTime dt = DateTime.Now;
                int yy = historyRequest.year ?? dt.Year;
                int mm = historyRequest.month ?? dt.Month;
                int dd = historyRequest.day ?? dt.Day;
                dt = new DateTime(yy, mm, dd);

                qry += GetOperator(qry) + $"timestamp.Date = \"{yy}-{mm}-{dd}\"";
            }

            return qry;
        }

        public async Task<GeneralResponse> Get(HistoryRequest historyRequest)
        {
            GeneralResponse response = new GeneralResponse()
            {
                ErrorCode = StatusCodes.Status200OK
            };

            string qry = BuildQuery(historyRequest);

            if (string.IsNullOrEmpty(qry))
            {
                response.ErrorCode = StatusCodes.Status400BadRequest;
                response.ErrorMessage = "Requested is not valid!";
            }
            else
            {
                var querable = _dbContext.History.Where(qry);

                if (historyRequest.top != null)
                    querable = querable.Take(historyRequest.top ?? 0);

                response.Data = await querable.ToListAsync();

                if ((response.Data as List<History>)!.Count == 0)
                {
                    response.ErrorCode = StatusCodes.Status404NotFound;
                    response.ErrorMessage = "Requested history not found!";
                }
            }

            _logger.LogDebug($"Generated query: {qry}!");

            return response;
        }
        
        public async Task Add(DoorUnlockInfo doorLockInfo)
        {
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
            int n = await _dbContext.SaveChangesAsync();
            
            _logger.LogDebug($"A history added. return {n}");
        }
    }
}
