using CheapestPlaneTickets.Data;
using CheapestPlaneTickets.Data.Models;

public static class LargeDatabaseSeeder
{
    public static void Seed(CheapestPlaneTicketsDbContext dbContext)
    {
        var locations = new[] { "Amsterdam", "London", "Paris", "Berlin", "New York", "Tokyo", "Sydney", "Dubai", "Rome", "Madrid" };
        
        var planes = new List<Plane>
        {
            new Plane { Model = "Boeing 737" },
            new Plane { Model = "Airbus A320" },
            new Plane { Model = "Embraer 190" },
            new Plane { Model = "Boeing 747" }
        };
        
        if (!dbContext.Planes.Any())
        {
            dbContext.Planes.AddRange(planes);
            dbContext.SaveChanges();
        }

        var random = new Random();
        var flights = new List<Flight>();

        for (int i = 0; i < 100000; i++)
        {
            string origin, destination;
            do
            {
                origin = locations[random.Next(locations.Length)];
                destination = locations[random.Next(locations.Length)];
            } while (origin == destination);

            var plane = planes[random.Next(planes.Count)];
            
            var departureTime = DateTime.Now.AddDays(random.Next(1, 30)).AddHours(random.Next(24));
            var arrivalTime = departureTime.AddHours(random.Next(1, 12));
            
            var price = Math.Round((decimal)(random.NextDouble() * 500 + 50), 2);
            
            flights.Add(new Flight
            {
                Origin = origin,
                Destination = destination,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Price = price,
                PlaneId = plane.Id
            });
        }
        
        dbContext.Flights.AddRange(flights);
        dbContext.SaveChanges();
    }
}