using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void EqualTwoIdenticalBsonValues()
        {
            // Arrange
            var a = new BsonInt32(100);
            var b = new BsonInt32(100);

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.True(eq);
        }

        [Test]
        public void EqualTwoDifferentBsonValues()
        {
            // Arrange
            var a = new BsonInt32(100);
            var b = new BsonInt32(200);

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoIdenticalBsonValueOfTypeDocument()
        {
            // Arrange
            BsonValue a =
                new BsonDocument
                    {
                        new BsonElement("array", new BsonArray {"1", "2", "3"})
                    };
            BsonValue b =
                new BsonDocument
                    {
                        new BsonElement("array", new BsonArray {"1", "2", "3"})
                    };
            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.True(eq);
        }

        [Test]
        public void EqualTwoDifferentBsonTypes()
        {
            // Arrange
            var a = new BsonInt32(100);
            var b = new BsonInt64(100);

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoIdenticalBsonDocuments()
        {
            // Arrange
            var a =
                new BsonDocument
                    {
                        new BsonElement("array", new BsonArray {"1", "2", "3"})
                    };
            var b =
                new BsonDocument
                    {
                        new BsonElement("array", new BsonArray {"1", "2", "3"})
                    };
            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.True(eq);
        }

        [Test]
        public void EqualTwoBsonDocumentsWithDifferentElementNames()
        {
            // Arrange
            var a =
                new BsonDocument
                    {
                        {"array1", new BsonArray {"1", "2", "3"}}
                    };
            var b =
                new BsonDocument
                    {
                        {"array2", new BsonArray {"1", "2", "3"}}
                    };
            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoBsonDocumentsWithDifferentElementValues()
        {
            // Arrange
            var a =
                new BsonDocument
                    {
                        {"array", new BsonArray {"1", "2", "3"}}
                    };
            var b =
                new BsonDocument
                    {
                        {"array", new BsonArray {"1", "1", "1"}}
                    };
            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoBsonDocumentsWithDifferentElementCounts()
        {
            // Arrange
            var a =
                new BsonDocument
                    {
                        {"a", new BsonInt32(1)},
                        {"b", new BsonInt32(2)}
                    };
            var b =
                new BsonDocument
                    {
                        {"c", new BsonInt32(3)},
                    };
            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoIdenticalBsonArrays()
        {
            // Arrange
            var a =
                new BsonArray
                    {
                        new BsonInt32(100),
                        new BsonInt32(200)
                    };
            var b =
                new BsonArray
                    {
                        new BsonInt32(100),
                        new BsonInt32(200)
                    };

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.True(eq);
        }

        [Test]
        public void EqualTwoDifferentBsonArrays()
        {
            // Arrange
            var a =
                new BsonArray
                    {
                        new BsonInt32(100),
                        new BsonInt32(200)
                    };
            var b =
                new BsonArray
                    {
                        new BsonInt32(101),
                        new BsonInt32(200)
                    };

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }

        [Test]
        public void EqualTwoBsonArraysWithDifferentCount()
        {
            // Arrange
            var a =
                new BsonArray
                    {
                        new BsonInt32(100),
                        new BsonInt32(200)
                    };
            var b =
                new BsonArray
                    {
                        new BsonInt32(101)
                    };

            // Act
            var eq = a.Equal(b);

            // Assert
            Assert.False(eq);
        }
    }
}