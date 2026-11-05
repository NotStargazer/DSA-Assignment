using System;
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
            var queue = new MinPriorityQueue<int>();
            Assert.Throws<InvalidOperationException>(() => { queue.Dequeue(); });
            Assert.Throws<InvalidOperationException>(() => { queue.Peek(); });
        }

        [Test, Ignore("Phase 4: enable when implementing Dijkstra route finding.")]
        public void Dijkstra_StartEqualsGoal_ReturnsZeroCostRoute()
        {
            var finder = new DijkstraRouteFinder(WorldDatasetFactory.CreateSmall());
            RouteResult result = finder.FindRoute(0, 0);
            Assert.That(result.Found, Is.True);
            Assert.That(result.TotalCost, Is.EqualTo(0));
            Assert.That(result.LocationIds, Is.EqualTo(new[] { 0 }));
        }

        [Test, Ignore("Phase 5: enable when implementing bounded reachability.")]
        public void Reachability_ZeroBudget_IncludesStartAtZeroCost()
        {
            var query = new ReachabilityQuery(WorldDatasetFactory.CreateSmall());
            var result = query.FindReachable(0, 0);
            Assert.That(result.ContainsKey(0), Is.True);
            Assert.That(result[0], Is.EqualTo(0));
        }
    }
}
