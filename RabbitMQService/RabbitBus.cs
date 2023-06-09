﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQServiceLib
{
    public class RabbitBus : IBus
    {
        private readonly IModel _channel;
        public RabbitBus(IModel channel)
        {
            _channel = channel;
        }
        public async Task SendAsync<T>(string queue, T message)
        {
            await Task.Run(() =>
            {
                _channel.QueueDeclare(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = false; 

                var output = JsonConvert.SerializeObject(message);
                _channel.BasicPublish(
                    string.Empty, 
                    queue, 
                    null,
                    Encoding.UTF8.GetBytes(output));
            });
        }
        public async Task ReceiveAsync<T>(string queue, Func<T, Task> onMessage)
        {
            _channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (s, e) =>
            {
                var jsonSpecified = Encoding.UTF8.GetString(e.Body.Span);
                var item = JsonConvert.DeserializeObject<T>(jsonSpecified);
                await onMessage(item!);
            };
            _channel.BasicConsume(queue, true, consumer);
            await Task.Yield();
        }
    }
}
