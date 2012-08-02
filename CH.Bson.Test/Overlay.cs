using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {

        [Test]
        public void Overlay_GivenSimpleValues_Overwrites()
        {
            // arrange
            var bson = new BsonDocument { { "a", 1 }, { "b", "c" } };
            var with = new BsonDocument { { "a", 3 }, { "b", "f" } };

            // act
            bson.Overlay(with);

            // assert
            Assert.AreEqual(bson["a"].AsInt32, 3);
            Assert.AreEqual(bson["b"].AsString, "f");
        }

        [Test]
        public void Overlay_GivenDocuments_Merges()
        {
            // arrange
            var bson = new BsonDocument("a", new BsonDocument("b", new BsonDocument("c", "d")));
            var with = new BsonDocument("a", new BsonDocument("b", new BsonDocument("e", "f")));

            // act
            bson.Overlay(with);

            // assert
            Assert.AreEqual(bson.SelectValue("a.b.c").AsString, "d");
            Assert.AreEqual(bson.SelectValue("a.b.e").AsString, "f");
        }

        [Test]
        public void Overlay_GivenArrays_Concats()
        {
            // arrange
            var bson = new BsonDocument("a", new BsonArray { new BsonDocument("c", "d") });
            var with = new BsonDocument("a", new BsonArray { new BsonDocument("e", "f") });

            // act
            bson.Overlay(with);

            // assert
            Assert.AreEqual(bson.SelectValue("a[0].c").AsString, "d");
            Assert.AreEqual(bson.SelectValue("a[1].e").AsString, "f");
        }
    }
}