using HistoryWebAPI.Interfaces;

namespace HistoryWebAPI.Services
{
    public class HistoryService : IHistoryService
    {
        public Task<GeneralResponse> GetByDate(int year, int month, int day)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetByDoorId(long doorId)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetByRole(string role)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> GetByUserId(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> Add(AddHistoryRequest addHistoryRequest)
        {
            throw new NotImplementedException();
        }
    }
}
