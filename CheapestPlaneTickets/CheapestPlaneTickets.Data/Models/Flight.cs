namespace CheapestPlaneTickets.Data.Models;

public class Flight
{
    public int Id { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Price { get; set; }
    public Plane Plane { get; set; }
    public int PlaneId { get; set; }
    public List<Ticket> Tickets { get; set; }
}