using System;
using System.Text;
using Consumer.Models.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

public static class Receive
{
    private const string ConfirmationQueue = "ConfirmationQueue";
        
    public static void Main()
    {
        var factory = new ConnectionFactory { 
            HostName = "localhost",//Environment.GetEnvironmentVariable("RabbitMqHost"),
            /*UserName = "user",//Environment.GetEnvironmentVariable("RabbitMqUsername"),
            Password = "pass"//Environment.GetEnvironmentVariable("RabbitMqPassword")*/
        };
        
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

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

            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Received Command Type: {0} with message {1}", cmd.hotelId, cmd.roomNo);
        };
        
        channel.BasicConsume(ConfirmationQueue, true, consumer);
            
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}