using System.Text;

namespace RFord.Projects.MultiRoundHashing.Core.Services
{
    internal class DataProvider : IDataProvider
    {
        private readonly IFileSystemAccess _fileSystemAccess;

        public DataProvider(
            IFileSystemAccess fileSystemAccess
        )
        {
            _fileSystemAccess = fileSystemAccess;
        }

        public Stream GetStream(HashSource selectedSourceType, string hashSource)
        {
            return selectedSourceType switch
            {
                HashSource.File
                    => _fileSystemAccess.ReadFile(hashSource),
                HashSource.Text
                    => new MemoryStream(Encoding.Unicode.GetBytes(hashSource)),
                _
                    => throw new InvalidOperationException()
            };
        }
    }
}
