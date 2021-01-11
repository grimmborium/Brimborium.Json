using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Linq;

namespace Brimborium.Disguise.CompileTime {
    public class BuildUtility {
        public static void InitLocator(string? msbuildPath) {
            try {
                if (!string.IsNullOrEmpty(msbuildPath)) {
                    Microsoft.Build.Locator.MSBuildLocator.RegisterMSBuildPath(msbuildPath);
                } else {
                    Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
                }
            } catch {
            }
        }
    }
    public class WorkspaceUtility {


        public static WorkspaceUtility Create(Dictionary<string, string>? properties) {
            var result = new WorkspaceUtility(properties);
            result.InitWorkspace();
            return result;
        }

        public WorkspaceUtility(Dictionary<string, string>? properties) {
            this.Properties = properties ?? new Dictionary<string, string>();
            this.Projects = new List<Project>();
        }
        public string? MsBuildPath { get; }
        public Dictionary<string, string> Properties { get; }

        public MSBuildWorkspace? Workspace { get; private set; }
        public Solution? Solution { get; private set; }
        public List<Project> Projects { get; }


        public MSBuildWorkspace InitWorkspace() {
            if (this.Workspace is object) {
                throw new InvalidOperationException("Workspace is already set.");
            }

            // else if (TryGetSingleInstance(out VisualStudioInstance instance)) {
            //     MSBuildLocator.RegisterInstance(instance);
            //     msbuildPath = instance.MSBuildPath;
            // } else {
            //     return null;
            // }
            // WriteLine($"MSBuild location is '{msbuildPath}'", Verbosity.Diagnostic);
            // if (!ParseHelpers.TryParseMSBuildProperties(rawProperties, out Dictionary<string, string> properties)) { return null; }

            // https://github.com/Microsoft/MSBuildLocator/issues/16
            if (!this.Properties.ContainsKey("AlwaysCompileMarkupFilesInSeparateDomain")) {
                this.Properties["AlwaysCompileMarkupFilesInSeparateDomain"] = bool.FalseString;
            }

            var result = MSBuildWorkspace.Create(this.Properties);
            this.Workspace = result;
            return result;
        }

        public async Task<Solution> OpenSolutionAsync(string solutionFilePath, ContextDisguise contextDisguise, CancellationToken cancellationToken) {
            if (this.Workspace is null) {
                throw new InvalidOperationException("Init not called.");
            }
            Solution solution = await this.Workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cancellationToken);
            this.Solution = solution;
            await AddProjectsAsync(solution, contextDisguise);
            return solution;
        }

        public async Task<Project?> OpenProjectAsync(string projectFilePath, ContextDisguise contextDisguise, CancellationToken cancellationToken) {
            if (this.Workspace is null) {
                throw new InvalidOperationException("Init not called.");
            }
            if (this.Solution is null) {
                Project project = await this.Workspace.OpenProjectAsync(projectFilePath, cancellationToken: cancellationToken);
                this.Solution = project.Solution;
                await AddProjectsAsync(this.Solution, contextDisguise);
                return project;
            } else {
                Project? project = this.Solution.Projects.FirstOrDefault(p => string.Equals(p.FilePath, projectFilePath, StringComparison.InvariantCultureIgnoreCase));

                if (project is null) {
                    project = this.Solution.Projects.FirstOrDefault(p => string.Equals(p.Name, projectFilePath, StringComparison.InvariantCultureIgnoreCase));
                }

                if (project is object) {
                    await AddProjectAsync(project, contextDisguise);
                }

                return project;
            }
        }

        private  async Task AddProjectsAsync(Solution solution, ContextDisguise contextDisguise) {
            foreach (var project in solution.Projects) {
                await AddProjectAsync(project, contextDisguise);
            }
        }

        private static async Task AddProjectAsync(Project project, ContextDisguise contextDisguise) {
            var assembly = new AssemblyCTDisguise(project, contextDisguise);
            // var compilation = await project.GetCompilationAsync(cancellationToken);
            var assemblyIdentity = new AssemblyIdentity(project.AssemblyName);
            if (contextDisguise.TryGetAssembly(assemblyIdentity, out var assemblyFound)) {
                if (assemblyFound is AssemblyCTDisguise assemblyCTDisguise) {
                    if (ReferenceEquals(assemblyCTDisguise.Project, project)) {
                        return;
                    } else {
                        assembly.Compilation = await assembly.Project.GetCompilationAsync(default);
                    }
                }
            } else {
                assembly.Compilation = await assembly.Project.GetCompilationAsync(default);
            }
            contextDisguise.Assemblies[assemblyIdentity] = assembly;
        }

#if false
        private static bool TryGetSingleInstance(out VisualStudioInstance instance) {
            using (IEnumerator<VisualStudioInstance> en = MSBuildLocator.QueryVisualStudioInstances().GetEnumerator()) {
                if (!en.MoveNext()) {
                    // WriteLine($"MSBuild location not found. Use option '--{ParameterNames.MSBuildPath}' to specify MSBuild location", Verbosity.Quiet);
                    instance = null;
                    return false;
                }

                VisualStudioInstance firstInstance = en.Current;

                if (en.MoveNext()) {
                    // WriteLine("Multiple MSBuild locations found:", Verbosity.Quiet);
                    // WriteLine($"  {firstInstance.MSBuildPath}", Verbosity.Quiet);
                    //do {
                    //    WriteLine($"  {en.Current.MSBuildPath}", Verbosity.Quiet);
                    //} while (en.MoveNext());
                    //WriteLine($"Use option '--{ParameterNames.MSBuildPath}' to specify MSBuild location", Verbosity.Quiet);
                    instance = null;
                    return false;
                }

                instance = firstInstance;
                return true;
            }
        }
#endif
    }
}
