using System;
using System.Collections.Generic;
using MongoDB.Bson;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
// ReSharper disable UnusedMember.Local
        private static IEnumerable<Tuple<string, BsonDocument>> ParseBsonDocumentCasesNoCover
// ReSharper restore UnusedMember.Local
        {
            get
            {
                yield return Tuple.Create("{}", new BsonDocument());
                yield return Tuple.Create("{'a':'test'}", new BsonDocument("a", "test"));
            }
        }

        [Test]
        [TestCaseSource(typeof (BsonExtensionsTests), "ParseBsonDocumentCasesNoCover")]
        public void ParseBsonDocumentValidDocuments(Tuple<string, BsonDocument> testcase)
        {
            var doc = testcase.Item1.ParseBsonDocument();

            var diff = doc.Diff(testcase.Item2);

            Assert.That(diff.ElementCount, Is.EqualTo(0));
        }

        [Test]
        [TestCase("[]")]
        [TestCase("{")]
        public void ParseBsonDocumentInvalidDocuments(string str)
        {
            ThowsAnExceptionNoCover(
                () => str.ParseBsonDocument()
            );
        }

        private static void ThowsAnExceptionNoCover(Action action)
        {
            try
            {
                action();
                Assert.Fail();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
            }
        }
    }
}