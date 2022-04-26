using Consumer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Database;

public class ReservationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=[::1],1433;Database=ReservationDb;User Id=SA;Password=yourStrong(!)Password;Trusted_Connection=false;");
    }

    public DbSet<Reservation> Reservations { get; set; }
}