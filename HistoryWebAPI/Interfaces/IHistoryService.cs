namespace HistoryWebAPI.Interfaces
{
    public interface IHistoryService
    {
        Task<GeneralResponse> GetByDate(int year, int month, int day);
        Task<GeneralResponse> GetByUserId(long userId);
        Task<GeneralResponse> GetByDoorId(long doorId);
        Task<GeneralResponse> GetByRole(string role);
        Task<GeneralResponse> Add(AddHistoryRequest addHistoryRequest);
    }
}
