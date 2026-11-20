using System;

namespace FrontierDispatch.Core
{
    public static class Heuristics
    {
        public static int ManhattenDistance(WorldPosition a, WorldPosition b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
        
        public static int EuclideanDistance(WorldPosition a, WorldPosition b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return (int)MathF.Sqrt(dx*dx + dy*dy);
        }
    }
}