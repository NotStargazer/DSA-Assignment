using System;
using System.Diagnostics;
using FrontierDispatch.Core;

if (args.Length > 0 && args[0] == "--baseline")
{
    RunBaseline();
    return;
}

if (args.Length > 0 && args[0] == "--pf")
{
    RunDijkstra();
    RunAStar();
    return;
}

if (args.Length > 0 && args[0] == "--pfgrid")
{
    RunDijkstraGrid();
    RunAStarGrid();
    return;
}

if (args.Length > 0 && args[0] == "--world")
{
    WorldCreationMetrics();
    return;
}

// Small sanity-check application.
// This is separate from the larger performance workload below.
WorldData world = WorldDatasetFactory.CreateSmall();
var queries = new WorldQueries(world);

Console.WriteLine("Frontier Dispatch - assignment starter");
Console.WriteLine($"Small demo locations: {world.Locations.Count}");
Console.WriteLine($"Find 734: {queries.FindLocationById(734)?.Name ?? "missing"}");
Console.WriteLine();
Console.WriteLine("Run with --baseline to record the supplied baseline workload.");

static void RunBaseline()
{
    // Build the dataset before timing the repeated runtime queries.
    // This baseline therefore measures query work, not world construction.
    WorldData world = WorldDatasetFactory.CreateMedium();
    var queries = new WorldQueries(world);

    // The checksum provides a stable sanity check that the workload still
    // produces equivalent results after implementation changes.
    int checksum = 0;

    const int iterations = 5000;

    var sw = Stopwatch.StartNew();

    for (int i = 0; i < iterations; ++i)
    {
        // Deterministically visit many different location IDs rather than
        // repeatedly querying the same location.
        int id = (i * 997) % world.Locations.Count;

        Location? location = queries.FindLocationById(id);
        checksum += location?.Id ?? 0;
        checksum += queries.FindOutgoingConnections(id)?.Count ?? 0;
    }

    sw.Stop();

    Console.WriteLine("Baseline world-query workload");
    Console.WriteLine($"Locations: {world.Locations.Count}");
    Console.WriteLine($"Connections: {world.ConnectionCount}");
    Console.WriteLine($"Iterations: {iterations}");
    Console.WriteLine($"Elapsed: {sw.Elapsed.TotalMilliseconds} ms");
    Console.WriteLine($"Checksum: {checksum}");
    Console.WriteLine("Absolute time varies by machine. Keep this result for before/after comparison.");
}

static void WorldCreationMetrics()
{
    var sw = Stopwatch.StartNew();
    var bytesPre = GC.GetAllocatedBytesForCurrentThread();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    sw.Stop();
    var bytesPost = GC.GetAllocatedBytesForCurrentThread();
    
    Console.WriteLine("Large World Creation Metrics");
    Console.WriteLine($"Locations: {largeWorld.Locations.Count}");
    Console.WriteLine($"Connections: {largeWorld.ConnectionCount}");
    Console.WriteLine($"Memory Allocation (Bytes): {bytesPost - bytesPre:00,000,000} bytes");
    Console.WriteLine($"Memory Allocation (Mb): {(bytesPost - bytesPre) / 1_000_000} Mb");
    Console.WriteLine($"Creation Time: {sw.Elapsed.TotalMilliseconds} ms");
}

