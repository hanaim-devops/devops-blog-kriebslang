using CheapestPlaneTickets.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapestPlaneTickets.Data;

public class CheapestPlaneTicketsDbContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Plane> Planes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;User Id=sa;Password=d8bc90AD1d4e5e5cC98352d2670bb7ea!;Database=CheapestPlaneTickets;TrustServerCertificate=True;");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Plane);

        modelBuilder.Entity<Flight>()
            .HasMany(f => f.Tickets);

        modelBuilder.Entity<Passenger>()
            .HasMany(f => f.Tickets)
            .WithOne(t => t.Passenger);
    }
    
}