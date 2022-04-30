using System;
using System.Text;
using System.Threading;
using Consumer.Models.Dto;
using Consumer.ReservationUtil;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

public static class Receive
{
    private const string ReservationQueue = "ReservationQueue";
    private const string ConfirmationQueue = "ConfirmationQueue";
    
    private static IConnection _connection;
        
    public static void Main()
    {
        _connection = RetryRabbitMqConnection();
        Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Connected");
        var channel = _connection.CreateModel();
        channel.BasicQos(0,1,true); // prefetch only one message at a time

        channel.QueueDeclare(queue: ReservationQueue,
            durable:false, 
            exclusive: false, 
            autoDelete:false, 
            arguments:null);

        channel.QueueDeclare(queue: ConfirmationQueue,
            durable:false, 
            exclusive: false, 
            autoDelete:false, 
            arguments:null);
        
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var cmd = JsonConvert.DeserializeObject<ReservationRequest>(message);

            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Received Reservation request for hotel {0} with room {1}.", cmd?.hotelId, cmd?.roomNo);
            var reservation =  ConflictCheck.EnsureNoConflictingReservation(cmd);
            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Saved Reservation");
            
            var obj = JsonConvert.SerializeObject(reservation);
            var newBody = Encoding.UTF8.GetBytes(obj);

            channel.BasicPublish(exchange: "",
                routingKey: "ConfirmationQueue",
                basicProperties: null,
                body: newBody);
            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Sent Confirmation for reservation id {0}", reservation.orderId);
        };
        
        channel.BasicConsume(ReservationQueue, true, consumer);

        for (;;) ;
    }
    
    private static IConnection RetryRabbitMqConnection()
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