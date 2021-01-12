
using Microsoft.CodeAnalysis;

namespace Brimborium.Disguise.Tool {
    public partial class WorkspaceCTUtility {
        public class CodeUtility {
            public readonly ProjectCTUtility ProjectUtility;
            public readonly SyntaxTree SyntaxTree;
            public readonly SemanticModel SemanticModel;
            public Compilation? Compilation => ProjectUtility.Compilation;

            public CodeUtility(
                    ProjectCTUtility projectUtility,
                    SyntaxTree syntaxTree,
                    SemanticModel semanticModel
                ) {
                this.ProjectUtility = projectUtility;
                this.SyntaxTree = syntaxTree;
                this.SemanticModel = semanticModel;
            }
        }
    }
}
