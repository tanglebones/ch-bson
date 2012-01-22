using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void AutoVivifyCanAddressContainedPathsDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", "foo"));
            bson.AvDocument("test")["bar"] = "baz";
            var expected = "{'test':{'doc':'foo','bar':'baz'}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanAddressContainedPathsDocArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", "foo"));
            bson.AvArray("test.bar").Add("baz");
            var expected = "{'test':{'doc':'foo','bar':['baz']}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanAddressLeafPathsDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvDocument("test.doc")["bar"] = "baz";
            var expected = "{'test':{'doc':{'bar':'baz'}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanAddressLeafPathsDocArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvArray("test.doc.bar").Add("baz");
            var expected = "{'test':{'doc':{'bar':['baz']}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByOneDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvDocument("test.doc.foo")["bar"] = "baz";
            var expected = "{'test':{'doc':{'foo':{'bar':'baz'}}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByOneArrArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("arr", new BsonArray()));
            bson.AvArray("test.arr[0]").Add("baz");
            var expected = "{'test':{'arr':[['baz']]}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByOneArrDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("arr", new BsonArray()));
            bson.AvDocument("test.arr[0]")["bar"] = "baz";
            var expected = "{'test':{'arr':[{'bar':'baz'}]}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }


        [Test]
        public void AutoVivifyCanExtendLeafByOneDocArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvArray("test.doc.foo.bar").Add("baz");
            var expected = "{'test':{'doc':{'foo':{'bar':['baz']}}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByTwoDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvDocument("test.doc.foo.qux")["bar"] = "baz";
            var expected = "{'test':{'doc':{'foo':{'qux':{'bar':'baz'}}}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByTwoDocArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvDocument("test.doc.foo[0]")["bar"] = "baz";
            var expected = "{'test':{'doc':{'foo':[{'bar':'baz'}]}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByTwoArrDoc()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvDocument("test.doc.foo[0].qux")["bar"] = "baz";
            var expected = "{'test':{'doc':{'foo':[{'qux':{'bar':'baz'}}]}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByTwoArr()
        {
            var bson = new BsonDocument("test", new BsonDocument("doc", new BsonDocument()));
            bson.AvArray("test.doc.foo.qux").Add("baz");
            var expected = "{'test':{'doc':{'foo':{'qux':['baz']}}}}".ParseBsonDocument();
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanAddressContainedPathsArray()
        {
            var bson = new BsonArray {new BsonArray {"test", new BsonArray {"doc"}}, "foo"};
            bson.AvArray("[0]").Add("baz");
            var expected = new BsonArray {new BsonArray {"test", new BsonArray {"doc"}, "baz"}, "foo"};
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanAddressLeafPathsArray()
        {
            var bson = new BsonArray {new BsonArray {"test", new BsonArray {"doc"}}, "foo"};
            bson.AvArray("[0][1]").Add("baz");
            var expected = new BsonArray {new BsonArray {"test", new BsonArray {"doc", "baz"}}, "foo"};
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyCanExtendLeafByOneArray()
        {
            var bson = new BsonArray {new BsonArray {"test", new BsonArray {"doc"}}, "foo"};
            bson.AvArray("[0][1][1]").Add("baz");
            var expected = new BsonArray {new BsonArray {"test", new BsonArray {"doc", new BsonArray {"baz"}}}, "foo"};
            Assert.That(bson.Equals(expected));
        }


        [Test]
        public void AutoVivifyCanExtendLeafByTwoArray()
        {
            var bson = new BsonArray {new BsonArray {"test", new BsonArray {"doc"}}, "foo"};
            bson.AvArray("[0][1][1][0]").Add("baz");
            var expected = new BsonArray
                               {
                                   new BsonArray {"test", new BsonArray {"doc", new BsonArray {new BsonArray {"baz"}}}},
                                   "foo"
                               };
            Assert.That(bson.Equals(expected));
        }

        [Test]
        public void AutoVivifyThrowsOnArrayIndexTypeMismatch()
        {
            var bson = new BsonDocument("a", "b");
            Assert.Throws<ArgumentException>(() => bson.AutoVivify("[0]", BsonType.Document));
        }

        [Test]
        public void AutoVivifyThrowsOnDocumentTypeMismatch()
        {
            var bson = new BsonArray();
            Assert.Throws<ArgumentException>(() => bson.AutoVivify("a", BsonType.Document));
        }

        [Test]
        public void AutoVivifyThrowsOnArrayMismatch()
        {
            var bson = new BsonDocument("a", "b");
            Assert.Throws<ArgumentException>(() => bson.AutoVivify("a", BsonType.Array));
        }
    }
}