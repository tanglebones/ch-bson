using System.Linq;
using MongoDB.Bson;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static void RemoveValue(this BsonDocument doc, string path)
        {
            var pa = SplitPath(path);
            if (pa.Length == 0) return;
            if (pa.Length == 1) doc.Remove(path);
            var subdoc = SelectValueFromPath(doc, pa, 0, pa.Length - 1);
            if (subdoc.IsBsonDocument)
            {
                subdoc.AsBsonDocument.Remove(pa.Last());
            }
        }
    }
}