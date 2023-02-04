namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    internal class FileSystemAccess : IFileSystemAccess
    {
        private readonly char[] _trimmables;

        public FileSystemAccess() {
            _trimmables = new char[] { ' ', '\n', '\r' };
        }

        public Stream ReadFile(string filePath) => File.OpenRead(filePath);

        public bool FileExists(string filePath) => File.Exists(filePath);

        // this *could* be a string extension method, however, I question the
        // rationale and discoverability of throwing yet ANOTHER method onto
        // `string`.
        // just counted.  right now `string` has 73 extension methods on it.
        // lol
        public bool TryCleanFilePath(string filePath, out string cleanFilePath)
        {
            cleanFilePath = filePath.Trim(_trimmables);

            if (cleanFilePath.StartsWith('"') ^ cleanFilePath.EndsWith('"'))
            {
                return false;
            }

            cleanFilePath = filePath.StartsWith('"') ? filePath.Substring(1, filePath.Length - 2) : cleanFilePath;

            cleanFilePath = cleanFilePath.Trim(_trimmables);

            return true;
        }
    }
}
