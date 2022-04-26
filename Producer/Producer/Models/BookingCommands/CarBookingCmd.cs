namespace Producer.Models.BookingCommands
{
    public class CarBookingCmd : Command
    {
        public string Name { get; }
        public string Type => "CarBookingCommand";

        public CarBookingCmd(string carName)
        {
            Name = carName;
        }
    }
}
