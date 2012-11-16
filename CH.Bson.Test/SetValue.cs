using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void SetValueTest()
        {
            var doc = new BsonDocument();

            var expected = BsonDocument.Parse("{'a':{'b':{'c':1}}}");

            doc.SetValue("a.b", "c");
            doc.SetValue("a.b", new BsonDocument("c",1));

            var diff = doc.Diff(expected);
            Assert.That(diff.ElementCount, Is.EqualTo(0));

            doc.SetValue("e",1);
            var expected2 = BsonDocument.Parse("{'a':{'b':{'c':1}},'e':1}");
            diff = doc.Diff(expected2);
            Assert.That(diff.ElementCount, Is.EqualTo(0));

            doc.SetValue(string.Empty, 1);
            diff = doc.Diff(expected2);
            Assert.That(diff.ElementCount, Is.EqualTo(0));
        }
    }
}