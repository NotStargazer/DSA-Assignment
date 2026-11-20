using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    /// <summary>
    /// Creates deterministic grid worlds for assignment testing,
    /// performance measurement, and scaling experiments.
    ///
    /// The same dimensions always produce the same locations, connections,
    /// and travel costs. This makes before/after measurements easier to compare.
    /// </summary>
    public static class WorldDatasetFactory
    {
        /// <summary>
        /// Creates a 5 x 5 grid world.
        /// Useful for small correctness tests and manual inspection.
        /// </summary>
        public static WorldData CreateSmall() => CreateGrid(5, 5);

        /// <summary>
        /// Creates a 50 x 50 grid world.
        /// Used by the supplied baseline workload.
        /// </summary>
        public static WorldData CreateMedium() => CreateGrid(50, 50);

        /// <summary>
        /// Creates a 200 x 200 grid world for larger scaling experiments.
        /// </summary>
        public static WorldData CreateLarge() => CreateGrid(200, 200);

        /// <summary>
        /// Creates a rectangular grid of locations.
        ///
        /// Horizontally and vertically adjacent locations are connected in
        /// both directions. Travel costs are deterministic rather than random.
        /// </summary>
        public static WorldData CreateGrid(int width, int height)
        {
            var worldSize = width * height;
            var locations = new Dictionary<int, Location>(worldSize);
            // Locations use a stable row-major ID, i.e. the first row is 0..width-1, the second row is width..2*width-1, etc.
            // id = row * width + column.
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var id = y * width + x;
                    locations.Add(id, new Location($"Outpost {id}", new WorldPosition(x, y), "Outpost"));
                }
            }

            var connections = Utilities.CreateConnectionDictionary(locations);
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var id = y * width + x;
                    if (x + 1 < width) AddBidirectional(connections, id, id + 1, Cost(id, id + 1));
                    if (y + 1 < height) AddBidirectional(connections, id, id + width, Cost(id, id + width));
                }
            }

            return new WorldData(locations, connections);
        }

        /// <summary>
        /// Produces a deterministic positive travel cost for an edge.
        /// </summary>
        private static int Cost(int a, int b) => 1 + ((a * 31 + b * 17) % 9);

        /// <summary>
        /// Represents an undirected road as two directed connections.
        /// </summary>
        private static void AddBidirectional(Dictionary<int, List<Connection>> connections, int a, int b, int cost)
        {
            connections.AddConnection(a, b, cost);
            connections.AddConnection(b, a, cost);
        }
    }
}
