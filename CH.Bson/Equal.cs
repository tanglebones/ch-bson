using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
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
    }
}