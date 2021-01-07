#if NETSTANDARD
#nullable disable

using System;
using Brimborium.Json.Internal;
using Brimborium.Json.Formatters.Internal;

namespace Brimborium.Json.Formatters.Internal
{
    // reduce static constructor generate size on generics(especially IL2CPP on Unity)
    internal static class ValueTupleFormatterHelper
    {
        internal static readonly byte[][] nameCache1;
        internal static readonly AutomataDictionary dictionary1;
        internal static readonly byte[][] nameCache2;
        internal static readonly AutomataDictionary dictionary2;
        internal static readonly byte[][] nameCache3;
        internal static readonly AutomataDictionary dictionary3;
        internal static readonly byte[][] nameCache4;
        internal static readonly AutomataDictionary dictionary4;
        internal static readonly byte[][] nameCache5;
        internal static readonly AutomataDictionary dictionary5;
        internal static readonly byte[][] nameCache6;
        internal static readonly AutomataDictionary dictionary6;
        internal static readonly byte[][] nameCache7;
        internal static readonly AutomataDictionary dictionary7;
        internal static readonly byte[][] nameCache8;
        internal static readonly AutomataDictionary dictionary8;

        static ValueTupleFormatterHelper()
        {
            nameCache1 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
            };
            dictionary1 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
            };
            nameCache2 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
            };
            dictionary2 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
            };
            nameCache3 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
            };
            dictionary3 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
            };
            nameCache4 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item4"),
            };
            dictionary4 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item4"), 3 },
            };
            nameCache5 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item4"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item5"),
            };
            dictionary5 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item4"), 3 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item5"), 4 },
            };
            nameCache6 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item4"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item5"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item6"),
            };
            dictionary6 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item4"), 3 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item5"), 4 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item6"), 5 },
            };
            nameCache7 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item4"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item5"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item6"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item7"),
            };
            dictionary7 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item4"), 3 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item5"), 4 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item6"), 5 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item7"), 6 },
            };
            nameCache8 = new byte[][]
            {
                JsonWriterUtf8.GetEncodedPropertyNameWithBeginObject("Item1"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item2"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item3"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item4"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item5"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item6"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Item7"),
                JsonWriterUtf8.GetEncodedPropertyNameWithPrefixValueSeparator("Rest"),
            };
            dictionary8 = new AutomataDictionary
            {
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item1"), 0 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item2"), 1 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item3"), 2 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item4"), 3 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item5"), 4 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item6"), 5 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Item7"), 6 },
                {JsonWriterUtf8.GetEncodedPropertyNameWithoutQuotation("Rest"), 7 },
            };
        }
    }
}

namespace Brimborium.Json.Formatters
{

    public sealed class ValueTupleFormatter<T1> : IJsonFormatter<ValueTuple<T1>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache1;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary1;

        public void Serialize(JsonWriter writer, ValueTuple<T1> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1>(item1);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2> : IJsonFormatter<ValueTuple<T1, T2>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache2;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary2;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2>(item1, item2);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3> : IJsonFormatter<ValueTuple<T1, T2, T3>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache3;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary3;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4> : IJsonFormatter<ValueTuple<T1, T2, T3, T4>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache4;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary4;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3, T4> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteRaw(cache[3]);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            T4 item4 = default(T4);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    case 3:
                        item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache5;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary5;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteRaw(cache[3]);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            writer.WriteRaw(cache[4]);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            T4 item4 = default(T4);
            T5 item5 = default(T5);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    case 3:
                        item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(reader, formatterResolver);
                        break;
                    case 4:
                        item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache6;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary6;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteRaw(cache[3]);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            writer.WriteRaw(cache[4]);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            writer.WriteRaw(cache[5]);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            T4 item4 = default(T4);
            T5 item5 = default(T5);
            T6 item6 = default(T6);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    case 3:
                        item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(reader, formatterResolver);
                        break;
                    case 4:
                        item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(reader, formatterResolver);
                        break;
                    case 5:
                        item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache7;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary7;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteRaw(cache[3]);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            writer.WriteRaw(cache[4]);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            writer.WriteRaw(cache[5]);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
            writer.WriteRaw(cache[6]);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(writer, value.Item7, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            T4 item4 = default(T4);
            T5 item5 = default(T5);
            T6 item6 = default(T6);
            T7 item7 = default(T7);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    case 3:
                        item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(reader, formatterResolver);
                        break;
                    case 4:
                        item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(reader, formatterResolver);
                        break;
                    case 5:
                        item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(reader, formatterResolver);
                        break;
                    case 6:
                        item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }
    }


