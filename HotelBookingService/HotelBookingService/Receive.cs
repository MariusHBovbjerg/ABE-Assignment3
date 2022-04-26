using System;
using System.Text;
using Consumer.Database;
using Consumer.Models.Dto;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

public static class Receive
{
    private const string ReservationQueue = "ReservationQueue";
    private const string ConfirmationQueue = "ConfirmationQueue";
        
    public static void Main()
    {
        var factory = new ConnectionFactory { 
            HostName = "localhost",//Environment.GetEnvironmentVariable("RabbitMqHost"),
            /*UserName = "user",//Environment.GetEnvironmentVariable("RabbitMqUsername"),
            Password = "pass"//Environment.GetEnvironmentVariable("RabbitMqPassword")*/
        };
        var db = new ReservationDbContext();
        
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

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

            Console.WriteLine(Environment.MachineName + " - " + DateTime.Now.Millisecond +" - Received Command Type: {0} with message {1}", cmd.hotelId, cmd.roomNo);
            var reservation = Reservation.MapDtoToReservation(cmd);
            db.Reservations.Add(reservation);
            db.SaveChanges();
            
            var obj = JsonConvert.SerializeObject(reservation);
            var newBody = Encoding.UTF8.GetBytes(obj);

            channel.BasicPublish(exchange: "",
                routingKey: "ConfirmationQueue",
                basicProperties: null,
                body: newBody);
            Console.WriteLine(" [x] Sent {0}", reservation);
        };
        
        channel.BasicConsume(ReservationQueue, true, consumer);
            
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}