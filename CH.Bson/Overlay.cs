using System;
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
    }
}