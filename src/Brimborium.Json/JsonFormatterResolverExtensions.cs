#nullable enable

using System;

namespace Brimborium.Json {
    public static class JsonFormatterResolverExtensions {
        /*
        public static IJsonSerializer<T, TJsonWriter> GetSerializerWithVerify<T, TJsonWriter>(this JsonSerializationConfiguration configuration)
            where TJsonWriter : JsonWriter {
            IJsonSerializer<T, TJsonWriter> jsonSerializer;
            try {
                jsonSerializer = configuration.GetSerializer<T, TJsonWriter>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (jsonSerializer == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " Serializer is not registered.");
            }

            return jsonSerializer;
        }
        public static IJsonDeserializer<T, TJsonReader> GetDeserializerWithVerify<T, TJsonReader>(this JsonSerializationConfiguration configuration)
            where TJsonReader : JsonReader {
            IJsonDeserializer<T, TJsonReader> jsonDeserializer;
            try {
                jsonDeserializer = configuration.GetDeserializer<T, TJsonReader>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (jsonDeserializer == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " Deserializer is not registered.");
            }

            return jsonDeserializer;
        }
        */
        public static IJsonFormatter<T> GetFormatterWithVerify<T>(this IJsonFormatterResolver resolver) {
            IJsonFormatter<T> formatter;
            try {
                formatter = resolver.GetFormatter<T>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (formatter == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name);
            }

            return formatter;
        }

        public static object GetFormatterDynamic(this IJsonFormatterResolver resolver, Type type) {
            //var methodInfo = typeof(IJsonFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);

            //var formatter = methodInfo.MakeGenericMethod(type).Invoke(resolver, null);
            //return formatter;
#warning Remove GetFormatterDynamic
            throw new NotImplementedException();
        }
    }
}