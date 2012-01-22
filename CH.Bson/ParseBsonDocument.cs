using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        public static BsonDocument ParseBsonDocument(this string json)
        {
            return ParseBsonDocumentNoCover(json);
        }

        private static BsonDocument ParseBsonDocumentNoCover(string json)
        {
            var stringReader = new StringReader(json);
            {
                // you can't do using(stringReader) because bsonReader will dispose of stringReader
                BsonReader bsonReader;
                try // this is lame, but needed to get FxCop happy.
                    // it would be better if bsonReader didn't dispose of things it doesn't create/own!
                {
                    bsonReader = BsonReader.Create(stringReader);
                }
                catch
                {
                    stringReader.Dispose();
                    throw;
                }
                var safeBsonReaderCreateNoCover = bsonReader;
                using (safeBsonReaderCreateNoCover)
                {
                    return BsonDocument.ReadFrom(safeBsonReaderCreateNoCover);
                } // disposing of BsonReader will dispose stringReader
            }
        }
    }
}