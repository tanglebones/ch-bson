using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static BsonDocument Diff(this BsonValue valueA, BsonValue valueB)
        {
            if (valueA.BsonType != valueB.BsonType)
                return new BsonDocument("types differ",
                                        new BsonDocument
                                            {{"a", valueA.BsonType.ToString()}, {"b", valueB.BsonType.ToString()}});

            switch (valueB.BsonType)
            {
                case BsonType.Document:
                    return Diff(valueA.AsBsonDocument, valueB.AsBsonDocument);
                case BsonType.Array:
                    return Diff(valueA.AsBsonArray, valueB.AsBsonArray);
                default:
                    if (valueB != valueA)
                        return new BsonDocument("values differ", new BsonDocument {{"a", valueA}, {"b", valueB}});
                    break;
            }
            return new BsonDocument();
        }

        public static BsonDocument Diff(this BsonArray a, BsonArray b)
        {
            if (a.Count != b.Count)
                return new BsonDocument("counts differ", new BsonDocument {{"a", a.Count}, {"b", b.Count}});
            var bson = new BsonDocument();
            for (var i = 0; i < a.Count; ++i)
            {
                var d = Diff(a[i], b[i]);
                if (d.ElementCount > 0)
                    bson[i.ToString(CultureInfo.InvariantCulture)] = d;
            }
            return bson;
        }

        private static Boolean HasSameElementNamesAs(this BsonDocument a, BsonDocument b)
        {
            var union = a.Names.Union(b.Names);
            var intersect = a.Names.Intersect(b.Names);

            return union.Count() == intersect.Count();
        }

        public static BsonDocument Diff(this BsonDocument a, BsonDocument b)
        {
            var bson = ProcessDifferencesInDocument(a, b);


            bson.Add(ProcessFields(a, b));

            return bson;
        }

        private static IEnumerable<BsonElement> ProcessFields(this BsonDocument a, BsonDocument b)
        {
            return from e in a.Where(e => b.Contains(e.Name))
                   let d = Diff(e.Value, b[e.Name])
                   where d.ElementCount > 0
                   select new BsonElement(e.Name, d);
        }

        private static BsonDocument ProcessDifferencesInDocument(this BsonDocument a, BsonDocument b)
        {
            var bson = new BsonDocument();

            if (!a.HasSameElementNamesAs(b))
            {
                foreach (var e in a.Where(e => !b.Contains(e.Name)))
                {
                    bson.SetElement(new BsonElement("+a:" + e.Name, e.Value));
                }
                foreach (var e in b.Where(e => !a.Contains(e.Name)))
                {
                    bson.SetElement(new BsonElement("+b:" + e.Name, e.Value));
                }
            }

            return bson;
        }
    }
}