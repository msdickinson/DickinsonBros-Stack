using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BaseRunner
{
    [ExcludeFromCodeCoverage]
    public static class BaseRunnerSetup
    {
        public static IConfigurationRoot FetchConfiguration()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("BUILD_CONFIGURATION");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true);
            return builder.Build();
        }

        public static IServiceCollection InitializeDependencyInjection(IConfigurationRoot configurationRoot)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configurationRoot);
            services.AddOptions();
            services.AddLogging(config =>
            {
                config.AddConfiguration(configurationRoot.GetSection("Logging"));

                if (Environment.GetEnvironmentVariable("BUILD_CONFIGURATION") == "DEBUG")
                {
                    config.AddConsole();
                }
            });

            services.AddSingleton<IHostApplicationLifetime, HostApplicationLifetime>();

            return services;
        }
    }
}
