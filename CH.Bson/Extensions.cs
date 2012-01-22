using System;
using System.Globalization;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static BsonDocument ParseBsonDocument(this string json)
        {
            var stringReader = new StringReader(json);
            { // you can't do using(stringReader) because bsonReader will dipose of stringReader
                BsonReader bsonReader;
                try // this is lame, since .Create should dipose of stringReader before throwing!
                    // it would be better if bsonReader shouldn't dispose of things it doesn't create/own!
                {
                    bsonReader = BsonReader.Create(stringReader);
                }
                catch
                {
                    stringReader.Dispose();
                    throw;
                }

                using (bsonReader)
                {
                    return BsonDocument.ReadFrom(bsonReader);
                } // disposing of BsonReader will dispose stringReader
            }
        }

        public static bool Equal(this BsonValue valueA, BsonValue valueB)
        {
            if (valueA.BsonType != valueB.BsonType) return false;

            switch (valueA.BsonType)
            {
                case BsonType.Document:
                    return Equal(valueA.AsBsonDocument, valueB.AsBsonDocument);
                case BsonType.Array:
                    return Equal(valueA.AsBsonArray, valueB.AsBsonArray);
                default:
                    return valueA == valueB;
            }
        }

        public static bool Equal(this BsonArray a, BsonArray b)
        {
            if (a.Count != b.Count) return false;
            return !a.Where((t, index) => !Equal(b[index], t)).Any();
        }

        public static bool Equal(this BsonDocument a, BsonDocument b)
        {
            if (a.ElementCount != b.ElementCount)
                return false;

            foreach (var elementA in a)
            {
                if (!b.Contains(elementA.Name)) return false;
                if (!Equal(elementA.Value, b[elementA.Name])) return false;
            }
            return true;
        }

        public static BsonDocument Diff(this BsonValue valueA, BsonValue valueB)
        {
            if (valueA.BsonType != valueB.BsonType) return new BsonDocument("types differ", new BsonDocument { { "a", valueA.BsonType.ToString() }, { "b", valueB.BsonType.ToString() } });

            switch (valueB.BsonType)
            {
                case BsonType.Document:
                    return Diff(valueA.AsBsonDocument, valueB.AsBsonDocument);
                case BsonType.Array:
                    return Diff(valueA.AsBsonArray, valueB.AsBsonArray);
                default:
                    if (valueB != valueA) return new BsonDocument("values differ", new BsonDocument { { "a", valueA }, { "b", valueB } });
                    break;
            }
            return new BsonDocument();
        }

        public static BsonDocument Diff(this BsonArray a, BsonArray b)
        {
            if (a.Count != b.Count) return new BsonDocument("counts differ", new BsonDocument { { "a", a.Count }, { "b", b.Count } });
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
            if (!a.HasSameElementNamesAs(b))
            {
                var bson = new BsonDocument();
                foreach (var e in a.Where(e => !b.Contains(e.Name)))
                {
                    bson.SetElement(new BsonElement("+a:" + e.Name, e.Value));
                }
                foreach (var e in b.Where(e => !a.Contains(e.Name)))
                {
                    bson.SetElement(new BsonElement("+b:" + e.Name, e.Value));
                }
                return bson;
            }
            else
            {
                var bson = new BsonDocument();

                foreach (var e in a)
                {
                    var d = Diff(e.Value, b[e.Name]);
                    if (d.ElementCount > 0)
                        bson.Add(e.Name, d);
                }
                return bson;
            }
        }

        public static BsonValue SelectValue(this BsonValue bson, string path, BsonValue returnIfNotFound = null)
        {
            try
            {
                var pa = SplitPath(path);
                foreach (var p in pa)
                {
                    if (p.StartsWith("["))
                    {
                        var index = Int32.Parse(p.Substring(1, p.Length - 2));
                        bson = bson.AsBsonArray[index];
                    }
                    else
                    {
                        bson = bson.AsBsonDocument[p];
                    }
                }
                return bson;
            }
            catch
            {
                return returnIfNotFound;
            }
        }

        public static void Set(this BsonArray bsonArray, int index, BsonValue bson)
        {
            var count = bsonArray.Count;
            if (count <= index)
            {
                for (var i = 0; i < (index - count + 1); ++i)
                    bsonArray.Add(BsonNull.Value);
            }
            bsonArray[index] = bson;
        }

        public static BsonArray AvArray(this BsonValue bson, string path)
        {
            return bson.AutoVivify(path, BsonType.Array).AsBsonArray;
        }

        public static BsonDocument AvDocument(this BsonValue bson, string path)
        {
            return bson.AutoVivify(path, BsonType.Document).AsBsonDocument;
        }

        public static BsonValue AutoVivify(this BsonValue bson, string path, BsonType type)
        {
            var cur = bson;
            var pa = SplitPath(path);
            var i = 0;

            var aslot = 0;
            var akey = "";

            // follow the path down the tree until we hit a leaf or run out of path
            while (i < pa.Length)
            {
                var p = pa[i];
                if (p.StartsWith("["))
                {
                    var index = Int32.Parse(p.Substring(1, p.Length - 2));
                    if (cur.IsBsonArray)
                    {
                        var ba = cur.AsBsonArray;
                        if (index < ba.Count)
                            cur = ba[index];
                        else
                        {
                            aslot = index;
                            break;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("bson type not array at path element " +
                                                    string.Join(".", pa.Take(i + 1)));
                    }
                }
                else
                {
                    if (cur.IsBsonDocument)
                    {
                        var bd = cur.AsBsonDocument;
                        if (bd.Contains(p))
                            cur = bd[p];
                        else
                        {
                            akey = p;
                            break;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("bson type not document at path element " +
                                                    string.Join(".", pa.Take(i)));
                    }
                }
                ++i;
            }

            if (i == pa.Length)
            {
                // full path is contained in the tree
                if (cur.BsonType != type)
                    throw new ArgumentException("bson type mismatch at path element " + path);
                return cur;
            }

            ++i;
            // path goes beyond existing tree, extend it
            while (i < pa.Length)
            {
                var p = pa[i];
                if (p.StartsWith("["))
                {
                    var index = Int32.Parse(p.Substring(1, p.Length - 2));
                    var ba = new BsonArray();
                    if (cur.IsBsonArray)
                        cur.AsBsonArray.Set(aslot, ba);
                    else
                        cur.AsBsonDocument[akey] = ba;
                    aslot = index;
                    cur = ba;
                }
                else
                {
                    var bd = new BsonDocument();
                    if (cur.IsBsonArray)
                        cur.AsBsonArray.Set(aslot, bd);
                    else
                        cur.AsBsonDocument[akey] = bd;
                    akey = p;
                    cur = bd;
                }
                ++i;
            }

            if (type == BsonType.Array)
            {
                var ba = new BsonArray();
                if (cur.IsBsonArray)
                    cur.AsBsonArray.Set(aslot, ba);
                else
                    cur.AsBsonDocument[akey] = ba;
                cur = ba;
            }
            else
            {
                var bd = new BsonDocument();
                if (cur.IsBsonArray)
                    cur.AsBsonArray.Set(aslot, bd);
                else
                    cur.AsBsonDocument[akey] = bd;
                cur = bd;
            }
            return cur;
        }

        private static string[] SplitPath(string path)
        {
            return path.Replace("[", ".[").Split(new []{'.'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void Filter(this BsonArray bson, Func<string, BsonValue, bool> filter, string prefix = null)
        {
            var bsonValues = bson.ToArray();
            // run backwards to removed elements don't shift the index
            for (var index = bsonValues.Length - 1; index >= 0; --index)
            {
                var element = bsonValues[index];
                var elementName = prefix + "[]";

                switch (element.BsonType)
                {
                    case BsonType.Document:
                        Filter(element.AsBsonDocument, filter, elementName);
                        if (!element.AsBsonDocument.Any())
                            bson.RemoveAt(index);
                        break;
                    case BsonType.Array:
                        Filter(element.AsBsonArray, filter, elementName);
                        if (element.AsBsonArray.Count == 0)
                            bson.RemoveAt(index);
                        break;
                    default:
                        if (!filter(elementName, element))
                        {
                            bson.RemoveAt(index);
                        }
                        break;
                }
            }
        }

        public static void Filter(this BsonDocument bson, Func<string, BsonValue, bool> filter, string prefix = null)
        {
            // use index to avoid issues where the key is duplicated
            // run backwards to avoid the index shift when removing
            var bsonElements = bson.ToArray();
            for (var index = bsonElements.Length - 1; index >= 0; --index)
            {
                var element = bsonElements[index];
                var elementName = prefix == null ? element.Name : prefix + "." + element.Name;

                switch (element.Value.BsonType)
                {
                    case BsonType.Document:
                        Filter(element.Value.AsBsonDocument, filter, elementName);
                        if (!element.Value.AsBsonDocument.Any())
                            bson.RemoveAt(index);
                        break;
                    case BsonType.Array:
                        Filter(element.Value.AsBsonArray, filter, elementName);
                        if (element.Value.AsBsonArray.Count == 0)
                            bson.RemoveAt(index);
                        break;
                    default:
                        if (!filter(elementName, element.Value))
                            bson.RemoveAt(index);
                        break;
                }
            }
        }
    }
}