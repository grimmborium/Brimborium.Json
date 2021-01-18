namespace Brimborium.Disguise {
    public static class SolutionFolderPath {
        private static string? _GetSolutionFolderPath;
        public static string GetSolutionFolderPath() {
            if (_GetSolutionFolderPath is null) {
                var result = GetCurrentLocation();
                result = System.IO.Path.GetDirectoryName(result);
                result = System.IO.Path.GetDirectoryName(result);
                result = System.IO.Path.GetDirectoryName(result);
                return _GetSolutionFolderPath = result;
            } else {
                return _GetSolutionFolderPath;
            }
        }

        public static string GetSolutionItemPath(string path)
            => System.IO.Path.Combine(GetSolutionFolderPath(), path);

        private static string GetCurrentLocation(
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
        ) {
            return sourceFilePath;
        }
    }
}