    public sealed class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7, TRest> : IJsonFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>> where TRest : struct
    {
        static readonly byte[][] cache = TupleFormatterHelper.nameCache8;
        static readonly AutomataDictionary dictionary = TupleFormatterHelper.dictionary8;

        public void Serialize(JsonWriter writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value, IJsonFormatterResolver formatterResolver)
        {
            writer.WriteRaw(cache[0]);
            formatterResolver.GetFormatterWithVerify<T1>().Serialize(writer, value.Item1, formatterResolver);
            writer.WriteRaw(cache[1]);
            formatterResolver.GetFormatterWithVerify<T2>().Serialize(writer, value.Item2, formatterResolver);
            writer.WriteRaw(cache[2]);
            formatterResolver.GetFormatterWithVerify<T3>().Serialize(writer, value.Item3, formatterResolver);
            writer.WriteRaw(cache[3]);
            formatterResolver.GetFormatterWithVerify<T4>().Serialize(writer, value.Item4, formatterResolver);
            writer.WriteRaw(cache[4]);
            formatterResolver.GetFormatterWithVerify<T5>().Serialize(writer, value.Item5, formatterResolver);
            writer.WriteRaw(cache[5]);
            formatterResolver.GetFormatterWithVerify<T6>().Serialize(writer, value.Item6, formatterResolver);
            writer.WriteRaw(cache[6]);
            formatterResolver.GetFormatterWithVerify<T7>().Serialize(writer, value.Item7, formatterResolver);
            writer.WriteRaw(cache[7]);
            formatterResolver.GetFormatterWithVerify<TRest>().Serialize(writer, value.Rest, formatterResolver);
            writer.WriteEndObject();
        }

        public ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) throw new InvalidOperationException("Data is Nil, ValueTuple can not be null.");

            T1 item1 = default(T1);
            T2 item2 = default(T2);
            T3 item3 = default(T3);
            T4 item4 = default(T4);
            T5 item5 = default(T5);
            T6 item6 = default(T6);
            T7 item7 = default(T7);
            TRest item8 = default(TRest);
            
            var count = 0;
            reader.ReadIsBeginObjectWithVerify();
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref count))
            {
                var keyString = reader.ReadPropertyNameSegmentRaw();
                int key;
#if NETSTANDARD
                dictionary.TryGetValue(keyString, out key);
#else
                dictionary.TryGetValueSafe(keyString, out key);
#endif

                switch (key)
                {
                    case 0:
                        item1 = formatterResolver.GetFormatterWithVerify<T1>().Deserialize(reader, formatterResolver);
                        break;
                    case 1:
                        item2 = formatterResolver.GetFormatterWithVerify<T2>().Deserialize(reader, formatterResolver);
                        break;
                    case 2:
                        item3 = formatterResolver.GetFormatterWithVerify<T3>().Deserialize(reader, formatterResolver);
                        break;
                    case 3:
                        item4 = formatterResolver.GetFormatterWithVerify<T4>().Deserialize(reader, formatterResolver);
                        break;
                    case 4:
                        item5 = formatterResolver.GetFormatterWithVerify<T5>().Deserialize(reader, formatterResolver);
                        break;
                    case 5:
                        item6 = formatterResolver.GetFormatterWithVerify<T6>().Deserialize(reader, formatterResolver);
                        break;
                    case 6:
                        item7 = formatterResolver.GetFormatterWithVerify<T7>().Deserialize(reader, formatterResolver);
                        break;
                    case 7:
                        item8 = formatterResolver.GetFormatterWithVerify<TRest>().Deserialize(reader, formatterResolver);
                        break;
                    default:
                        reader.ReadNextBlock();
                        break;
                }
            }
            
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, item8);
        }
    }

}

#endif