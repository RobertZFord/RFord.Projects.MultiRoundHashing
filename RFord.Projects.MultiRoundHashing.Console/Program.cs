using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RFord.Projects.MultiRoundHashing.Core.DependencyInjection;

namespace RFord.Projects.MultiRoundHashing.Console
{
    internal class Program
    {
        static async Task Main(string[] args) =>
            await Host
                    .CreateDefaultBuilder(args)
                    .ConfigureLogging(loggingOption => loggingOption.ClearProviders())
                    .ConfigureServices(
                        (context, services) => {
                            services.AddCoreComponents();
                            services.AddHostedService<MainLoop>();
                        }
                    )
                    .UseConsoleLifetime()
                    .Build()
                    .RunAsync()
                    ;
    }
}