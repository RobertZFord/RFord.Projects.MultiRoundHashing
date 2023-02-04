namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    public interface IFileSystemAccess
    {
        Stream ReadFile(string filePath);
        bool FileExists(string filePath);
        bool TryCleanFilePath(string filePath, out string cleanPath);
    }
}
