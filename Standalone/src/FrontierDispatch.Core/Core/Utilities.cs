using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public static class Utilities
    {
        public static Dictionary<int, List<Connection>> CreateConnectionDictionary(Dictionary<int, Location> locations)
        { 
            var c = new Dictionary<int, List<Connection>>(locations.Count);
            foreach (var (key, _) in locations)
            {
                c.Add(key, new List<Connection>());
            }

            return c;
        }

        public static void AddConnection(this Dictionary<int, List<Connection>> c, int from, int to, int cost)
        {
            c[from].Add(new Connection(to, cost));
        }
    }
}