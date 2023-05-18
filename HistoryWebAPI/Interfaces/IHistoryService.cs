using HistoryWebAPI.Models;

namespace HistoryWebAPI.Interfaces
{
    public interface IHistoryService
    {
        Task<GeneralResponse> Get(HistoryRequest historyRequest);
        Task Add(DoorUnlockInfo doorLockInfo);
    }
}
