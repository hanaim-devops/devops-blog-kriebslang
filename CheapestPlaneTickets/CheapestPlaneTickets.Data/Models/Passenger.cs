namespace CheapestPlaneTickets.Data.Models;

public class Passenger
{
    public int Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public List<Ticket> Tickets { get; set; }
}