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
    public sealed class DijkstraRouteFinder
    {
        private readonly WorldData _world;
        private readonly MinPriorityQueue<int> _frontier;
        private readonly Dictionary<int, int> _bestCost;
        private readonly Dictionary<int, int> _cameFrom;

        /// <summary>
        /// Constructor. Creates a route finder operating on the supplied world.
        /// </summary>
        public DijkstraRouteFinder(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _frontier = new MinPriorityQueue<int>();
            _bestCost = new Dictionary<int, int>(_world.ConnectionCount);
            _cameFrom = new Dictionary<int, int>(_world.ConnectionCount);
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
            _frontier.Clear();
            _bestCost.Clear();
            _cameFrom.Clear();
            _bestCost[startLocationId] = 0;
            _frontier.Enqueue(startLocationId, 0);

            while (_frontier.Count > 0)
            {
                var currentId = _frontier.Dequeue();
                expandedNodes++;
                if (currentId == goalLocationId)
                {
                    var route = ReconstructRoute(_cameFrom, startLocationId, goalLocationId);
                    return RouteResult.Success(route, expandedNodes, _bestCost[goalLocationId]);
                }

                var g = _bestCost[currentId];
                foreach (var connection in _world.GetOutgoingConnections(currentId))
                {
                    var cost = g + connection.TravelCost;
                    if (!_bestCost.TryGetValue(connection.ToLocationId, out var known) || cost < known)
                    {
                        _bestCost[connection.ToLocationId] = cost;
                        _cameFrom[connection.ToLocationId] = connection.FromLocationId;
                        _frontier.Enqueue(connection.ToLocationId, cost);
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
