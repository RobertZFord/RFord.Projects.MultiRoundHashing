using Microsoft.Extensions.DependencyInjection;
using RFord.Projects.MultiRoundHashing.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFord.Projects.MultiRoundHashing.Core.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddTransient<IFileSystemAccess, FileSystemAccess>();
            services.AddTransient<IDataProvider, DataProvider>();
            services.AddTransient<IMultiRoundHasher, MultiRoundHasher>();
            return services;
        }
    }
}
