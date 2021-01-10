using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
    public class WorkspaceUtilityTest {
        [Fact]
        public async Task Test1() {
            var sut = new WorkspaceUtility(null, null);
            var project = await sut.OpenProjectAsync(SolutionFolderPath.GetSolutionItemPath(@"sample\SampleLibrary1\SampleLibrary1.csproj"), default);
            Assert.NotNull(project);
            var projectCompilation = await project.GetCompilationAsync(default);
            Assert.NotNull(projectCompilation);
            //var syntaxTrees = projectCompilation.SyntaxTrees;
            //var semanticModel = projectCompilation.GetSemanticModel(syntaxTrees, false);

        }
    }
}