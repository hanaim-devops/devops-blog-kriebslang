using CheapestPlaneTickets.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapestPlaneTickets.Data.Repositories.EntityFramework;

public class EFRawFlightRepository(CheapestPlaneTicketsDbContext context) : IFlightRepository
{
    public List<Flight> GetFlights()
    {
        return context.Flights.FromSqlRaw("SELECT * FROM Flights").ToList();
    }

    public Flight? GetFlight(int id)
    {
        return context.Flights.Find(id);
    }

    public void AddFlight(Flight flight)
    {
        context.Flights.Add(flight);
        context.SaveChanges();
    }
}