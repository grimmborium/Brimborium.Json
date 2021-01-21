namespace Brimborium.Json {
    public class JsonParserUtf8 {
        public void Parse(BoundedByteArray src, ref JsonToken jsonToken) {
            var c=src.Current;
        }
    }
    public class JsonParserUtf16 {
        public void Parse(BoundedCharArray src, ref JsonToken jsonToken) {
        }
    }
}
