using System;

namespace CH.Bson
{
    public static partial class BsonExtensions
    {
        internal static string[] SplitPath(string path)
        {
            return path.Replace("[", ".[").Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}