namespace HistoryWebAPI.Services
{
    public class LockHardware
    {
        public async Task<string> UnLock(string hardwareId, CancellationToken token)
        {
            if (hardwareId.StartsWith("o-")) // open without problem
                return await Task.FromResult("Ok");
            else if (hardwareId.StartsWith("d-")) // Delay for long time then return timeout
            {
                await Task.Delay(3000, token);
                return await Task.FromResult("Timeout");
            }

            return await Task.FromResult("Fail");
        }
    }
}
