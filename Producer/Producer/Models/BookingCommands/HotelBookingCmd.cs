namespace Producer.Models.BookingCommands
{
    public class HotelBookingCmd : Command
    {
        public string Name { get; }
        public string Type => "HotelBookingCommand";

        public HotelBookingCmd(string hotelName)
        {
            Name = hotelName;
        }
    }
}
