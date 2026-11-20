using System;
using System.Collections.Generic;
using NUnit.Framework;
using FrontierDispatch.Core;

namespace FrontierDispatch.Tests
{
    /// <summary>
    /// Small contract examples for later assignment phases.
    ///
    /// These are not an exhaustive assignment test suite.
    /// Students are expected to enable the relevant tests as they reach each
    /// phase and add meaningful tests of their own, including edge cases.
    /// </summary>
    public sealed class AssignmentStarterTests
    {
        // These tests document later assignment contracts without making the
        // untouched baseline red. Remove Ignore when you reach the named phase.

        [Test]
        public void PriorityQueue_EnqueueAndDequeue_ReturnsLowestPriorityFirst()
        {
            var queue = new MinPriorityQueue<string>();
            queue.Enqueue("expensive", 9);
            queue.Enqueue("cheap", 2);
            queue.Enqueue("medium", 5);
            Assert.That(queue.Dequeue(), Is.EqualTo("cheap"));
        }
        
        [Test]
        public void PriorityQueue_EnqueueAndDequeue_ReturnsCorrectOrder()
        {
            var queue = new MinPriorityQueue<string>();
            queue.Enqueue("expensive", 9);
            queue.Enqueue("cheap", 2);
            queue.Enqueue("medium", 5);
            queue.Enqueue("very expensive", 20);
            queue.Enqueue("somewhat cheap", 3);

            var dequeued = new string[queue.Count];
            for (var i = 0; i < dequeued.Length; i++)
            {
                dequeued[i] = queue.Dequeue();
            }
            
            Assert.That(dequeued, 
                Is.EqualTo(
                    ["cheap", "somewhat cheap", "medium", "expensive", "very expensive"]));
        }
        
        [Test]
        public void PriorityQueue_EnqueueAndDequeue_HandlesSingleElement()
        {
            var queue = new MinPriorityQueue<int>();
            queue.Enqueue(1, 9);
            
            Assert.That(queue.Dequeue(), Is.EqualTo(1));
        }
        
        [Test]
        public void PriorityQueue_EnqueueAndDequeue_ThrowsWhenEmpty()
        {
            var queue = new MinPriorityQueue<string>();
            Assert.Throws<InvalidOperationException>(() => { queue.Dequeue(); });
            Assert.Throws<InvalidOperationException>(() => { queue.Peek(); });
        }

        [Test]
        public void AStar_StartEqualsGoal_ReturnsZeroCostRoute()
        {
            var finder = new AStarRouteFinder(WorldDatasetFactory.CreateSmall(), Heuristics.EuclideanDistance);
            RouteResult result = finder.FindRoute(0, 0);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(0));
            Assert.That(result.LocationIds, Is.EqualTo([0]));
        }

        [Test]
        public void AStar_FindsGoal_CorrectPathAndCost()
        {
            var finder = new AStarRouteFinder(SampleWorldFactory.CreateSmall(), Heuristics.EuclideanDistance);
            RouteResult result = finder.FindRoute(101, 734);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(14));
            Assert.That(result.LocationIds, Is.EqualTo([101, 310, 512, 734]));
        }
        
        [Test]
        public void AStar_FindsGoal_BadHeuristicReturnsSameAsDijkstra()
        {
            var world = SampleWorldFactory.CreateSmall();
            var dijkstra = new DijkstraRouteFinder(world);
            var astar = new AStarRouteFinder(world, (_, _) => 0);
            var dijkstraResult = dijkstra.FindRoute(101, 734);
            var astarResult = astar.FindRoute(101, 734);
            Assert.That(dijkstraResult.Found, Is.EqualTo(astarResult.Found));
            Assert.That(dijkstraResult.ExpandedCount, Is.EqualTo(astarResult.ExpandedCount));
            Assert.That(dijkstraResult.TotalCost, Is.EqualTo(astarResult.TotalCost));
        }
        
        [Test]
        public void AStar_DijkstraComparison_MatchingPathWithLessExpandedNodes()
        {
            var random = new Random(1337);
            for (var i = 10; i <= 100; i++)
            {
                var world = WorldDatasetFactory.CreateGrid(i, i);
                var dijkstra = new DijkstraRouteFinder(world);
                var astar = new AStarRouteFinder(world, Heuristics.ManhattenDistance);
                var s = random.Next(0, world.Locations.Count);
                var g = random.Next(0, world.Locations.Count);
                var dijkstraResult = dijkstra.FindRoute(s, g);
                var astarResult = astar.FindRoute(s, g);
                Assert.That(dijkstraResult.ExpandedCount, Is.GreaterThan(astarResult.ExpandedCount));
                Assert.That(dijkstraResult.TotalCost, Is.EqualTo(astarResult.TotalCost));
            }
        }
        
        [Test]
        public void AStar_BlocksNodes_DoesNotTraverseBlockedNodes()
        {
            var finder = new AStarRouteFinder(SampleWorldFactory.CreateSmall(), Heuristics.EuclideanDistance);
            var result = finder.FindRoute(101, 734, [512]);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(15));
            Assert.That(result.LocationIds, Does.Not.Contain(512));
        }

        [Test]
        public void Reachability_ZeroBudget_IncludesStartAtZeroCost()
        {
            var query = new ReachabilityQuery(WorldDatasetFactory.CreateSmall());
            var result = query.FindReachable(0, 0);
            Assert.That(result.ContainsKey(0), Is.True);
            Assert.That(result[0], Is.EqualTo(0));
        }
        
        [Test]
        public void Reachability_InfiniteBudget_FindsEverything()
        {
            var query = new ReachabilityQuery(SampleWorldFactory.CreateSmall());
            var result = query.FindReachable(101, int.MaxValue);
            Assert.That(result.Keys, Is.EqualTo([
                101,
                205,
                310,
                901,
                512,
                734
            ]));
        }
        
        [Test]
        public void Reachability_LimitedBudget_FindsNearby()
        {
            var query = new ReachabilityQuery(SampleWorldFactory.CreateSmall());
            var result = query.FindReachable(205, 7);
            Assert.That(result.Keys, Is.EqualTo([
                205,
                101,
                310,
                901,
                512
            ]));
        }
    }
}
