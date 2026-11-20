using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class WorldData
    {
        private readonly Dictionary<int, Location> _locations;
        private readonly Dictionary<int, List<Connection>> _connections;

        public WorldData(ICollection<Location> locations)
        {
            if (locations == null) throw new ArgumentNullException(nameof(locations));
            _locations = new Dictionary<int, Location>(locations.Count);
            _connections = new Dictionary<int, List<Connection>>(locations.Count);

            foreach (var location in locations)
            {
                _locations.Add(location.Id, location);
                _connections.Add(location.Id, new List<Connection>());
            }
        }

        public IReadOnlyDictionary<int, Location> Locations => _locations;
        public IReadOnlyList<Connection> GetOutgoingConnections(int fromLocationId) => _connections[fromLocationId];
        public bool HasLocation(int locationId) => _locations.ContainsKey(locationId);
        public int ConnectionCount { get; private set; }

        public WorldPosition GetWorldPos(int locationId) => _locations[locationId].Position;

        public void AddConnection(int fromId, int toId, int travelCost)
        {
            ConnectionCount++;
            var didAdd = _connections.TryAdd(fromId, new List<Connection> { new (toId, travelCost) });
            if (!didAdd)
            {
                _connections[fromId].Add(new Connection(toId, travelCost));
            }
        }
    }
}
