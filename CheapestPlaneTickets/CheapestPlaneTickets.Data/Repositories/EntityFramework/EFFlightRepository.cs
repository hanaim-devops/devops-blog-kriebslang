using CheapestPlaneTickets.Data.Models;

namespace CheapestPlaneTickets.Data.Repositories.EntityFramework;

public class EFFlightRepository(CheapestPlaneTicketsDbContext context) : IFlightRepository
{
    public List<Flight> GetFlights()
    {
        return context.Flights.ToList();
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