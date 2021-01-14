#nullable enable

using System;
using System.Linq;
using System.Text;

namespace Brimborium.Json {
    /// <summary>
    /// High-Level API of Brimborium.Json.
    /// </summary>
    public static partial class JsonSerializer {
        public static string PrettyPrint(byte[] json) {
            return PrettyPrint(json, 0);
        }

        public static string PrettyPrint(byte[] json, int offset) {
            var reader = new JsonReaderUtf8(json, offset);
            var writer = new JsonWriterUtf8(MemoryPool.GetBuffer());
            WritePrittyPrint(reader, writer, 0);
            return writer.ToString();
        }

        public static string PrettyPrint(string json) {
            var reader = new JsonReaderUtf8(Encoding.UTF8.GetBytes(json));
            var writer = new JsonWriterUtf8(MemoryPool.GetBuffer());
            WritePrittyPrint(reader, writer, 0);
            return writer.ToString();
        }

        public static byte[] PrettyPrintByteArray(byte[] json) {
            return PrettyPrintByteArray(json, 0);
        }

        public static byte[] PrettyPrintByteArray(byte[] json, int offset) {
            var reader = new JsonReaderUtf8(json, offset);
            var writer = new JsonWriterUtf8(MemoryPool.GetBuffer());
            WritePrittyPrint(reader, writer, 0);
            return writer.ToUtf8ByteArray();
        }

        public static byte[] PrettyPrintByteArray(string json) {
            var reader = new JsonReaderUtf8(Encoding.UTF8.GetBytes(json));
            var writer = new JsonWriterUtf8(MemoryPool.GetBuffer());
            WritePrittyPrint(reader, writer, 0);
            return writer.ToUtf8ByteArray();
        }

        static readonly byte[][] indent = Enumerable.Range(0, 100).Select(x => Encoding.UTF8.GetBytes(new string(' ', x * 2))).ToArray();
        static readonly byte[] newLine = Encoding.UTF8.GetBytes(Environment.NewLine);

        static void WritePrittyPrint(JsonReader reader, JsonWriter writer, int depth) {
            var token = reader.GetCurrentJsonToken();
            switch (token) {
                case JsonToken.BeginObject: {
                        writer.WriteBeginObject();
                        writer.WriteRaw(newLine);
                        var c = 0;
                        while (reader.ReadIsInObject(ref c)) {
                            if (c != 1) {
                                writer.WriteRaw((byte)',');
                                writer.WriteRaw(newLine);
                            }
                            writer.WriteRaw(indent[depth + 1]);
                            writer.WritePropertyName(reader.ReadPropertyName());
                            writer.WriteRaw((byte)' ');
                            WritePrittyPrint(reader, writer, depth + 1);
                        }
                        writer.WriteRaw(newLine);
                        writer.WriteRaw(indent[depth]);
                        writer.WriteEndObject();
                    }
                    break;
                case JsonToken.BeginArray: {
                        writer.WriteBeginArray();
                        writer.WriteRaw(newLine);
                        var c = 0;
                        while (reader.ReadIsInArray(ref c)) {
                            if (c != 1) {
                                writer.WriteRaw((byte)',');
                                writer.WriteRaw(newLine);
                            }
                            writer.WriteRaw(indent[depth + 1]);
                            WritePrittyPrint(reader, writer, depth + 1);
                        }
                        writer.WriteRaw(newLine);
                        writer.WriteRaw(indent[depth]);
                        writer.WriteEndArray();
                    }
                    break;
                case JsonToken.Number: {
                        var v = reader.ReadDouble();
                        writer.WriteDouble(v);
                    }
                    break;
                case JsonToken.String: {
                        var v = reader.ReadString();
                        writer.WriteString(v);
                    }
                    break;
                case JsonToken.True:
                case JsonToken.False: {
                        var v = reader.ReadBoolean();
                        writer.WriteBoolean(v);
                    }
                    break;
                case JsonToken.Null: {
                        reader.ReadIsNull();
                        writer.WriteNull();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
