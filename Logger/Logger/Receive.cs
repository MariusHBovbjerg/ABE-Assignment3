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
        channel.BasicQos(0,1,true); // prefetch only one message at a time

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

            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Received Reservation Confirmation for Reservation: {0} for room {1} in hotel {2}", cmd?.orderId, cmd?.roomNo, cmd?.hotelId);
        };
        
        channel.BasicConsume(ConfirmationQueue, true, consumer);
            
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}