using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CH.Bson.Test
{
    [TestFixture, Category("Unit")]
    public sealed partial class BsonExtensionsTests
    {
        [Test]
        [TestCase("a", new[] {"a"})]
        [TestCase("a[0]", new[] {"a","[0]"})]
        [TestCase("a.b", new[] {"a","b"})]
        [TestCase("a..cd", new[] {"a","cd"})]
        [TestCase("a.c[1]..d[4][5]", new[] {"a","c","[1]","d","[4]","[5]"})]
        [TestCase("a.c[1]..d[45]", new[] { "a", "c", "[1]", "d", "[45]" })]
        public void CanSplitPath(string path, IEnumerable<string> result)
        {
            Assert.IsTrue(BsonExtensions.SplitPath(path).SequenceEqual(result));
        }
    }
}