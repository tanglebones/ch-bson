using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void SetTest()
        {
            var bsonArray = new BsonArray();

            var expected =
                new BsonArray(
                    new[]
                        {
                            BsonNull.Value,
                            BsonNull.Value,
                            BsonNull.Value,
                            BsonNull.Value,
                            BsonValue.Create("test")
                        });

            bsonArray.Set(4, "test");

            var diff = bsonArray.Diff(expected);
            Assert.That(diff.ElementCount, Is.EqualTo(0));
        }
    }
}