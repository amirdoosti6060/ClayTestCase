using RabbitMQ.Client;

namespace RabbitMQServiceLib
{
    public class RabbitBusBuilder
    {
        private static ConnectionFactory _factory = null!;
        private static IConnection _connection = null!;
        private static IModel _channel = null!;

        private string _hostName = "localhost";
        private string _userName = null!;
        private string _password = null!;
        private int _port = 0;
        private string _virtualHost = null!;

        public RabbitBusBuilder()
        {
        }

        public RabbitBusBuilder HostName (string value)
        {
            _hostName = value;
            return this;
        }

        public RabbitBusBuilder UserName (string value)
        { 
            _userName = value;
            return this;
        }

        public RabbitBusBuilder Password (string value)
        { 
            _password = value; 
            return this;
        }

        public RabbitBusBuilder Port (ushort value)
        { 
            _port = value; 
            return this;
        }

        public RabbitBusBuilder VirtualHost (string value)
        { 
            _virtualHost = value; 
            return this;
        }

        public IBus build()
        {
            _factory = new ConnectionFactory
            {
                DispatchConsumersAsync = true
            };

            if (_hostName != null) _factory.HostName = _hostName;
            if (_userName != null) _factory.UserName = _userName;
            if (_password != null) _factory.Password = _password;
            if (_port != 0) _factory.Port = _port;
            if (_virtualHost != null) _factory.VirtualHost = _virtualHost;

            bool connected = false;

            while (!connected)
            {
                try
                {
                    _connection = _factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    connected = true;
                }
                catch
                {
                    Task.Delay(1000);
                }
            }

            return new RabbitBus(_channel);
        }

        public static IBus CreateBus(string hostName, string username, string password)
        {
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = username,
                Password = password,
                DispatchConsumersAsync = true
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            return new RabbitBus(_channel);
        }
        public static IBus CreateBus(
        string hostName,
        ushort hostPort,
        string virtualHost,
        string username,
        string password)
        {
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = hostPort,
                VirtualHost = virtualHost,
                UserName = username,
                Password = password,
                DispatchConsumersAsync = true
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            return new RabbitBus(_channel);
        }
    }
}
