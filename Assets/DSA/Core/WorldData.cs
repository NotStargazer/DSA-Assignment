using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class WorldData
    {
        private readonly Dictionary<int, Location> _locations;
        private readonly Dictionary<int, List<Connection>> _connections;

        public WorldData(ICollection<Location> locations, ICollection<Connection> connections)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));
            _locations = new Dictionary<int, Location>(locations.Count);
            _connections = new Dictionary<int, List<Connection>>(locations.Count);

            foreach (var location in locations)
            {
                _locations.Add(location.Id, location);
                _connections.Add(location.Id, new List<Connection>());
            }

            ConnectionCount = connections.Count;
            foreach (var connection in connections)
            {
                _connections[connection.FromLocationId].Add(connection);
            }
        }

        public IReadOnlyDictionary<int, Location> Locations => _locations;
        public IReadOnlyList<Connection>? GetOutgoingConnections(int fromLocationId) => _connections.GetValueOrDefault(fromLocationId);
        public int ConnectionCount { get; }

    }
}
