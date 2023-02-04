using Microsoft.Extensions.DependencyInjection;
using RFord.Projects.MultiRoundHashing.Core;
using RFord.Projects.MultiRoundHashing.Core.DependencyInjection;
using RFord.Projects.MultiRoundHashing.Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace RFord.Projects.MultiRoundHashing.Tests
{

    public class FullTests
    {
        private readonly IServiceProvider _services;

        public FullTests()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();

            // this needs to be added before the mock file provider
            serviceDescriptors.AddCoreComponents();

            serviceDescriptors.AddTransient(_ => MockFileSystemAccess.GetMockFileSystemObject());

            _services = serviceDescriptors.BuildServiceProvider(new ServiceProviderOptions {  ValidateOnBuild= true, ValidateScopes = true });
        }

        [Theory]
        [InlineData(HashSource.File, MockFileSystemAccess.SimpleFileName, "8OTC92xYkW7CWPJGhRvqCR0U1CR6L8PhhpRGGxgW4Ts=")]
        [InlineData(HashSource.Text, "ⶼⱳⲈ⛔☶⾬░ⴽ◳⎀⊢⬍ⴕⅡ⪖⽯", "Stt5vWyOgxXW4HLJddGfXhHELNh0L9O12xyX0tnTtno=")]
        public void SingleSha256Round(HashSource hashSource, string hashSourceData, string expected)
        {
            IDataProvider dataProvider = _services.GetRequiredService<IDataProvider>();
            IMultiRoundHasher multiRoundHasher = _services.GetRequiredService<IMultiRoundHasher>();

            using (Stream stream = dataProvider.GetStream(hashSource, hashSourceData))
            {
                byte[] hashBytes = multiRoundHasher.ComputeHash(stream, SHA256.Create(), 1);
                Assert.Equal(
                    expected: Convert.FromBase64String(expected),
                    actual: hashBytes
                );
            }
        }
    }
}