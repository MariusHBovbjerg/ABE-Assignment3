using System;
using System.Threading.Tasks;
using Producer.Models.BookingCommands;
using Producer.Models.CancelCommands;

namespace Producer
{
    public class SagaHandler
    {
        public static BookingSaga CreateSaga(string hotel, string car, string flight)
        {
            var hotelCmd = new HotelBookingCmd(hotel);
            var carCmd = new CarBookingCmd(car);
            var flightCmd = new FlightBookingCmd(flight);
            return new BookingSaga(hotelCmd, carCmd, flightCmd);
        }

        public static async Task ExecuteSaga(BookingSaga saga)
        {
             var rpcClient = new RpcClient();

            // This could be parallelized, could introduce race-condition concerns, however.
            
            // Hotel Book
            var hotelBookingCmd = saga.HotelBookingCmd;
            
            CommandLog(hotelBookingCmd.Type, hotelBookingCmd.Name);
            var hotelResponse = await rpcClient.CallAsync(hotelBookingCmd, "HotelKey");
            ResponseLog(hotelResponse);

            if (hotelResponse == "HotelFalse")
            {
                var hotelCancelCmd = saga.HotelBookingCancelCmd;
                
                CommandLog(hotelCancelCmd.Type, hotelCancelCmd.Name);
                var hotelCancelResponse = await rpcClient.CallAsync(hotelCancelCmd, "HotelKey");
                ResponseLog(hotelCancelResponse);

                Console.WriteLine("Failed and cancelled booking hotel :(");
                rpcClient.Close();
                return;

            }
            
            // Car Book
            var carBookingCmd = saga.CarBookingCmd;
            
            CommandLog(carBookingCmd.Type, carBookingCmd.Name);
            var carResponse = await rpcClient.CallAsync(carBookingCmd, "CarKey");
            ResponseLog(carResponse);
            
            if (carResponse == "CarFalse")
            {
                var hotelCancelCmd = saga.HotelBookingCancelCmd;
                CommandLog(hotelCancelCmd.Type, hotelCancelCmd.Name);
                var hotelCancelResponse = await rpcClient.CallAsync(hotelCancelCmd, "HotelKey");
                ResponseLog(hotelCancelResponse);
                var carCancelCmd = saga.CarBookingCancelCmd;
                CommandLog(carCancelCmd.Type, carCancelCmd.Name);
                var carCancelResponse = await rpcClient.CallAsync(carCancelCmd, "CarKey");
                ResponseLog(carCancelResponse);
                
                Console.WriteLine("Failed and cancelled booking hotel and car :(");
                rpcClient.Close();
                return;
            }
            
            // Flight Book
            var flightBookingCmd = saga.FlightBookingCmd;
            
            CommandLog(flightBookingCmd.Type, flightBookingCmd.Name);
            var flightResponse = await rpcClient.CallAsync(flightBookingCmd, "FlightKey");
            ResponseLog(flightResponse);
            
            if (flightResponse == "FlightFalse")
            {
                var hotelCancelCmd = saga.HotelBookingCancelCmd;
                CommandLog(hotelCancelCmd.Type, hotelCancelCmd.Name);
                var hotelCancelResponse = await rpcClient.CallAsync(hotelCancelCmd, "HotelKey");
                ResponseLog(hotelCancelResponse);
                var carCancelCmd = saga.CarBookingCancelCmd;
                CommandLog(carCancelCmd.Type, carCancelCmd.Name);
                var carCancelResponse = await rpcClient.CallAsync(carCancelCmd, "CarKey");
                ResponseLog(carCancelResponse);
                var flightCancelCmd = saga.FlightBookingCancelCmd;
                CommandLog(flightCancelCmd.Type, flightCancelCmd.Name);
                var flightCancelResponse = await rpcClient.CallAsync(flightCancelCmd, "FlightKey");
                ResponseLog(flightCancelResponse);
                
                Console.WriteLine("Failed and cancelled booking hotel, car and flight :(");
                rpcClient.Close();
                return;
            }
            
            Console.WriteLine("Successfully booked a hotel, car and flight!!! WOOOO");
            
            rpcClient.Close();
        }
        
        private static  void CommandLog(string type, string name)
        {
            Console.WriteLine("<-- Sending {0} for {1}", type, name);
        }
        
        private static void ResponseLog(string response)
        {
            Console.WriteLine("--> Received '{0}'", response);
        }
    }

    public class BookingSaga
    {
        public HotelBookingCmd HotelBookingCmd { get; set; }
        public CarBookingCmd CarBookingCmd { get; set; }
        public FlightBookingCmd FlightBookingCmd { get; set; }
        public HotelCancelCmd HotelBookingCancelCmd { get; set; }
        public CarCancelCmd CarBookingCancelCmd { get; set; }
        public FlightCancelCmd FlightBookingCancelCmd { get; set; }

        public BookingSaga(HotelBookingCmd hotelBookingCmd, CarBookingCmd carBookingCmd,
            FlightBookingCmd flightBookingCmd)
        {
            HotelBookingCmd = hotelBookingCmd;
            CarBookingCmd = carBookingCmd;
            FlightBookingCmd = flightBookingCmd;
            HotelBookingCancelCmd = new HotelCancelCmd(hotelBookingCmd);
            CarBookingCancelCmd = new CarCancelCmd(carBookingCmd);
            FlightBookingCancelCmd = new FlightCancelCmd(flightBookingCmd);
        }
    }
}
