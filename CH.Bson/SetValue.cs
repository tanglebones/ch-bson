using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static void SetValue(this BsonDocument doc, string path, BsonValue value)
        {
            var pa = SplitPath(path);
            if (pa.Length == 0) return;
            if (pa.Length == 1) doc[path] = value;
            var subdoc = AutoVivifyFromPath(doc, new BsonDocument(), pa.Take(pa.Length-1).ToArray());
            if (subdoc.IsBsonDocument)
            {
                subdoc.AsBsonDocument[pa.Last()] = value;
            }
        }
    }
}