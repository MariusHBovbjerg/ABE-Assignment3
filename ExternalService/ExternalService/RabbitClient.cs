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
        var factory = new ConnectionFactory { 
            HostName = "localhost",//Environment.GetEnvironmentVariable("RabbitMqHost"),
            /*UserName = "user",//Environment.GetEnvironmentVariable("RabbitMqUsername"),
            Password = "pass"//Environment.GetEnvironmentVariable("RabbitMqPassword")*/
        };

        _connection = factory.CreateConnection();
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
}