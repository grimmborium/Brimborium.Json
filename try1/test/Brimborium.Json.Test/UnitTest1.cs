using System;

using Xunit;

namespace Brimborium.Json.Test {
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = Array.Empty<byte>();
            var b = Array.Empty<byte>();
            Assert.Same(a, b);
        }
    }
}
