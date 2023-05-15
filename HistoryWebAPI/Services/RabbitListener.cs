using HistoryWebAPI.Controllers;
using RabbitMQServiceLib;

namespace HistoryWebAPI.Services
{
    public class RabbitListener : BackgroundService
    {
        IBus _bus;

        public RabbitListener(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.ReceiveAsync<DoorUnlockInfo>("historyQueue", onMessage);
        }

        public void onMessage(DoorUnlockInfo unlockInfo)
        {
            Console.WriteLine($"{unlockInfo.DoorName}, {unlockInfo.Role}");
        }
    }
}
