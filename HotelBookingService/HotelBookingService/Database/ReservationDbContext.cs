using System;
using Consumer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Database;

public class ReservationDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString =
            @"Server=" + (Environment.GetEnvironmentVariable("MSSQL_HOST")?? "[::1]") + ","
            + (Environment.GetEnvironmentVariable("MSSQL_PORT")?? "1433") + ";" 
            + "Database=ReservationDb;User Id=SA;Password="
            + (Environment.GetEnvironmentVariable("SA_PASSWORD") ?? "yourStrong(!)Password") + ";"
            + "Trusted_Connection=false;";
        optionsBuilder.UseSqlServer(connectionString);
    }

    public DbSet<Reservation> Reservations { get; set; }
}