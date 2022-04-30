using System;
using System.Linq;
using Consumer.Database;
using Consumer.Models.Dto;

namespace Consumer.ReservationUtil;

public static class ConflictCheck
{
    public static Reservation EnsureNoConflictingReservation(ReservationRequest request)
    {
        var db = new ReservationDbContext();
        // Ensure no conflicting reservations
        var conflictingReservations = db.Reservations.Where(r => 
            r.hotelId == request.hotelId 
            && r.roomNo == request.roomNo 
            && r.checkIn <= request.checkOut
            && r.checkOut >= request.checkIn).ToList(); 
        
        if (conflictingReservations.Any())
        {
            throw new Exception("Conflicting reservation found");
        }
        // otherwise save the reservation
        var reservation = Reservation.MapDtoToReservation(request);

        db.Reservations.Add(reservation);
        db.SaveChanges();
        return reservation;
    }
}