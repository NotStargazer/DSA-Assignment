using NUnit.Framework;
using FrontierDispatch.Core;

namespace FrontierDispatch.Tests
{
    public sealed class WorldQueriesTests
    {
        [Test]
        public void FindLocationById_ReturnsMatchingLocation()
        {
            var queries = new WorldQueries(SampleWorldFactory.CreateSmall());
            Location? location = queries.FindLocationById(734);
            Assert.That(location, Is.Not.Null);
            Assert.That(location!.Name, Is.EqualTo("North Gate"));
        }

        [Test]
        public void FindLocationById_ReturnsNullWhenMissing()
        {
            var queries = new WorldQueries(SampleWorldFactory.CreateSmall());
            Assert.That(queries.FindLocationById(9999), Is.Null);
        }
    }
}
