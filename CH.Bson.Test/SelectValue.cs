using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void SelectValuePathDoesntMatchDocument()
        {
            // Arrange
            var bson =
                new BsonArray
                    {
                        new BsonArray {1, 2},
                        new BsonArray {new BsonDocument("a",1), 4}
                    };

            // Act
            // Assert
            Assert.IsNull(bson.SelectValue("a"));
            Assert.IsNull(bson.SelectValue("[1][0][4]"));
            Assert.IsNull(bson.SelectValue("[4]"));
        }

        [Test]
        public void SelectValueOfBsonArray()
        {
            // Arrange
            var bson =
                new BsonArray
                    {
                        new BsonArray {1, 2},
                        new BsonArray {3, 4}
                    };

            // Act
            var value = bson.SelectValue("[1][0]");

            // Assert
            Assert.That(value.AsInt32, Is.EqualTo(3));
        }

        [Test]
        public void SelectValueOfMissingBsonArray()
        {
            // Arrange
            var bson = new BsonDocument();

            // Act
            var value = bson.SelectValue("missing[0]");

            // Assert
            Assert.IsNull(value);
        }

        [Test]
        public void SelectValueOfEmptyBsonArray()
        {
            // Arrange
            var bson = new BsonDocument("empty", new BsonArray());

            // Act
            var value = bson.SelectValue("empty[0]");

            // Assert
            Assert.IsNull(value);
        }

        [Test]
        public void SelectValueReturnsNullWhenHasException()
        {
            // Arrange
            var bson = new BsonArray {1, 2};

            // Act
            var value = bson.SelectValue("[BadIndex]");

            // Assert
            Assert.Null(value);
        }

        [Test]
        public void SelectValueOfBsonDocument()
        {
            // Arrange
            var bson =
                new BsonDocument
                    {
                        new BsonElement("array", new BsonArray {1, 2})
                    };

            // Act
            var value = bson.SelectValue("array[0]");

            // Assert
            Assert.That(value.AsInt32, Is.EqualTo(1));
        }

        [Test]
        public void SelectValueOfBsonDocumentByPath()
        {
            // Arrange
            var bson =
                new BsonDocument
                    {
                        {"A", new BsonDocument {{"B", 1}}}
                    };


            // Act
            var value = bson.SelectValue("A.B");

            // Assert
            Assert.That(value.AsInt32, Is.EqualTo(1));
        }

        [Test]
        public void SelectValueReturnsNotFoundIfDoesntFindPath()
        {
            // Arrange
            var bson =
                new BsonDocument
                    {
                        {"A", new BsonDocument {{"B", 1}}}
                    };


            // Act
            var value = bson.SelectValue("A.C", BsonUndefined.Value);

            // Assert
            Assert.That(value, Is.EqualTo(BsonUndefined.Value));
        }
    }
}