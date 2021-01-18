using Xunit;

namespace Brimborium.Disguise {
    public class SolutionFolderPathTest {
        [Fact]
        public void SolutionFolderPathTest1() {
            Assert.Equal("Brimborium.Json", System.IO.Path.GetFileName(SolutionFolderPath.GetSolutionFolderPath()));
        }
    }
}