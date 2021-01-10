using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;

namespace Brimborium.Disguise.CompileTime {
    public class Utilities {
    }
    public class WorkspaceUtility {
        public WorkspaceUtility(string? msbuildPath, Dictionary<string, string>? properties) {
            this.MsbuildPath = msbuildPath;
            this.Properties = properties ?? new Dictionary<string, string>();
        }

        public ILogger Logger { get; }
        public string? MsbuildPath { get; }
        public Dictionary<string, string> Properties { get; }
        public MSBuildWorkspace? Workspace { get; private set; }

        public MSBuildWorkspace Init() {
            //if (msbuildPath != null) {
            //    Microsoft.Build.Locator.MSBuildLocator.RegisterMSBuildPath(msbuildPath);
            //}
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

        public async Task<Solution> OpenSolutionAsync(string solutionFilePath, CancellationToken cancellationToken) {
            Solution solution = await this.Workspace.OpenSolutionAsync(solutionFilePath, cancellationToken: cancellationToken);
            return solution;
        }

        public async Task<Project> OpenProjectAsync(string projectFilePath, CancellationToken cancellationToken) {
            Project project = await this.Workspace.OpenProjectAsync(projectFilePath, cancellationToken: cancellationToken);
            return project;
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
