using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static BsonValue SelectValue(this BsonValue bson, string path, BsonValue returnIfNotFound = null)
        {
            try
            {
                var pa = SplitPath(path);
                bson = SelectValueFromPath(bson, pa, 0, pa.Length);
                return bson;
            }
            catch
            {
                return returnIfNotFound;
            }
        }

        internal static BsonValue SelectValueFromPath(BsonValue bson, IEnumerable<string> pa, int skip, int take)
        {
            foreach (var p in pa.Skip(skip).Take(take))
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
    }
}