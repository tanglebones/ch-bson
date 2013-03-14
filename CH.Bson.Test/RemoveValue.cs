using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void RemoveValueTest()
        {
            var doc = BsonDocument.Parse("{'a':{'b':{'c':1}}}");

            var expected = BsonDocument.Parse("{'a':{}}");

            doc.RemoveValue("a.b");

            var diff = doc.Diff(expected);
            Assert.That(diff.ElementCount, Is.EqualTo(0));

            doc.RemoveValue(string.Empty);
            diff = doc.Diff(expected);
            Assert.That(diff.ElementCount, Is.EqualTo(0));

            doc.RemoveValue("a");
            Assert.AreEqual(doc.ElementCount,0);
        }
    }
}