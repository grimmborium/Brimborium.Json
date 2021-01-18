#nullable disable

using System;

namespace Brimborium.Json.Formatters
{
    public sealed class IgnoreFormatter<T> : IJsonFormatter<T>
    {
        public void Serialize(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteNull();
        }

        public T Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            reader.ReadNextBlock();
            return default(T);
        }
    }
}
