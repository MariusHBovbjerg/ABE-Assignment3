namespace Producer.Models.BookingCommands
{
    public class FlightBookingCmd : Command
    {
        public string Name { get; }
        public string Type => "FlightBookingCommand";

        public FlightBookingCmd(string flightName)
        {
            Name = flightName;
        }
    }
}