static void RunDijkstra()
{
    var smallWorld = SampleWorldFactory.CreateSmall();
    var mediumWorld = SampleWorldFactory.CreateMedium();
    var largeWorld = SampleWorldFactory.CreateLarge();
    
    const int iterations = 5000;
    var sw = Stopwatch.StartNew();
    var smallResult = IterateWorldDataDijkstra(smallWorld, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumResult = IterateWorldDataDijkstra(mediumWorld, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeResult = IterateWorldDataDijkstra(largeWorld, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("Dijkstra performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Avg. Expanded  | {smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static void RunAStar()
{
    var smallWorld = SampleWorldFactory.CreateSmall();
    var mediumWorld = SampleWorldFactory.CreateMedium();
    var largeWorld = SampleWorldFactory.CreateLarge();
    
    const int iterations = 5000;
    var sw = Stopwatch.StartNew();
    var smallResult = IterateWorldDataAStar(smallWorld, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumResult = IterateWorldDataAStar(mediumWorld, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeResult = IterateWorldDataAStar(largeWorld, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("A-Star performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Avg. Expanded  | {smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Avg. Relaxed   | {smallResult.AverageRelaxedNodes,-11} | {mediumResult.AverageRelaxedNodes,-12} | {largeResult.AverageRelaxedNodes,-11} |");
    Console.WriteLine($"Avg. Checked   | {smallResult.AverageRelaxedNodes + smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageRelaxedNodes + mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageRelaxedNodes + largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static void RunDijkstraGrid()
{
    var smallWorld = WorldDatasetFactory.CreateSmall();
    var mediumWorld = WorldDatasetFactory.CreateMedium();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallResult = IterateWorldDataDijkstraRandom(smallWorld, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumResult = IterateWorldDataDijkstraRandom(mediumWorld, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeResult = IterateWorldDataDijkstraRandom(largeWorld, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("Dijkstra performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Avg. Expanded  | {smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static void RunAStarGrid()
{
    var smallWorld = WorldDatasetFactory.CreateSmall();
    var mediumWorld = WorldDatasetFactory.CreateMedium();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallResult = IterateWorldDataAStarRandom(smallWorld, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumResult = IterateWorldDataAStarRandom(mediumWorld, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeResult = IterateWorldDataAStarRandom(largeWorld, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("A-Star performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Avg. Expanded  | {smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Avg. Relaxed   | {smallResult.AverageRelaxedNodes,-11} | {mediumResult.AverageRelaxedNodes,-12} | {largeResult.AverageRelaxedNodes,-11} |");
    Console.WriteLine($"Avg. Checked   | {smallResult.AverageRelaxedNodes + smallResult.AverageExpandedNodes,-11} | {mediumResult.AverageRelaxedNodes + mediumResult.AverageExpandedNodes,-12} | {largeResult.AverageRelaxedNodes + largeResult.AverageExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static PathfindingMetrics IterateWorldDataDijkstra(WorldData world, int iterations)
{
    var pathFinder = new DijkstraRouteFinder(world);
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var result = pathFinder.FindRoute(101, 734);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost;
            totalExpandedNodes += result.ExpandedCount;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(iterations, totalExpandedNodes, totalCost, checksum);
}

static PathfindingMetrics IterateWorldDataAStar(WorldData world, int iterations)
{
    var pathFinder = new AStarRouteFinder(world, Heuristics.EuclideanDistance);
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalRelaxedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var result = pathFinder.FindRoute(101, 734);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost + result.RelaxedNodes;
            totalExpandedNodes += result.ExpandedCount;
            totalRelaxedNodes += result.RelaxedNodes;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(iterations, totalExpandedNodes, totalCost, checksum, totalRelaxedNodes);
}

static PathfindingMetrics IterateWorldDataDijkstraRandom(WorldData world, int iterations)
{
    var rand = new Random(1337);
    var pathFinder = new DijkstraRouteFinder(world);
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var a = rand.Next(0, world.Locations.Count);
        var b = rand.Next(0, world.Locations.Count);
        var result = pathFinder.FindRoute(a, b);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost;
            totalExpandedNodes += result.ExpandedCount;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(iterations, totalExpandedNodes, totalCost, checksum);
}

static PathfindingMetrics IterateWorldDataAStarRandom(WorldData world, int iterations)
{
    var rand = new Random(1337);
    var pathFinder = new AStarRouteFinder(world, Heuristics.ManhattenDistance);
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalRelaxedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var a = rand.Next(0, world.Locations.Count);
        var b = rand.Next(0, world.Locations.Count);
        var result = pathFinder.FindRoute(a, b);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost + result.RelaxedNodes;
            totalExpandedNodes += result.ExpandedCount;
            totalRelaxedNodes += result.RelaxedNodes;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(iterations, totalExpandedNodes, totalCost, checksum, totalRelaxedNodes);
}

readonly struct PathfindingMetrics(int iterations, int totalExpandedNodes, int totalCost, int checkSum, int totalRelaxedNodes = 0)
{
    public int AverageExpandedNodes { get; } = totalExpandedNodes / iterations;
    public int AverageRelaxedNodes { get; } = totalRelaxedNodes / iterations;
    public int TotalCost { get; } = totalCost;
    public int CheckSum { get; } = checkSum;
}