using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void DiffTwoDifferentBsonTypes()
        {
            // Arrange
            var a = new BsonInt32(100);
            var b = new BsonInt64(100);
            var expected = new BsonDocument("types differ", new BsonDocument {{"a", "Int32"}, {"b", "Int64"}});

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc, Is.EqualTo(expected));
        }

        [Test]
        public void DiffTwoBsonDocumentsWithDifferentElementNames()
        {
            // Arrange
            var a = new BsonDocument {new BsonElement("Name", "John"), new BsonElement("Age", 20)};
            var b = new BsonDocument {new BsonElement("Name", "John"), new BsonElement("Weight", 160)};
            var expected = new BsonDocument {new BsonElement("+a:Age", 20), new BsonElement("+b:Weight", 160)};

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc.Equals(expected));
        }

        [Test]
        public void DiffTwoBsonDocumentsWithElementValues()
        {
            // Arrange
            var a = new BsonDocument {new BsonElement("Name", "John"), new BsonElement("Age", 20)};
            var b = new BsonDocument {new BsonElement("Name", "John"), new BsonElement("Age", 30)};
            var expected =
                new BsonDocument
                    {
                        new BsonElement(
                            "Age",
                            new BsonDocument(
                                "values differ",
                                new BsonDocument {{"a", 20}, {"b", 30}}
                                )
                            )
                    };

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc.Equals(expected));
        }

        [Test]
        public void DiffTwoBsonArraysWithDifferentCounts()
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
            var expected = new BsonDocument("counts differ", new BsonDocument {{"a", 2}, {"b", 1}});

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc.Equals(expected));
        }

        [Test]
        public void DiffTwoBsonValuesOfSameType()
        {
            // Arrange
            var a = new BsonInt32(1);
            var b = new BsonInt32(2);
            var expectedDiff =
                new BsonDocument
                    {
                        new BsonElement("values differ", new BsonDocument {{"a", 1}, {"b", 2}})
                    };

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expectedDiff));
        }

        [Test]
        public void DiffTwoDifferentBsonValuesOfTypeDocument()
        {
            // Arrange
            BsonValue a = new BsonDocument("x", 1);
            BsonValue b = new BsonDocument("x", 2);
            var expectedDiff =
                new BsonDocument
                    {
                        new BsonElement(
                            "x",
                            new BsonDocument
                                {
                                    new BsonElement(
                                        "values differ",
                                        new BsonDocument {{"a", 1}, {"b", 2}})
                                }
                            )
                    };

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expectedDiff));
        }

        [Test]
        public void DiffTwoIdenticalBsonValuesOfTypeDocument()
        {
            // Arrange
            BsonValue a = new BsonDocument("x", 1);
            BsonValue b = new BsonDocument("x", 1);
            var expectedDiff = new BsonDocument();

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expectedDiff));
        }

        [Test]
        public void DiffTwoDifferentBsonValuesOfTypeArray()
        {
            // Arrange
            BsonValue a = new BsonArray {1, 2, 3, 6};
            BsonValue b = new BsonArray {1, 2, 4, 5};
            var expected =
                new BsonDocument
                    {
                        new BsonDocument("2", new BsonDocument("values differ", new BsonDocument {{"a", 3}, {"b", 4}})),
                        new BsonDocument("3", new BsonDocument("values differ", new BsonDocument {{"a", 6}, {"b", 5}}))
                    };

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void DiffTwoIdenticalBsonArrays()
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
            var expected = new BsonDocument();

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc.Equals(expected));
        }

        [Test]
        public void DiffTwoDifferentBsonArrays()
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
            var expected =
                new BsonDocument(
                    "0",
                    new BsonDocument(
                        "values differ",
                        new BsonDocument {{"a", 100}, {"b", 101}}));

            // Act
            var doc = a.Diff(b);

            // Assert
            Assert.That(doc, Is.EqualTo(expected));
        }

        [Test]
        public void DiffTwoDifferentBsonValues()
        {
            // Arrange
            var a = new BsonInt32(1);
            var b = new BsonInt32(2);
            var expectedDiff = new BsonDocument("values differ", new BsonDocument {{"a", 1}, {"b", 2}});

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expectedDiff));
        }

        [Test]
        public void DiffTwoIdenticalBsonValues()
        {
            // Arrange
            var a = new BsonInt32(1);
            var b = new BsonInt32(1);
            var expectedDiff = new BsonDocument();

            // Act
            var value = a.Diff(b);

            // Assert
            Assert.That(value, Is.EqualTo(expectedDiff));
        }
    }
}