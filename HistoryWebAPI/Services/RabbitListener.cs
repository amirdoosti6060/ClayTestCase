using HistoryWebAPI.Interfaces;
using HistoryWebAPI.Models;
using RabbitMQServiceLib;

namespace HistoryWebAPI.Services
{
    public class RabbitListener : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        // Using IServicePrvider is because current service is singleton but
        // IHistoryService is registered as transient
        public RabbitListener(IBus bus, IServiceProvider serviceProvider)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.ReceiveAsync<DoorUnlockInfo>("historyQueue", onMessage);
        }

        public async Task onMessage(DoorUnlockInfo unlockInfo)
        {
            // Create a new scope to be able to consume IHistoryService
            using (var scope = _serviceProvider.CreateScope())
            {
                var _historyService = scope.ServiceProvider.GetService<IHistoryService>();
                if (_historyService != null)
                    await _historyService.Add(unlockInfo);
            }
        }
    }
}
