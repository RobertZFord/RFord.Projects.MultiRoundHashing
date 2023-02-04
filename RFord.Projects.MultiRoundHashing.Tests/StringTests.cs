using Microsoft.Extensions.DependencyInjection;
using RFord.Projects.MultiRoundHashing.Core.Services;

namespace RFord.Projects.MultiRoundHashing.Tests
{
    public class StringTests
    {
        private readonly IFileSystemAccess _resolvers;

        public StringTests()
        {
            // a `new FileSystemAccess()` would work, but I think I would like
            // to mimic the service injection as close as I possibly can
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<IFileSystemAccess, FileSystemAccess>();
            IServiceProvider _services = serviceDescriptors.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

            _resolvers = _services.GetRequiredService<IFileSystemAccess>();
        }


        [Theory]
        [InlineData("C:\\Windows\\notepad.exe", @"C:\Windows\notepad.exe")]
        [InlineData("\"C:\\Program Files (x86)\\Windows Defender\\MpCmdRun.exe\"", @"C:\Program Files (x86)\Windows Defender\MpCmdRun.exe")]
        public void ValidFilePaths(string filePath, string expected)
        {
            Assert.True(_resolvers.TryCleanFilePath(filePath, out string cleanedFilePath));
            Assert.Equal(expected, cleanedFilePath);
        }

        [Theory]
        [InlineData("\"C:\\Program Files (x86)\\Windows Defender\\MpCmdRun.exe")]
        [InlineData("C:\\Program Files (x86)\\Windows Defender\\MpCmdRun.exe\"")]
        public void InvalidFilePaths(string filePath)
        {
            Assert.False(_resolvers.TryCleanFilePath(filePath, out _));
        }
    }
}