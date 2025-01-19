using CheapestPlaneTickets.Data.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CheapestPlaneTickets.Data.Repositories.Dapper;

public class DapperFlightRepository() : IFlightRepository
{
    
    private readonly string _connectionString = "Server=localhost,1433;User Id=sa;Password=d8bc90AD1d4e5e5cC98352d2670bb7ea!;Database=CheapestPlaneTickets;TrustServerCertificate=True;";
    
    public List<Flight> GetFlights()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = "SELECT f.Id, f.DepartureTime, f.ArrivalTime, f.Price, f.PlaneId, p.Id, p.Model FROM Flights f INNER JOIN Planes p ON PlaneId = p.Id";

        return connection.Query<Flight, Plane, Flight>(sql, (flight, plane) =>
        {
            flight.Plane = plane;
            return flight;
        },
        splitOn: "PlaneId")
        .ToList();
    }

    public Flight? GetFlight(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = $"""
                   
                           SELECT 
                               f.Id, 
                               f.DepartureTime, 
                               f.ArrivalTime, 
                               f.Price, 
                               f.PlaneId, 
                               p.Id, 
                               p.Model 
                           FROM Flights f
                           INNER JOIN Planes p ON f.PlaneId = p.Id
                           WHERE f.Id = @Id
                   """;

        return connection.Query<Flight, Plane, Flight>(sql, (flight, plane) =>
                {
                    flight.Plane = plane;
                    return flight;
                },
                splitOn: "PlaneId",
                param: new { Id = id })
            .FirstOrDefault();
    }
    
    public void AddFlight(Flight flight)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        var sql = @"
        INSERT INTO Flights (Origin, Destination, DepartureTime, ArrivalTime, Price, PlaneId)
        VALUES (@Origin, @Destination, @DepartureTime, @ArrivalTime, @Price, @PlaneId)";

        connection.Execute(sql, flight);
    }
}