using Microsoft.Extensions.DependencyInjection;
using RFord.Projects.MultiRoundHashing.Core.Services;
using System.Security.Cryptography;
using System.Text;

namespace RFord.Projects.MultiRoundHashing.Tests
{
    //  https://learn.microsoft.com/en-us/dotnet/core/testing/
    //  https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0
    //  https://jeremydmiller.com/2021/09/07/integration-testing-ihost-lifecycle-with-xunit-net/
    //  https://www.c-sharpcorner.com/article/unit-testing-using-xunit-and-moq-in-asp-net-core/
    //  https://medium.com/swlh/testing-an-asp-net-core-service-with-xunit-f18225d9b22a
    public class RoundTests
    {
        private readonly IServiceProvider _services;
        private readonly IMultiRoundHasher _multiRoundHasher;

        public RoundTests()
        {
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddTransient<IMultiRoundHasher, MultiRoundHasher>();
            _services = serviceDescriptors.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
            _multiRoundHasher = _services.GetRequiredService<IMultiRoundHasher>();
        }

        [Fact]
        public void SimpleSingleRound()
        {
            byte[] result = _multiRoundHasher.ComputeHash(
                new MemoryStream(Array.Empty<byte>()),
                SHA1.Create(),
                1
            );

            // DA39A3EE5E6B4B0D3255BFEF95601890AFD80709
            Assert.Equal(
                expected: Convert.FromBase64String("2jmj7l5rSw0yVb/vlWAYkK/YBwk="),
                actual: result
            );
        }

        [Fact]
        public void ComplexMultipleRounds()
        {
            byte[] result = _multiRoundHasher.ComputeHash(
                new MemoryStream(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }),
                SHA256.Create(),
                10_000_000
            );

            // 6E45253C55979B50FECFA7E884587FF50267FF18B7D5C00823B0FCA9A551D5D7
            Assert.Equal(
                expected: Convert.FromBase64String("bkUlPFWXm1D+z6fohFh/9QJn/xi31cAII7D8qaVR1dc="),
                actual: result
            );
        }
    }
}