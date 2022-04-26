namespace Consumer.Models.BookingCommands
{
    public class HotelBookingCmd : Command
    {
        public string Name { get; }
        public const string Type = "HotelBookingCommand";

        public HotelBookingCmd(string hotelName)
        {
            Name = hotelName;
        }
    }
}