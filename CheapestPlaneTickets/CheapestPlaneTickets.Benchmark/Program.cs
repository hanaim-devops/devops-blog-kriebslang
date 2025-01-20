using BenchmarkDotNet.Running;
using CheapestPlaneTickets.Benchmark;

BenchmarkRunner.Run<BenchmarkSmallDatabase>();

BenchmarkRunner.Run<BenchmarkLargeDatabase>();