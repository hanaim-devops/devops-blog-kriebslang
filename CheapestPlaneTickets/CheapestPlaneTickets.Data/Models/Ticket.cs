namespace CheapestPlaneTickets.Data.Models;

public class Ticket
{
    public int Id { get; set; }
    public Flight Flight { get; set; }
    public Passenger Passenger { get; set; }
    public int PassengerId { get; set; }
}