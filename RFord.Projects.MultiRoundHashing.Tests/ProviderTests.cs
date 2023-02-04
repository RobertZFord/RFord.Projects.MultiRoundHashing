using Microsoft.Extensions.DependencyInjection;
using Moq;
using RFord.Projects.MultiRoundHashing.Core;
using RFord.Projects.MultiRoundHashing.Core.Services;

namespace RFord.Projects.MultiRoundHashing.Tests
{

    public class ProviderTests
    {
        private readonly IDataProvider _dataProvider;

        public ProviderTests()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();

            serviceDescriptors.AddTransient(_ => MockFileSystemAccess.GetMockFileSystemObject());

            serviceDescriptors.AddTransient<IDataProvider, DataProvider>();
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true
                }
            );
            _dataProvider = serviceProvider.GetRequiredService<IDataProvider>();
        }

        [Theory]
        [InlineData(HashSource.File, MockFileSystemAccess.SimpleFileName, "YXNkZg==")]
        [InlineData(HashSource.Text, "this is a simple test string", "dABoAGkAcwAgAGkAcwAgAGEAIABzAGkAbQBwAGwAZQAgAHQAZQBzAHQAIABzAHQAcgBpAG4AZwA=")]
        public void ProviderInputs(HashSource selectedSourceType, string hashSource, string expectedValue)
        {
            Stream providerStream = _dataProvider.GetStream(selectedSourceType, hashSource);
            BinaryReader br = new BinaryReader(providerStream);
            byte[] arr = br.ReadBytes((int)providerStream.Length);

            Assert.Equal(
                expected: Convert.FromBase64String(expectedValue),
                actual: arr
            );
        }
    }
}