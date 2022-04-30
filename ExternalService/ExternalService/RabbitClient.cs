using RabbitMQ.Client;
using System.Text;
using ExternalService.Dto;
using Newtonsoft.Json;

namespace ExternalService;

public class RabbitClient
{
    private const string ReservationQueue = "ReservationQueue";

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitClient()
    {
        _connection = RetryRabbitMqConnection();
        Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Connected");
        _channel = _connection.CreateModel();
        // declare a server-named queue
        _channel.QueueDeclare(queue: ReservationQueue,
            durable:false, 
            exclusive: false, 
            autoDelete:false, 
            arguments:null);
    }

    public void SendRequest(ReservationRequest request)
    {
        var obj = JsonConvert.SerializeObject(request);
        var body = Encoding.UTF8.GetBytes(obj);

        _channel.BasicPublish(exchange: "",
            routingKey: "ReservationQueue",
            basicProperties: null,
            body: body);
        Console.WriteLine(" [x] Sent {0}", request);
    }

    private IConnection RetryRabbitMqConnection()
    {
        var factory = new ConnectionFactory { 
            HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST")?? "localhost",
            Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT")?? "5672"),
            UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
            Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest",
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(2)
        };
        
        try {
            return factory.CreateConnection();
        } catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e) {
            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Failed to connect. Retrying");
            Thread.Sleep(1000);
            return RetryRabbitMqConnection();
        }
    }
}