using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public readonly struct FrontierEntry : IComparable
    {
        public FrontierEntry(int id, int gCost)
        {
            Id = id;
            GCost = gCost;
        }

        public int Id { get; }
        public int GCost { get; }

        public int CompareTo(object obj)
        {
            if (obj is FrontierEntry fe)
            {
                if (Id > fe.Id)
                {
                    return 1;
                }

                if (Id < fe.Id)
                {
                    return -1;
                }
            }

            return 0;
        }
    }

    public sealed class AStarRouteFinder
    {
        public delegate int Heuristic(WorldPosition a, WorldPosition b);
        
        private readonly WorldData _world;
        private readonly MinPriorityQueue<FrontierEntry> _frontier;
        private readonly Dictionary<int, int> _bestCost;
        private readonly Dictionary<int, int> _cameFrom;
        private readonly Heuristic _heuristic;
        
        public AStarRouteFinder(WorldData world, Heuristic heuristicFunction)
        {
            _heuristic = heuristicFunction ?? throw new ArgumentNullException(nameof(heuristicFunction));
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _frontier = new MinPriorityQueue<FrontierEntry>(_world.ConnectionCount);
            _bestCost = new Dictionary<int, int>(_world.ConnectionCount);
            _cameFrom = new Dictionary<int, int>(_world.ConnectionCount);
        }
        
        public RouteResult FindRoute(int startLocationId, int goalLocationId, HashSet<int>? skipNodes = null)
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
            var relaxedNodes = 0;
            _frontier.Clear();
            _bestCost.Clear();
            _cameFrom.Clear();
            _bestCost[startLocationId] = 0;
            _frontier.Enqueue(new FrontierEntry(startLocationId, 0), 0);
            var goalWorldPos = _world.GetWorldPos(goalLocationId);

            while (_frontier.Count > 0)
            {
                var next = _frontier.Dequeue();
                var currentId = next.Id;
                if (next.GCost > _bestCost[currentId])
                {
                    relaxedNodes++;
                    continue;
                }
                
                expandedNodes++;
                
                if (currentId == goalLocationId)
                {
                    var route = ReconstructRoute(_cameFrom, startLocationId, goalLocationId);
                    return RouteResult.SuccessAStar(route, expandedNodes, relaxedNodes, _bestCost[goalLocationId]);
                }
                
                var g = _bestCost[currentId];
                foreach (var connection in _world.GetOutgoingConnections(currentId))
                {
                    var to = connection.ToLocationId;
                    var gCost = g + connection.TravelCost;
                    var hasVal = _bestCost.TryGetValue(to, out var known);
                    if (!hasVal || gCost < known)
                    {
                        if (skipNodes != null && skipNodes.Contains(to))
                        {
                            continue;
                        }
                        
                        _bestCost[to] = gCost;
                        _cameFrom[to] = currentId;
                        var fCost = gCost + _heuristic(_world.GetWorldPos(to), goalWorldPos);
                        _frontier.Enqueue(new FrontierEntry(to, gCost), fCost);
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
