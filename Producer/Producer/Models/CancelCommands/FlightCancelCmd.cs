using Producer.Models.BookingCommands;

namespace Producer.Models.CancelCommands
{
    public class FlightCancelCmd : Command
    {
        public string Name { get; }
        public string Type => "FlightCancelCommand";
        
        public FlightCancelCmd(FlightBookingCmd flight)
        {
            Name = flight.Name;
        }
    }
}
