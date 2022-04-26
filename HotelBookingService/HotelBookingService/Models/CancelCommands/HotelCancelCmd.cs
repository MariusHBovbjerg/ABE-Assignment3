namespace Consumer.Models.CancelCommands
{
    public class HotelCancelCmd : Command
    {
        public new string Name { get; set; }
        public new const string Type = "HotelCancelCommand";

        public HotelCancelCmd(string hotelName)
        {
            Name = hotelName;
        }
    }
}