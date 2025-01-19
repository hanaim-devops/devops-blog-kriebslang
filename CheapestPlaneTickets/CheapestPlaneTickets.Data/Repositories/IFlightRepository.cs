using CheapestPlaneTickets.Data.Models;

namespace CheapestPlaneTickets.Data.Repositories;

public interface IFlightRepository
{
    public List<Flight> GetFlights();
    public Flight? GetFlight(int id);
    public void AddFlight(Flight flight);
}