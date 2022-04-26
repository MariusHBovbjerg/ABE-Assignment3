namespace ExternalService.Dto;

public class ReservationResponse
{
    public Guid orderId { get; set; }
    public int hotelId { get; set; }
    public DateTimeOffset checkIn { get; set; }
    public DateTimeOffset checkOut { get; set; }
    public int roomNo { get; set; }
    public string customerName { get; set; }
    public string customerEmail { get; set; }
    public string customerAddress { get; set; }
        

    public ReservationResponse(
        int hotelId, 
        DateTimeOffset checkIn, 
        DateTimeOffset checkOut, 
        int roomNo, 
        string customerName, 
        string customerEmail, 
        string customerAddress, 
        Guid orderId)
    {
        this.hotelId = hotelId;
        this.checkIn = checkIn;
        this.checkOut = checkOut;
        this.roomNo = roomNo;
        this.customerName = customerName;
        this.customerEmail = customerEmail;
        this.customerAddress = customerAddress;
        this.orderId = orderId;
    }
}