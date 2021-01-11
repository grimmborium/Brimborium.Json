using System.Threading.Tasks;

using Xunit;
namespace Brimborium.Disguise.CompileTime {
    public class WorkspaceUtilityTest {
        [Fact]
        public async Task WorkspaceUtility_OpenSolution() {
            BuildUtility.InitLocator(null);
            ContextDisguise contextDisguise = new ContextDisguise();
            var sut = WorkspaceUtility.Create(null);
            var solution = await sut.OpenSolutionAsync(
                SolutionFolderPath.GetSolutionItemPath(@"sample\SampleApp1.sln"),
                contextDisguise,
                default);
            Assert.NotNull(solution);
            Assert.True(contextDisguise.Assemblies.Count > 2);
            foreach (var assembly in contextDisguise.Assemblies.Values) {
                if (assembly is AssemblyCTDisguise assemblyCT) {
                    var compilation = await assemblyCT.GetCompilationAsync();
                    
                }
            }
            foreach (var assembly in contextDisguise.Assemblies.Values) {
                if (assembly is AssemblyCTDisguise assemblyCT) {
                    assemblyCT.Project.n
                }
            }

                //solution.Workspace.CurrentSolution.GetProjectDependencyGraph()
                //var project = await sut.OpenProjectAsync(SolutionFolderPath.GetSolutionItemPath(@"sample\SampleLibrary1\SampleLibrary1.csproj"), default);
                //Assert.NotNull(project);
                //var assembly = new AssemblyCTDisguise(project, contextDisguise);
                //var projectCompilation = await project.GetCompilationAsync(default);
                //Assert.NotNull(projectCompilation);
                //projectCompilation.GlobalNamespace
                //var syntaxTrees = projectCompilation.SyntaxTrees;
                //var semanticModel = projectCompilation.GetSemanticModel(syntaxTrees, false);

            }
        [Fact]
        public async Task WorkspaceUtility_OpenProject() {
            BuildUtility.InitLocator(null);
            ContextDisguise contextDisguise = new ContextDisguise();
            var sut = WorkspaceUtility.Create(null);
            var project = await sut.OpenProjectAsync(
                SolutionFolderPath.GetSolutionItemPath(@"sample\SampleLibrary1\SampleLibrary1.csproj"),
                contextDisguise,
                default);
            Assert.NotNull(project);
            Assert.True(contextDisguise.Assemblies.Count == 1);
            //var assembly = new AssemblyCTDisguise(project, contextDisguise);
            //var projectCompilation = await project.GetCompilationAsync(default);
            //Assert.NotNull(projectCompilation);
            //projectCompilation.GlobalNamespace
            //var syntaxTrees = projectCompilation.SyntaxTrees;
            //var semanticModel = projectCompilation.GetSemanticModel(syntaxTrees, false);

        }
    }
}