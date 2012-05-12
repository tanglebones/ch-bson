using System;
using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static BsonArray AvArray(this BsonValue bson, string path)
        {
            return bson.AutoVivify(path, new BsonArray()).AsBsonArray;
        }

        public static BsonDocument AvDocument(this BsonValue bson, string path)
        {
            return bson.AutoVivify(path, new BsonDocument()).AsBsonDocument;
        }

        public static BsonValue AutoVivify(this BsonValue bson, string path, BsonValue defaultValue)
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
                if (cur.BsonType != defaultValue.BsonType)
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

            if (cur.IsBsonArray)
                cur.AsBsonArray.Set(aslot, defaultValue);
            else
                cur.AsBsonDocument[akey] = defaultValue;
            return defaultValue;
        }
    }
}