using System.Collections.Generic;
using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        public void FilterArray()
        {
            var bsonArray = new BsonArray {5, BsonDocument.Parse("{'a':2}")};
            bsonArray.Filter((e, v) => e == "[].a");
            var expected = new BsonArray {BsonDocument.Parse("{'a':2}")};
            Assert.That(expected.ToJson(), Is.EqualTo(bsonArray.ToJson()));
        }

        [Test]
        public void FilterArrayToNothing()
        {
            var bsonArray = new BsonArray {1, 2, new BsonArray {4}, 3};
            bsonArray.Filter((e, v) => false);
            Assert.That(bsonArray.Count, Is.EqualTo(0));
        }

        [Test]
        public void FilterDocument()
        {
            var bsonDocument = BsonDocument.Parse(
                @"{
                'a': 234521345,
                'b': {
                   'c': {
                      'd': 'e'
                   },
                   'f':[
                      5,
                      {'g':{'h':1}}
                   ],
                   'r':[[1,2,3]],
                },
                'i': [
                   {'j': {'k':'l'}},
                   {'m':[
                      6,
                      {'n':'o'}
                     ]
                   }
                ],
                'p':'q'
            }");
            var include =
                new HashSet<string>
                    {
                        "a",
                        "b.c.d",
                        "b.r[][]",
                        "i[].m[]",
                        "i[].m[].n",
                        "p"
                    };

            var expected = BsonDocument.Parse(
                @"{
                'a': 234521345,
                'b': {
                   'c': {
                      'd': 'e'
                   },
                   'r':[[1,2,3]],
                },
                'i': [
                   {'m':[
                      6,
                      {'n':'o'}
                     ]
                   }
                ],
                'p':'q'
            }");

            bsonDocument.Filter((e, v) => include.Contains(e));

            Assert.That(bsonDocument.ToJson() == expected.ToJson());
        }
    }
}