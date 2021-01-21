namespace Brimborium.Json {
    public class JsonReaderContext {
        //public bool TokensAvailible;
        //public bool ContentSplit;
        public int IndexToken;
        public int CountToken;
        public JsonToken[] Tokens;
        public BoundedByteArray SourceByteArray;
        public BoundedCharArray SourceCharArray;

        public BoundedByteArray BufferByteArray;
        public BoundedCharArray BufferCharArray;

        public JsonReaderContext() {
            this.Tokens = new JsonToken[4096];
            this.SourceByteArray = BoundedByteArray.Empty();
            this.SourceCharArray = BoundedCharArray.Empty();
            this.BufferByteArray = BoundedByteArray.Empty();
            this.BufferCharArray = BoundedCharArray.Empty();
            this.SaveStateUtf8 = JsonReaderContextStateUtf8.Start();
            this.IndexToken = 0;
            this.CountToken = 0;
        }

        public JsonReaderContextStateUtf8 SaveStateUtf8;
    }
}

