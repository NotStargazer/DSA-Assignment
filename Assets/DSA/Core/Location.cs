using System;

namespace FrontierDispatch.Core
{
    /// <summary>
    /// A location/node in the Frontier Dispatch world graph.
    /// </summary>
    public sealed class Location
    {
        public Location(string name, WorldPosition position, string type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Type is required.", nameof(type));

            Name = name;
            Position = position;
            Type = type;
        }

        public string Name { get; }
        public WorldPosition Position { get; }
        public string Type { get; }
    }
}
