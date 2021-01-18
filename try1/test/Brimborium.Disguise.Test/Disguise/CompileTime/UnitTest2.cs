using System;
using System.Reflection;
using System.Threading;

using Xunit;
namespace Brimborium.Disguise.CompileTime {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            var a = Array.Empty<byte>();
            var b = Array.Empty<byte>();
            Assert.Same(a, b);
            /*
                G:\github\grimmborium\Brimborium.Json\sample\SampleLibrary1\SampleLibrary1.csproj
                G:\github\grimmborium\Brimborium.Json\sample\SampleLibrary2\SampleLibrary2.csproj
                G:\github\grimmborium\Brimborium.Json\sample\SampleLibrary2Specification\SampleLibrary2Specification.csproj
                G:\github\grimmborium\Brimborium.Json\sample\SampleApp1\SampleApp1.csproj
            */
        }
    }
}