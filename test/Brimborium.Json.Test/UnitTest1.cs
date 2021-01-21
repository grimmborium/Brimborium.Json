using System;
using System.Text;

using Xunit;

namespace Brimborium.Json.Test {
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var sb = new StringBuilder();
            for (byte b = 1; b <= 127; b++) {
                char c = (char)b;
                if (c == '\r') {
                    sb.AppendLine($"case {b}: /* \\r */");
                } else if (c == '\n') {
                    sb.AppendLine($"case {b}: /* \\n */");
                } else if (c == '\t') {
                    sb.AppendLine($"case {b}: /* \\t */");
                } else if (c <= 31) {
                    sb.AppendLine($"case {b}:");
                } else { 
                    sb.AppendLine($"case {b}: /* {c} */");
                }
                sb.AppendLine($"break;");
            }
            Assert.Equal("a", sb.ToString());
        }
    }
}
