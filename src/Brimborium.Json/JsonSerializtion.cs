#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    /// <summary>
    /// EntryPoint of Brimborium.Json.
    /// </summary>
    public static partial class JsonSerializtion {
        private static JsonConfigurationBuilder? _DefaultBuilder;

        public static JsonConfigurationBuilder DefaultBuilder {
            get {
                var result = _DefaultBuilder;
                if (result == null) {
                    lock (typeof(JsonSerializtion)) {
                        result = _DefaultBuilder;
                        if (result == null) {
                            result = new JsonConfigurationBuilder();
                            System.Threading.Interlocked.Exchange(ref _DefaultBuilder, result);
                        }
                    }
                }
                return result;
            }
        }

        private static JsonConfiguration? _DefaultConfiguration;

        /// <summary>
        /// Is DefaultConfiguration decided?
        /// </summary>
        public static bool IsConfigurationInitialized => _DefaultConfiguration != null;

        public static JsonConfiguration GetDefaultConfiguration() {
            var result = _DefaultConfiguration;
            if (result is object) {
                return result;
            } else {
                lock (typeof(JsonSerializer)) {
                    result = _DefaultConfiguration;
                    if (result is object) {
                        // race
                    } else {
                        var builder = DefaultBuilder;
                        result = builder.Build();
                        System.Threading.Volatile.Write(ref _DefaultConfiguration, result);
                    }
                }
                return result;
            }
        }

        public static void SetDefaultConfiguration(JsonConfiguration? defaultConfiguration) {
            System.Threading.Volatile.Write(ref _DefaultConfiguration, defaultConfiguration);
        }

        /// <summary>
        /// 
        /// var result = Serialize<T>(...,...);
        /// result.GetUsedSpan().CopyTo(....);
        /// result.Return();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static BoundedByteArray SerializeToArray<T>(
            T value,
            [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            using (var sink = new JsonSinkUtf8Array(configuration)) {
                var writer = new JsonWriter(sink);
                writer.Serialize<T>(value);
                writer.Flush();
                return sink.DisposeAndGetBuffer();
            }
        }

        public static void SerializeToAsyncStreamUtf8<T>(
            T value,
            Stream stream,
            [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            throw new NotImplementedException();
        }

        public static void SerializeToSyncStreamUtf8<T>(
            T value,
            Stream stream,
            [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            throw new NotImplementedException();
        }

        public static void SerializeToEncodingStream<T>(
            T value,
            Stream stream,
            [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            throw new NotImplementedException();
        }

        public static string SerializeToString<T>(
            T value,
            Stream stream,
            [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            throw new NotImplementedException();
        }

        public static async Task<T> DeserializeToStreamUtf8Async<T>(
                Stream stream,
                [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            using (var source = new JsonSourceUtf8AsyncStream(stream, configuration)) {
                var reader = new JsonReader(source);
                await source.ReadParseAsync();
                return await reader.DeserializeAsync<T>();
            }
        }

        public static T DeserializeToStreamUtf8<T>(
                Stream stream,
                [AllowNull] JsonConfiguration configuration = default
            ) {
            configuration ??= JsonSerializtion.GetDefaultConfiguration();
            using (var source = new JsonSourceUtf8AsyncStream(stream, configuration)) {
                var reader = new JsonReader(source);
                return reader.Deserialize<T>();
            }
        }

    }
}

#if no
namespace Brimborium.Json {
    /// <summary>
    /// EntryPoint of Brimborium.Json.
    /// </summary>
    public static partial class JsonSerializer {
        private static JsonSerializationConfiguration? _DefaultConfiguration;

        public static readonly List<IJsonFormatterResolver> DefaultJsonFormatterResolvers = new List<IJsonFormatterResolver>();
        public static readonly List<IJsonFormatter> DefaultJsonFormatters = new List<IJsonFormatter>();

       


        private static JsonWriterUtf8? _JsonWriterUtf8;

        /// <summary>
        /// Serialize to binary with default or specified configuration.
        /// </summary>
        public static byte[] Serialize<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            //var writer = System.Threading.Interlocked.Exchange(ref _JsonWriterUtf8, null)
            //    ?? new JsonWriterUtf8(MemoryPool.GetBuffer());
            //configuration.Serialize(writer, value);
            //var result = writer.ToUtf8ByteArray();
            //if (writer.ownBuffer) {
            //    System.Threading.Interlocked.CompareExchange(ref _JsonWriterUtf8, writer.Reset(), null);
            //}
            //return result;

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize<T>(writer, value);
                return writer.ToUtf8ByteArray();
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

        public static void Serialize<T>(JsonWriter writer, T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();
            configuration.Serialize(writer, value);
        }

        /// <summary>
        /// Serialize to stream with specified resolver.
        /// </summary>
        public static void Serialize<T>(Stream stream, T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize<T>(writer, value);
                var buffer = writer.GetBuffer();
                stream.Write(buffer.Array!, buffer.Offset, buffer.Count);
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

#if false
        /// <summary>
        /// Serialize to stream(write async).
        /// </summary>
        public static System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, T value) {
            return SerializeAsync<T>(stream, value, JsonSerializer.DefaultResolver);
        }

        /// <summary>
        /// Serialize to stream(write async) with specified resolver.
        /// </summary>
        public static async System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, T value, IJsonFormatterResolver resolver) {
            if (resolver == null) {
                resolver = JsonSerializer.DefaultResolver;
            }

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                var formatter = resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(writer, value, resolver);
                var buffer = writer.GetBuffer();
                await stream.WriteAsync(buffer.Array!, buffer.Offset, buffer.Count).ConfigureAwait(false);
            } finally {
                BufferPool.Default.Return(buf);
            }
        }
#endif

        /// <summary>
        /// Serialize to binary with specified resolver. Get the raw memory pool byte[]. 
        /// The result can not share across thread and can not hold, so use quickly.
        /// </summary>
        public static ArraySegment<byte> SerializeUnsafe<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var writer = System.Threading.Interlocked.Exchange(ref _JsonWriterUtf8, null)
             ?? new JsonWriterUtf8(MemoryPool.GetBuffer());
            configuration.Serialize<T>(writer, value);
            var result = writer.GetBuffer();
            if (writer.ownBuffer) {
                System.Threading.Interlocked.CompareExchange(ref _JsonWriterUtf8, writer, null);
            }
            return result;
        }

        /// <summary>
        /// Serialize to JsonString with specified resolver.
        /// </summary>
        public static string ToJsonString<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize(writer, value);
                return writer.ToString();
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

        public static T Deserialize<T>(string json, [AllowNull] JsonSerializationConfiguration configuration = default) {
            return Deserialize<T>(StringEncoding.UTF8NoBOM.GetBytes(json), configuration);
        }

        public static T Deserialize<T>(byte[] bytes, [AllowNull] JsonSerializationConfiguration configuration = default) {
            return Deserialize<T>(bytes, 0, configuration);
        }

        public static T Deserialize<T>(byte[] bytes, int offset, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var reader = new JsonReaderUtf8(bytes, offset);
            return configuration.Deserialize<T>(reader);
        }

        public static T Deserialize<T>(JsonReader reader, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();
            return configuration.Deserialize<T>(reader);
        }

        public static T Deserialize<T>(Stream stream, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

#if NETSTANDARD && !NET45
            var ms = stream as MemoryStream;
            if (ms != null) {
                ArraySegment<byte> buf2;
                if (ms.TryGetBuffer(out buf2)) {
                    // when token is number, can not use from pool(can not find end line).
                    var token = new JsonReaderUtf8(buf2.Array, buf2.Offset).GetCurrentJsonToken();
                    if (token == JsonToken.Number) {
                        var buf3 = new byte[buf2.Count];
                        Buffer.BlockCopy(buf2.Array!, buf2.Offset, buf3, 0, buf3.Length);
                        return Deserialize<T>(buf3, 0, configuration);
                    }

                    return Deserialize<T>(buf2.Array!, buf2.Offset, configuration);
                }
            }
#endif
            {
                var buf = MemoryPool.GetBuffer();
                var len = FillFromStream(stream, ref buf);

                // when token is number, can not use from pool(can not find end line).
                var token = new JsonReaderUtf8(buf).GetCurrentJsonToken();
                if (token == JsonToken.Number) {
                    buf = ByteArrayUtil.FastCloneWithResize(buf, len);
                }

                return Deserialize<T>(buf, configuration);
            }
        }

        public static async System.Threading.Tasks.Task<T> DeserializeAsync<T>(Stream stream, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buffer = BufferPool.Default.Rent();
            var buf = buffer;
            try {
                int length = 0;
                int read;
                while ((read = await stream.ReadAsync(buf, length, buf.Length - length).ConfigureAwait(false)) > 0) {
                    length += read;
                    if (length == buf.Length) {
                        ByteArrayUtil.FastResize(ref buf, length * 2);
                    }
                }

                // when token is number, can not use from pool(can not find end line).
                var token = new JsonReaderUtf8(buf).GetCurrentJsonToken();
                if (token == JsonToken.Number) {
                    buf = ByteArrayUtil.FastCloneWithResize(buf, length);
                }

                return Deserialize<T>(buf, configuration);
            } finally {
                BufferPool.Default.Return(buffer);
            }
        }

        static int FillFromStream(Stream input, ref byte[] buffer) {
            int length = 0;
            int read;
            while ((read = input.Read(buffer, length, buffer.Length - length)) > 0) {
                length += read;
                if (length == buffer.Length) {
                    ByteArrayUtil.FastResize(ref buffer, length * 2);
                }
            }

            return length;
        }

        static class MemoryPool {
            [ThreadStatic]
            static byte[]? buffer = null;

            public static byte[] GetBuffer() {
                if (buffer == null) {
                    return buffer = new byte[65536];
                } else {
                    return buffer;
                }
            }
        }
    }
}
#endif


#if no
namespace Brimborium.Json {
    /// <summary>
    /// EntryPoint of Brimborium.Json.
    /// </summary>
    public static partial class JsonSerializer {
        private static JsonSerializationConfiguration? _DefaultConfiguration;

        public static readonly List<IJsonFormatterResolver> DefaultJsonFormatterResolvers = new List<IJsonFormatterResolver>();
        public static readonly List<IJsonFormatter> DefaultJsonFormatters = new List<IJsonFormatter>();

       


        private static JsonWriterUtf8? _JsonWriterUtf8;

        /// <summary>
        /// Serialize to binary with default or specified configuration.
        /// </summary>
        public static byte[] Serialize<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            //var writer = System.Threading.Interlocked.Exchange(ref _JsonWriterUtf8, null)
            //    ?? new JsonWriterUtf8(MemoryPool.GetBuffer());
            //configuration.Serialize(writer, value);
            //var result = writer.ToUtf8ByteArray();
            //if (writer.ownBuffer) {
            //    System.Threading.Interlocked.CompareExchange(ref _JsonWriterUtf8, writer.Reset(), null);
            //}
            //return result;

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize<T>(writer, value);
                return writer.ToUtf8ByteArray();
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

        public static void Serialize<T>(JsonWriter writer, T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();
            configuration.Serialize(writer, value);
        }

        /// <summary>
        /// Serialize to stream with specified resolver.
        /// </summary>
        public static void Serialize<T>(Stream stream, T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize<T>(writer, value);
                var buffer = writer.GetBuffer();
                stream.Write(buffer.Array!, buffer.Offset, buffer.Count);
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

#if false
        /// <summary>
        /// Serialize to stream(write async).
        /// </summary>
        public static System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, T value) {
            return SerializeAsync<T>(stream, value, JsonSerializer.DefaultResolver);
        }

        /// <summary>
        /// Serialize to stream(write async) with specified resolver.
        /// </summary>
        public static async System.Threading.Tasks.Task SerializeAsync<T>(Stream stream, T value, IJsonFormatterResolver resolver) {
            if (resolver == null) {
                resolver = JsonSerializer.DefaultResolver;
            }

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                var formatter = resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(writer, value, resolver);
                var buffer = writer.GetBuffer();
                await stream.WriteAsync(buffer.Array!, buffer.Offset, buffer.Count).ConfigureAwait(false);
            } finally {
                BufferPool.Default.Return(buf);
            }
        }
#endif

        /// <summary>
        /// Serialize to binary with specified resolver. Get the raw memory pool byte[]. 
        /// The result can not share across thread and can not hold, so use quickly.
        /// </summary>
        public static ArraySegment<byte> SerializeUnsafe<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var writer = System.Threading.Interlocked.Exchange(ref _JsonWriterUtf8, null)
             ?? new JsonWriterUtf8(MemoryPool.GetBuffer());
            configuration.Serialize<T>(writer, value);
            var result = writer.GetBuffer();
            if (writer.ownBuffer) {
                System.Threading.Interlocked.CompareExchange(ref _JsonWriterUtf8, writer, null);
            }
            return result;
        }

        /// <summary>
        /// Serialize to JsonString with specified resolver.
        /// </summary>
        public static string ToJsonString<T>(T value, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buf = BufferPool.Default.Rent();
            try {
                var writer = new JsonWriterUtf8(buf);
                configuration.Serialize(writer, value);
                return writer.ToString();
            } finally {
                BufferPool.Default.Return(buf);
            }
        }

        public static T Deserialize<T>(string json, [AllowNull] JsonSerializationConfiguration configuration = default) {
            return Deserialize<T>(StringEncoding.UTF8NoBOM.GetBytes(json), configuration);
        }

        public static T Deserialize<T>(byte[] bytes, [AllowNull] JsonSerializationConfiguration configuration = default) {
            return Deserialize<T>(bytes, 0, configuration);
        }

        public static T Deserialize<T>(byte[] bytes, int offset, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var reader = new JsonReaderUtf8(bytes, offset);
            return configuration.Deserialize<T>(reader);
        }

        public static T Deserialize<T>(JsonReader reader, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();
            return configuration.Deserialize<T>(reader);
        }

        public static T Deserialize<T>(Stream stream, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

#if NETSTANDARD && !NET45
            var ms = stream as MemoryStream;
            if (ms != null) {
                ArraySegment<byte> buf2;
                if (ms.TryGetBuffer(out buf2)) {
                    // when token is number, can not use from pool(can not find end line).
                    var token = new JsonReaderUtf8(buf2.Array, buf2.Offset).GetCurrentJsonToken();
                    if (token == JsonToken.Number) {
                        var buf3 = new byte[buf2.Count];
                        Buffer.BlockCopy(buf2.Array!, buf2.Offset, buf3, 0, buf3.Length);
                        return Deserialize<T>(buf3, 0, configuration);
                    }

                    return Deserialize<T>(buf2.Array!, buf2.Offset, configuration);
                }
            }
#endif
            {
                var buf = MemoryPool.GetBuffer();
                var len = FillFromStream(stream, ref buf);

                // when token is number, can not use from pool(can not find end line).
                var token = new JsonReaderUtf8(buf).GetCurrentJsonToken();
                if (token == JsonToken.Number) {
                    buf = ByteArrayUtil.FastCloneWithResize(buf, len);
                }

                return Deserialize<T>(buf, configuration);
            }
        }

        public static async System.Threading.Tasks.Task<T> DeserializeAsync<T>(Stream stream, [AllowNull] JsonSerializationConfiguration configuration = default) {
            configuration ??= JsonSerializer.GetDefaultConfiguration();

            var buffer = BufferPool.Default.Rent();
            var buf = buffer;
            try {
                int length = 0;
                int read;
                while ((read = await stream.ReadAsync(buf, length, buf.Length - length).ConfigureAwait(false)) > 0) {
                    length += read;
                    if (length == buf.Length) {
                        ByteArrayUtil.FastResize(ref buf, length * 2);
                    }
                }

                // when token is number, can not use from pool(can not find end line).
                var token = new JsonReaderUtf8(buf).GetCurrentJsonToken();
                if (token == JsonToken.Number) {
                    buf = ByteArrayUtil.FastCloneWithResize(buf, length);
                }

                return Deserialize<T>(buf, configuration);
            } finally {
                BufferPool.Default.Return(buffer);
            }
        }

        static int FillFromStream(Stream input, ref byte[] buffer) {
            int length = 0;
            int read;
            while ((read = input.Read(buffer, length, buffer.Length - length)) > 0) {
                length += read;
                if (length == buffer.Length) {
                    ByteArrayUtil.FastResize(ref buffer, length * 2);
                }
            }

            return length;
        }

        static class MemoryPool {
            [ThreadStatic]
            static byte[]? buffer = null;

            public static byte[] GetBuffer() {
                if (buffer == null) {
                    return buffer = new byte[65536];
                } else {
                    return buffer;
                }
            }
        }
    }
}
#endif