namespace RabbitMQServiceLib
{
    public interface IBus
    {
        Task SendAsync<T>(string queue, T message);
        Task ReceiveAsync<T>(string queue, Func<T, Task> onMessage);
    }
}
