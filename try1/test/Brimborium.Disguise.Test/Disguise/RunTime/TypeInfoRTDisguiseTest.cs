using System;
using System.Reflection;
using Xunit;
using Brimborium.Disguise;

namespace Brimborium.Disguise.RunTime {
    public class TypeInfoRTDisguiseTest {
        [Fact]
        public void TypeInfoRTDisguise_Name1()
        {
            var contextDisguise = new ContextDisguise();
            var sut = new TypeRTDisguise(typeof(Hugo.Gna), contextDisguise);
            Assert.Equal("Gna", sut.Name);
        }

        [Fact]
        public void TypeInfoRTDisguise_Name2() {
            var sut = new TypeRTDisguise(typeof(Hugo.Gna), null);
            Assert.Equal("Gna", sut.Name);
        }
    }
}
