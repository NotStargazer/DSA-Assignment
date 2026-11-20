using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class WorldData
    {
        private readonly Dictionary<int, Location> _locations;
        private readonly Dictionary<int, List<Connection>> _connections;

        public WorldData(Dictionary<int, Location> locations, Dictionary<int, List<Connection>> connections)
        {
            _locations = locations ?? throw new ArgumentNullException(nameof(locations));
            _connections = connections ?? throw new ArgumentNullException(nameof(connections));
            foreach (var (_, list) in connections)
            {
                ConnectionCount += list.Count;
            }
        }

        public IReadOnlyDictionary<int, Location> Locations => _locations;
        public IReadOnlyList<Connection> GetOutgoingConnections(int fromLocationId) => _connections[fromLocationId];
        public bool HasLocation(int locationId) => _locations.ContainsKey(locationId);
        public int ConnectionCount { get; }

        public WorldPosition GetWorldPos(int locationId) => _locations[locationId].Position;
    }
}
