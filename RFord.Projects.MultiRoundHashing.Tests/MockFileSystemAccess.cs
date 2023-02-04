using Moq;
using RFord.Projects.MultiRoundHashing.Core.Services;

namespace RFord.Projects.MultiRoundHashing.Tests
{
    public class MockFileSystemAccess
    {
        public const string SimpleFileName = "simple file.binary";

        private static readonly (string, byte[])[] _files = new (string, byte[])[]
        {
            (SimpleFileName, new byte[] { 0x61, 0x73, 0x64, 0x66 })
        };

        public static IFileSystemAccess GetMockFileSystemObject()
        {
            var fauxFileSystemAccess = new Mock<IFileSystemAccess>(MockBehavior.Strict);

            foreach (var file in _files)
            {
                fauxFileSystemAccess
                    .Setup(
                        x => x.ReadFile(file.Item1)
                    )
                    .Returns(
                        new MemoryStream(file.Item2)
                    );
            }

            return fauxFileSystemAccess.Object;
        }
    }
}