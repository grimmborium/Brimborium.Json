#nullable enable

//
namespace Brimborium.Json {
    public static class JsonConstText {
        public static readonly JsonText True = new JsonText("true", false);
        public static readonly JsonText False = new JsonText("false", false);
        public static readonly JsonText ObjectOpen = new JsonText("{", false);
        public static readonly JsonText ObjectClose = new JsonText("}", false);
        public static readonly JsonText StringQuote = new JsonText("\"", false);
        public static readonly JsonText EscapedQuote = new JsonText("\\\"", false);
        public static readonly JsonText EscapedN = new JsonText("\\\n", false);
        public static readonly JsonText N = new JsonText("\n", false);
        public static readonly JsonText EscapedR = new JsonText("\\\r", false);
        public static readonly JsonText R = new JsonText("\r", false);
        public static readonly JsonText EscapedT = new JsonText("\\\t", false);
        public static readonly JsonText T = new JsonText("\t", false);
        public static readonly JsonText KeyValueSep = new JsonText(":", false);
        public static readonly JsonText ArrayOpen = new JsonText("[", false);
        public static readonly JsonText ArrayClose = new JsonText("]", false);
        public static readonly JsonText ArraySep = new JsonText(",", false);
        public static readonly JsonText Null = new JsonText(",", false);
    }
}