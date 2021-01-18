using Brimborium.Disguise.CompileTime;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Disguise.Tool {
    public partial class WorkspaceCTUtility {
        public static WorkspaceCTUtility Create(Dictionary<string, string>? properties) {
            var result = new WorkspaceCTUtility(properties);
            result.InitWorkspace();
            return result;
        }

        public WorkspaceCTUtility(Dictionary<string, string>? properties) {
            this.Properties = properties ?? new Dictionary<string, string>();
            this.Projects = new List<ProjectCTUtility>();
        }
        public string? MsBuildPath { get; }
        public Dictionary<string, string> Properties { get; }

        public MSBuildWorkspace? Workspace { get; private set; }
        public Solution? Solution { get; private set; }
        public List<ProjectCTUtility> Projects { get; }

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
            var topologicallySortedProjects = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects();
            foreach (var projectId in topologicallySortedProjects) {
                var project = solution.GetProject(projectId);
                if (project is object) {
                    var projectUtility = new ProjectCTUtility(project);
                    this.Projects.Add(projectUtility);
                    await AddProjectAsync(projectUtility, contextDisguise, cancellationToken);
                }
            }
            return solution;
        }

        public async Task<Project?> OpenProjectAsync(
            string projectFilePath,
            ContextDisguise contextDisguise,
            CancellationToken cancellationToken) {
            if (this.Workspace is null) {
                throw new InvalidOperationException("Init not called.");
            }
            if (this.Solution is null) {
                Project project = await this.Workspace.OpenProjectAsync(projectFilePath, cancellationToken: cancellationToken);
                this.Solution = project.Solution;
                var projectUtility = new ProjectCTUtility(project);
                this.Projects.Add(projectUtility);
                await AddProjectAsync(projectUtility, contextDisguise, cancellationToken);

                return project;
            } else {
                var projectUtility = this.Projects.FirstOrDefault(p => string.Equals(p.Project.FilePath, projectFilePath, StringComparison.InvariantCultureIgnoreCase));

                if (projectUtility is null) {
                    projectUtility = this.Projects.FirstOrDefault(p => string.Equals(p.Project.Name, projectFilePath, StringComparison.InvariantCultureIgnoreCase));
                }

                return projectUtility?.Project;
            }
        }

        private static async Task AddProjectAsync(
            ProjectCTUtility projectUtility,
            ContextDisguise contextDisguise,
            CancellationToken cancellationToken) {

            if (contextDisguise.TryGetAssembly(projectUtility.AssemblyIdentity, out var assemblyFound)) {
                if (assemblyFound is AssemblyCTDisguise assemblyCTDisguise) {
                    if (ReferenceEquals(assemblyCTDisguise.Project, projectUtility.Project)) {
                        return;
                    }
                } else {
                    return;
                }
            }
            {
                var assembly = new AssemblyCTDisguise(projectUtility.Project, contextDisguise);
                projectUtility.Assembly = assembly;
                await projectUtility.GetCompilationAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }


        public async Task<List<CodeUtility>> ProcessProjectsAsync(
            Func<ProjectCTUtility, bool>? predicateProject,
            Func<Compilation, bool>? predicateCompilation,
            ContextDisguise contextDisguise,
            Action<ContextDisguise, CodeUtility> process,
            CancellationToken cancellationToken) {
            var result = new List<CodeUtility>();
            foreach (var projectUtility in this.Projects) {
                if (predicateProject is null || predicateProject(projectUtility)) {
                    var projects = await projectUtility.ProcessProjectAsync(
                        predicateCompilation,
                        contextDisguise,
                        process,
                        cancellationToken);
                    if (projects.Count > 0) {
                        result.AddRange(projects);
                    }
                }
            }
            return result;
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
