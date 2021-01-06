#nullable disable

using Brimborium.Json.Internal;
using Brimborium.Json.Resolvers;

namespace Brimborium.Json {
    /// <summary>
    /// EntryPoint of Brimborium.Json.
    /// </summary>
    public static partial class Serializer {
        static IJsonFormatterResolver defaultResolver;

        /// <summary>
        /// FormatterResolver that used resolver less overloads. If does not set it, used StandardResolver.Default.
        /// </summary>
        public static IJsonFormatterResolver DefaultResolver {
            get {
                if (defaultResolver == null) {
                    defaultResolver = StandardResolver.Default;
                }

                return defaultResolver;
            }
        }

        /// <summary>
        /// Is resolver decided?
        /// </summary>
        public static bool IsInitialized {
            get {
                return defaultResolver != null;
            }
        }


        /// <summary>
        /// Set default resolver of Utf8Json APIs.
        /// </summary>
        /// <param name="resolver"></param>
        public static void SetDefaultResolver(IJsonFormatterResolver resolver) {
            defaultResolver = resolver;
        }
    }
}
