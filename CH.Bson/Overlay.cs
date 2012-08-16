using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static void Overlay(this BsonDocument bson, BsonDocument with)
        {
            foreach (var element in with)
            {
                if (bson.Contains(element.Name))
                {
                    var original = bson[element.Name];
                    if (original.BsonType == element.Value.BsonType)
                    {
                        if (original.BsonType == BsonType.Document)
                        {
                            bson[element.Name].AsBsonDocument.Overlay(element.Value.AsBsonDocument);
                            continue;
                        }
                        if (original.BsonType == BsonType.Array)
                        {
                            bson[element.Name] = new BsonArray(original.AsBsonArray.Concat(element.Value.AsBsonArray));
                            continue;
                        }
                    }
                }
                bson[element.Name] = element.Value;
            }
        }
    }
}