using System;
using System.Reflection;
using Xunit;
using Brimborium.Disguise;

namespace Brimborium.Disguise.RunTime {
    public class TypeInfoRTDisguiseTest {
        [Fact]
        public void Test1()
        {
            var sut = new TypeInfoRTDisguise(typeof(Hugo.Gna));
            Assert.Equal("Gna", sut.Name);
        }
    }
}
