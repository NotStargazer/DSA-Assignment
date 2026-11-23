using System;
using System.Collections.Generic;

//Full credit to Alex for supplying this full sample world factory.
namespace FrontierDispatch.Core
{
    public static class SampleWorldFactory
    {
        public static WorldData CreateSmall()
        {
            var locations = new Dictionary<int, Location>
            {
                { 101, new Location("Harbor", new WorldPosition(-4, -1), "Port") },
                { 205, new Location("Relay", new WorldPosition(-1, 2), "Relay") },
                { 310, new Location("Crossroads", new WorldPosition(1, 0), "Junction") },
                { 512, new Location("Old Mine", new WorldPosition(4, 2), "Industry") },
                { 734, new Location("North Gate", new WorldPosition(3, 5), "Gate") },
                { 901, new Location("Farm", new WorldPosition(-3, 5), "Farm") },
                { 666, new Location("Hell", new WorldPosition(-4, 1), "Gate") },
                { 999, new Location("Persephone's Garden", new WorldPosition(-4, 2), "Farm") },
            };

            var connections = Utilities.CreateConnectionDictionary(locations);
            
            connections.AddConnection(101, 205, 4); connections.AddConnection(205, 101, 4);
            connections.AddConnection(101, 310, 7); connections.AddConnection(310, 101, 7);
            connections.AddConnection(205, 310, 3); connections.AddConnection(310, 205, 3);
            connections.AddConnection(205, 901, 5); connections.AddConnection(901, 205, 5);
            connections.AddConnection(310, 512, 4); connections.AddConnection(512, 310, 4);
            connections.AddConnection(310, 734, 8); connections.AddConnection(734, 310, 8);
            connections.AddConnection(512, 734, 3); connections.AddConnection(734, 512, 3);
            connections.AddConnection(901, 734, 7); connections.AddConnection(734, 901, 7);
            connections.AddConnection(666, 999, 0); connections.AddConnection(999, 666, 0);
            
            var world = new WorldData(locations, connections);
            return world;
        }
        
        public static WorldData CreateMedium()
        {
            var locations = new Dictionary<int, Location>
            {
                { 101, new Location("Harbor", new WorldPosition(-18, -14), "Port") },
                { 102, new Location("Lighthouse", new WorldPosition(-20, -6), "Port") },
                { 120, new Location("Salt Flats", new WorldPosition(-20, -18), "Wilderness") },
                { 130, new Location("Ruins", new WorldPosition(-10, -20), "Wilderness") },
                { 205, new Location("Relay", new WorldPosition(-10, -8), "Relay") },
                { 210, new Location("Fishing Camp", new WorldPosition(-14, 2), "Camp") },
                { 310, new Location("Crossroads", new WorldPosition(-2, -4), "Junction") },
                { 315, new Location("Trading Post", new WorldPosition(-6, 6), "Market") },
                { 320, new Location("Sawmill", new WorldPosition(4, -12), "Industry") },
                { 410, new Location("Ferry Landing", new WorldPosition(8, -4), "Port") },
                { 415, new Location("Watchtower", new WorldPosition(0, 8), "Outpost") },
                { 512, new Location("Old Mine", new WorldPosition(14, 4), "Industry") },
                { 520, new Location("Quarry", new WorldPosition(18, -8), "Industry") },
                { 601, new Location("Ranger Station", new WorldPosition(-8, 14), "Outpost") },
                { 610, new Location("Chapel", new WorldPosition(6, 14), "Settlement") },
                { 620, new Location("Smelter", new WorldPosition(12, 10), "Industry") },
                { 901, new Location("Farm", new WorldPosition(-16, 12), "Farm") },
                { 905, new Location("Orchard", new WorldPosition(-12, 18), "Farm") },
                { 910, new Location("Pasture", new WorldPosition(2, 20), "Farm") },
                { 734, new Location("North Gate", new WorldPosition(16, 18), "Gate") },
            };

            var connections = Utilities.CreateConnectionDictionary(locations);

            Road(101, 102);
            Road(101, 205);
            Road(101, 120);
            Road(120, 130);
            Road(130, 205);
            Road(130, 320);
            Road(102, 210);
            Road(205, 210);
            Road(205, 310);
            Road(205, 320);
            Road(210, 315);
            Road(210, 901);
            Road(310, 315);
            Road(310, 410);
            Road(310, 415, extraCost: 3);  // river ford
            Road(320, 410);
            Road(320, 520);
            Road(410, 512);
            Road(410, 520);
            Road(415, 315);
            Road(415, 610);
            Road(315, 601);
            Road(512, 620);
            Road(512, 520);
            Road(512, 734, extraCost: 10); // mountain pass
            Road(620, 734);
            Road(610, 620);
            Road(610, 910);
            Road(910, 734);
            Road(601, 901);
            Road(601, 905);
            Road(905, 910);
            Road(901, 905);

            var world = new WorldData(locations, connections);
            
            return world;

            void Road(int a, int b, int extraCost = 0) =>
                AddRoad(connections, a, b, locations[a], locations[b], extraCost);
        }

        public static WorldData CreateLarge()
        {
            const int width = 50;
            const int height = 50;
            const int spacing = 10;

            var locations = new Dictionary<int, Location>(width * height);
            var connections = new Dictionary<int, List<Connection>>(width * height);
            var grid = new int[width, height];

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    // Fixed offset between -3 and +3 so the outposts don't sit on a perfect grid.
                    var offsetX = (x * 73 + y * 151) % 7 - 3;
                    var offsetY = (x * 37 + y * 89) % 7 - 3;
                    var position = new WorldPosition(x * spacing + offsetX, y * spacing + offsetY);

                    var id = 0;
                    Location location;
                    switch (x)
                    {
                        case 2 when y == 2:
                            id = 101;
                            location = new Location("Harbor", position, "Port");
                            break;
                        case width - 3 when y == height - 3:
                            id = 734;
                            location = new Location("North Gate", position, "Gate");
                            break;
                        default:
                        {
                            id = 1000 + y * height + x;
                            location = new Location($"Outpost {id}", position, "Outpost");
                            break;
                        }
                    }

                    locations.Add(id, location);
                    connections.Add(id, new List<Connection>());
                    grid[x, y] = id;
                }
            }

            // Right, down, down-right and down-left. AddRoad adds both directions,
            // so this covers all eight neighbours without adding any road twice.
            int[] stepX = { 1, 0, 1, -1 };
            int[] stepY = { 0, 1, 1, 1 };

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    for (var i = 0; i < stepX.Length; ++i)
                    {
                        int neighbourX = x + stepX[i];
                        int neighbourY = y + stepY[i];

                        if (neighbourX < 0 || neighbourX >= width || neighbourY >= height)
                        {
                            continue;
                        }

                        int roughGround = (x * 31 + y * 17 + stepX[i] * 7 + stepY[i] * 13) % 3;
                        var id = grid[x, y];
                        var neighborId = grid[neighbourX, neighbourY];
                        AddRoad(connections, id, neighborId, locations[id], locations[neighborId], roughGround);
                    }
                }
            }

            return new WorldData(locations, connections);
        }

        // Adds a two-way road. The cost is the straight-line length rounded up, plus any extra cost.
        private static void AddRoad(Dictionary<int, List<Connection>> world, int from, int to, Location a, Location b, int extraCost)
        {
            var dx = a.Position.X - b.Position.X;
            var dy = a.Position.Y - b.Position.Y;
            var cost = (int)Math.Ceiling(Math.Sqrt(dx * dx + dy * dy)) + extraCost;

            world.AddConnection(from, to, cost);
            world.AddConnection(to, from, cost);
        }
    }
}