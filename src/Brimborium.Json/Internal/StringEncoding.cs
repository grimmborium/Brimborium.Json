using System.Text;

namespace Brimborium.Json.Internal
{
    public static class StringEncoding
    {
        public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
    }
}