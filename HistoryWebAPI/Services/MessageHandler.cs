using RabbitMQServiceLib;

namespace HistoryWebAPI.Services
{
    public class MessageHandler: BackgroundService
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly string _requestQueue;

        public MessageHandler(RabbitMQService rabbitMQService, string requestQueue)
        {
            _rabbitMQService = rabbitMQService;
            _requestQueue = requestQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Receive a message from the RabbitMQ queue
                    _rabbitMQService.ReceiveMessage(_requestQueue, HandleMessageAsync);

                    // Process the received message
                    await ProcessMessageAsync(message);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during message processing
                    // You can log the error or take appropriate action based on your application's requirements
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }

        private async Task HandleMessageAsync(string message)
        {
            Console.WriteLine($"Received message: {message}");

            var response = $"Response to '{message}'";
            _rabbitMQService.SendMessage(_requestQueue, response);
        }
    }
}
