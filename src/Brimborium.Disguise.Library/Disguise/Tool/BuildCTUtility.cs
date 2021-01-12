namespace Brimborium.Disguise.Tool {
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
}
