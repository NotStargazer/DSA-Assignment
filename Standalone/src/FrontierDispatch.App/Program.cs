using System;
using System.Diagnostics;
using FrontierDispatch.Core;

if (args.Length > 0 && args[0] == "--baseline")
{
    RunBaseline();
    return;
}

if (args.Length > 0 && args[0] == "--dijkstra")
{
    RunDijkstra();
    return;
}

if (args.Length > 0 && args[0] == "--astar")
{
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
    var extremeWorld = WorldDatasetFactory.CreateGrid(1000, 1000);
    
    var checksum = 0;
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallDijkstra = new DijkstraRouteFinder(smallWorld);
    var smallResult = IterateWorldDataDijkstra(smallWorld, smallDijkstra, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumDijkstra = new DijkstraRouteFinder(mediumWorld);
    var mediumResult = IterateWorldDataDijkstra(mediumWorld, mediumDijkstra, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeDijkstra = new DijkstraRouteFinder(largeWorld);
    var largeResult = IterateWorldDataDijkstra(largeWorld, largeDijkstra, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var extremeDijkstra = new DijkstraRouteFinder(extremeWorld);
    var extremeResult = IterateWorldDataDijkstra(extremeWorld, extremeDijkstra, iterations);
    sw.Stop();
    var extremeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("Dijkstra performance metrics");
    Console.WriteLine("---------------------------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum + extremeResult.CheckSum}");
    Console.WriteLine("---------------------------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World | Extreme World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} | {extremeWorld.Locations.Count,-13} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} | {extremeWorld.ConnectionCount,-13} |");
    Console.WriteLine($"Expanded Nodes | {smallResult.TotalExpandedNodes,-11} | {mediumResult.TotalExpandedNodes,-12} | {largeResult.TotalExpandedNodes,-11} | {extremeResult.TotalExpandedNodes,-13} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} | {extremeResult.TotalCost,-13} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms | {extremeElapsed,-10} ms |");
    Console.WriteLine("---------------------------------------------------------------------------");
}

static void RunAStar()
{
    var smallWorld = WorldDatasetFactory.CreateSmall();
    var mediumWorld = WorldDatasetFactory.CreateMedium();
    var largeWorld = WorldDatasetFactory.CreateLarge();
    var extremeWorld = WorldDatasetFactory.CreateGrid(1000, 1000);
    
    var checksum = 0;
    const int iterations = 100;
    var sw = Stopwatch.StartNew();
    var smallAStar = new AStarRouteFinder(smallWorld);
    var smallResult = IterateWorldDataAStar(smallWorld, smallAStar, iterations);
    sw.Stop();
    var smallElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var mediumAStar = new AStarRouteFinder(mediumWorld);
    var mediumResult = IterateWorldDataAStar(mediumWorld, mediumAStar, iterations);
    sw.Stop();
    var mediumElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var largeAStar = new AStarRouteFinder(largeWorld);
    var largeResult = IterateWorldDataAStar(largeWorld, largeAStar, iterations);
    sw.Stop();
    var largeElapsed = sw.Elapsed.TotalMilliseconds;
    sw.Restart();
    var extremeAStar = new AStarRouteFinder(extremeWorld);
    var extremeResult = IterateWorldDataAStar(extremeWorld, extremeAStar, iterations);
    sw.Stop();
    var extremeElapsed = sw.Elapsed.TotalMilliseconds;
    
    Console.WriteLine("A-Star performance metrics");
    Console.WriteLine("---------------------------------------------------------------------------");
    Console.WriteLine($"Iterations Per World: {iterations} | Checksum: {smallResult.CheckSum + mediumResult.CheckSum + largeResult.CheckSum + extremeResult.CheckSum}");
    Console.WriteLine("---------------------------------------------------------------------------");
    Console.WriteLine("               | Small World | Medium World | Large World | Extreme World |");
    Console.WriteLine($"Locations      | {smallWorld.Locations.Count,-11} | {mediumWorld.Locations.Count,-12} | {largeWorld.Locations.Count,-11} | {extremeWorld.Locations.Count,-13} |");
    Console.WriteLine($"Connections    | {smallWorld.ConnectionCount,-11} | {mediumWorld.ConnectionCount,-12} | {largeWorld.ConnectionCount,-11} | {extremeWorld.ConnectionCount,-13} |");
    Console.WriteLine($"Expanded Nodes | {smallResult.TotalExpandedNodes,-11} | {mediumResult.TotalExpandedNodes,-12} | {largeResult.TotalExpandedNodes,-11} | {extremeResult.TotalExpandedNodes,-13} |");
    Console.WriteLine($"Total Cost     | {smallResult.TotalCost,-11} | {mediumResult.TotalCost,-12} | {largeResult.TotalCost,-11} | {extremeResult.TotalCost,-13} |");
    Console.WriteLine($"Elapsed Time   | {smallElapsed,-8} ms | {mediumElapsed,-9} ms | {largeElapsed,-8} ms | {extremeElapsed,-10} ms |");
    Console.WriteLine("---------------------------------------------------------------------------");
}

static PathfindingMetrics IterateWorldDataDijkstra(WorldData world, DijkstraRouteFinder pathFinder, int iterations)
{
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalCost = 0;
    for (var i = 1; i <= iterations; i++)
    {
        var startId = i * 32069 % world.Locations.Count;
        var endId = i * 25679 % world.Locations.Count;
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

static PathfindingMetrics IterateWorldDataAStar(WorldData world, AStarRouteFinder pathFinder, int iterations)
{
    var checksum = 0;
    var totalExpandedNodes = 0;
    var totalCost = 0;
    for (var i = 1; i <= iterations; i++)
    {
        var startId = i * 32069 % world.Locations.Count;
        var endId = i * 25679 % world.Locations.Count;
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

readonly struct PathfindingMetrics(int totalExpandedNodes, int totalCost, int checkSum)
{
    public int TotalExpandedNodes { get; } = totalExpandedNodes;
    public int TotalCost { get; } = totalCost;
    public int CheckSum { get; } = checkSum;
}