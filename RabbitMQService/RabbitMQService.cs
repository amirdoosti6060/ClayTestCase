using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQServiceLib
{
    public class RabbitMQService : IDisposable
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _responseQueue;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingRequests;

        public RabbitMQService(string hostName, string userName, string password)
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _responseQueue = Guid.NewGuid().ToString(); // Unique queue for receiving responses
            _pendingRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

            var factory = new ConnectionFactory() 
            { 
                HostName = _hostName, 
                UserName = _userName, 
                Password = _password 
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _responseQueue, 
                durable: false, 
                exclusive: true, 
                autoDelete: true, 
                arguments: null);
        }

        public void InitiateRequestReplyPattern()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += HandleResponseMessage!;
            _channel.BasicConsume(queue: _responseQueue, autoAck: true, consumer: consumer);
        }

        public async Task<string> SendRequestAndWaitForResponse(string requestQueue, string message, TimeSpan timeout)
        {
            var correlationId = Guid.NewGuid().ToString();
            var completionSource = new TaskCompletionSource<string>();
            _pendingRequests.TryAdd(correlationId, completionSource);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = _responseQueue;

            _channel.QueueDeclare(
                queue: requestQueue, 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", 
                routingKey: requestQueue, 
                basicProperties: properties, 
                body: body);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);

            var responseTask = completionSource.Task;
            var timeoutTask = Task.Delay(timeout, cancellationTokenSource.Token);

            var completedTask = await Task.WhenAny(responseTask, timeoutTask);
            if (completedTask == responseTask)
            {
                cancellationTokenSource.Cancel(); // Cancel the timeout task if the response is received
                return await responseTask;
            }
            else
            {
                return await Task.FromResult("Timeout");
                //throw new TimeoutException("Response not received within the specified timeout.");
            }
        }

        private void HandleResponseMessage(object sender, BasicDeliverEventArgs e)
        {
            var correlationId = e.BasicProperties.CorrelationId;
            if (_pendingRequests.TryRemove(correlationId, out var completionSource))
            {
                var body = e.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);

                completionSource.SetResult(response);
            }
        }

        public void SendMessage(string queueName, string message)
        {
            _channel.QueueDeclare(
                queue: queueName, 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", 
                routingKey: queueName, 
                basicProperties: null, 
                body: body);
        }

        public void ReceiveMessage(string queueName, Func<string, Task> messageHandler)
        {
            _channel.QueueDeclare(
                queue: queueName, 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}