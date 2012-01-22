using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
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
    }
}