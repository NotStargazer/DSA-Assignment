using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 4.

    /// <summary>
    /// Finds minimum-total-cost routes through a Frontier Dispatch world
    /// using Dijkstra's shortest-path algorithm.
    ///
    /// This class is intentionally incomplete in the assignment starter.
    /// It should eventually use the student's prepared world representation
    /// and MinPriorityQueue implementation.
    /// </summary>
    public sealed class AStarRouteFinder
    {
        private readonly WorldData _world;

        /// <summary>
        /// Constructor. Creates a route finder operating on the supplied world.
        /// </summary>
        public AStarRouteFinder(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        /// <summary>
        /// Finds a minimum-total-travel-cost route from the start location
        /// to the goal location.
        ///
        /// A successful RouteResult should contain the route as location IDs,
        /// including both the start and goal locations.
        /// </summary>
        public RouteResult FindRoute(int startLocationId, int goalLocationId)
        {
            if (!_world.HasLocation(startLocationId) || !_world.HasLocation(goalLocationId))
            {
                return RouteResult.NotFound(0);
            }

            if (startLocationId == goalLocationId)
            {
                return RouteResult.Success(new[] { startLocationId }, 0, 0);
            }

            var expandedNodes = 0;
            var frontier = new HashedPriorityMinQueue<int>();
            frontier.Enqueue(startLocationId, 0);
            var bestCost = new Dictionary<int, int>(_world.ConnectionCount){{startLocationId, 0}};
            var cameFrom = new Dictionary<int, int>(_world.ConnectionCount);

            while (frontier.Count > 0)
            {
                var currentId = frontier.Dequeue();
                expandedNodes++;
                if (currentId == goalLocationId)
                {
                    var route = ReconstructRoute(cameFrom, startLocationId, goalLocationId);
                    return RouteResult.Success(route, expandedNodes, bestCost[goalLocationId]);
                }

                foreach (var connection in _world.GetOutgoingConnections(currentId))
                {
                    var gCost = bestCost[currentId] + connection.TravelCost;
                    if (bestCost.TryAdd(connection.ToLocationId, gCost) || gCost < bestCost[connection.ToLocationId])
                    {
                        bestCost[connection.ToLocationId] = gCost;
                        cameFrom[connection.ToLocationId] = connection.FromLocationId;
                        var fCost = gCost + _world.DistanceBetween(connection.ToLocationId, goalLocationId);
                        if (frontier.ContainsKey(connection.ToLocationId))
                        {
                            frontier[connection.ToLocationId] = fCost;
                        }
                        else
                        {
                            frontier.Enqueue(connection.ToLocationId, fCost);
                        }
                    }
                }
                
            }

            return RouteResult.NotFound(expandedNodes);
        }

        private IReadOnlyList<int> ReconstructRoute(Dictionary<int, int> cameFrom, int start, int goal)
        {
            var route = new List<int> { goal };
            for (var id = cameFrom[goal]; id != start; id = cameFrom[id])
            {
                route.Add(id);
            }
            route.Add(start);
            route.Reverse();
            return route;
        }
    }
}
