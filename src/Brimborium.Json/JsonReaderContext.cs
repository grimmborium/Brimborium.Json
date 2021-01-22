namespace Brimborium.Json {
    public class JsonReaderContext {
        //public bool TokensAvailible;
        //public bool ContentSplit;
        public int IndexToken;
        public int CountToken;
        public JsonToken[] Tokens;
        public BoundedByteArray BoundedByteArray;
        public BoundedCharArray BoundedCharArray;

        public bool FinalContent;

        public JsonReaderContext() {
            this.Tokens = new JsonToken[8];
            this.BoundedByteArray = BoundedByteArray.Empty();
            this.BoundedCharArray = BoundedCharArray.Empty();
            this.SaveStateUtf8 = JsonReaderContextStateUtf8.Start();
            this.IndexToken = 0;
            this.CountToken = 0;
        }

        public JsonReaderContextStateUtf8 SaveStateUtf8;
    }
}

