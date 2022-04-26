using Producer.Models.BookingCommands;

namespace Producer.Models.CancelCommands
{
    public class CarCancelCmd : Command
    {
        public string Name { get; }
        public string Type => "CarCancelCommand";

        public CarCancelCmd(CarBookingCmd car)
        {
            Name = car.Name;
        }
    }
}
