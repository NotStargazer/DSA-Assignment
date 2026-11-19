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

static void RunDijkstra()
{
    var smallWorld = WorldDatasetFactory.CreateSmall();
    var mediumWorld = WorldDatasetFactory.CreateMedium();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    var smallRand = new Random(1337);
    var mediumRand = new Random(1337);
    var largeRand = new Random(1337);
    
    var checksum = 0;
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallDijkstra = new DijkstraRouteFinder(smallWorld);
    var smallResult = IterateWorldDataDijkstra(smallRand, smallWorld, smallDijkstra, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumDijkstra = new DijkstraRouteFinder(mediumWorld);
    var mediumResult = IterateWorldDataDijkstra(mediumRand, mediumWorld, mediumDijkstra, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeDijkstra = new DijkstraRouteFinder(largeWorld);
    var largeResult = IterateWorldDataDijkstra(largeRand, largeWorld, largeDijkstra, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("Dijkstra performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Expanded Nodes | {smallResult.TotalExpandedNodes,-11} | {mediumResult.TotalExpandedNodes,-12} | {largeResult.TotalExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static void RunAStar()
{
    var smallWorld = WorldDatasetFactory.CreateSmall();
    var mediumWorld = WorldDatasetFactory.CreateMedium();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    var smallRand = new Random(1337);
    var mediumRand = new Random(1337);
    var largeRand = new Random(1337);
    
    var checksum = 0;
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallAStar = new AStarRouteFinder(smallWorld);
    var smallResult = IterateWorldDataAStar(smallRand, smallWorld, smallAStar, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumAStar = new AStarRouteFinder(mediumWorld);
    var mediumResult = IterateWorldDataAStar(mediumRand, mediumWorld, mediumAStar, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeAStar = new AStarRouteFinder(largeWorld);
    var largeResult = IterateWorldDataAStar(largeRand, largeWorld, largeAStar, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("A-Star performance metrics");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum}");
    Console.WriteLine("-----------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} |");
    Console.WriteLine($"Expanded Nodes | {smallResult.TotalExpandedNodes,-11} | {mediumResult.TotalExpandedNodes,-12} | {largeResult.TotalExpandedNodes,-11} |");
    Console.WriteLine($"Relaxed Nodes  | {smallResult.TotalRelaxedNodes,-11} | {mediumResult.TotalRelaxedNodes,-12} | {largeResult.TotalRelaxedNodes,-11} |");
    Console.WriteLine($"Total Nodes    | {smallResult.TotalRelaxedNodes + smallResult.TotalExpandedNodes,-11} | {mediumResult.TotalRelaxedNodes + mediumResult.TotalExpandedNodes,-12} | {largeResult.TotalRelaxedNodes + largeResult.TotalExpandedNodes,-11} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms |");
    Console.WriteLine("-----------------------------------------------------------");
}

static PathfindingMetrics IterateWorldDataDijkstra(Random rand, WorldData world, DijkstraRouteFinder pathFinder, int iterations)
{
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var startId = rand.Next(0, world.Locations.Count);
        var endId = rand.Next(0, world.Locations.Count);
        var result = pathFinder.FindRoute(startId, endId);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost;
            totalExpandedNodes += result.ExpandedCount;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(totalExpandedNodes, totalCost, checksum);
}

static PathfindingMetrics IterateWorldDataAStar(Random rand, WorldData world, AStarRouteFinder pathFinder, int iterations)
{
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalRelaxedNodes = 0;
    var totalCost = 0;
    for (var i = 0; i < iterations; i++)
    {
        var startId = rand.Next(0, world.Locations.Count);
        var endId = rand.Next(0, world.Locations.Count);
        var result = pathFinder.FindRoute(startId, endId);
        if (result.Found)
        {
            checksum += result.ExpandedCount + result.TotalCost;
            totalExpandedNodes += result.ExpandedCount;
            totalRelaxedNodes += result.RelaxedNodes;
            totalCost += result.TotalCost;
        }
    }
    return new PathfindingMetrics(totalExpandedNodes, totalCost, checksum, totalRelaxedNodes);
}

readonly struct PathfindingMetrics(int totalExpandedNodes, int totalCost, int checkSum, int totalRelaxedNodes = 0)
{
    public int TotalExpandedNodes { get; } = totalExpandedNodes;
    public int TotalRelaxedNodes { get; } = totalRelaxedNodes;
    public int TotalCost { get; } = totalCost;
    public int CheckSum { get; } = checkSum;
}