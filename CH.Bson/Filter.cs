using System;
using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
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