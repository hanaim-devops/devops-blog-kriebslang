using BenchmarkDotNet.Attributes;
using CheapestPlaneTickets.Benchmark.Seeders;
using CheapestPlaneTickets.Data;
using CheapestPlaneTickets.Data.Repositories.Dapper;
using CheapestPlaneTickets.Data.Repositories.EntityFramework;
using Microsoft.Data.SqlClient;

namespace CheapestPlaneTickets.Benchmark;

public class BenchmarkSmallDatabase
{
    private CheapestPlaneTicketsDbContext _context;
    
    [GlobalSetup]
    public void Setup()
    {
        _context = new CheapestPlaneTicketsDbContext();
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        SmallDatabaseSeeder.Seed(_context);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
    
    [Benchmark]
    public void GetFlightsWithEF()
    {
        var context = new CheapestPlaneTicketsDbContext();
        var efRepo = new EFFlightRepository(context);
        efRepo.GetFlights();
    }
    
    [Benchmark]
    public void GetFlightsWithEFRaw()
    {
        var context = new CheapestPlaneTicketsDbContext();
        var efRawRepo = new EFRawFlightRepository(context);
        efRawRepo.GetFlights();
    }
    
    [Benchmark]
    public void GetFlightsWithDapper()
    {
        var dapperRepo = new DapperFlightRepository();
        dapperRepo.GetFlights();
    }
}