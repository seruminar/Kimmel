using Functions;

using Kimmel.Activation.Kontent.DependencyInjection;
using Kimmel.Core;
using Kimmel.Core.Kontent;
using Kimmel.Export.Zip.DependencyInjection;
using Kimmel.Kontent;
using Kimmel.Parsing.DependencyInjection;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Functions
{
    /// <summary>
    /// Runs when the Azure Functions host starts.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder functionsHostBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var settings = new Settings();

            ConfigurationBinder.Bind(configuration, settings);

            functionsHostBuilder.Services
                .AddSingleton(_ => settings);

            functionsHostBuilder.Services
                .AddLogging();

            var applicationRootPath = functionsHostBuilder.GetContext().ApplicationRootPath;

            functionsHostBuilder.Services
                .AddKimmel(applicationRootPath)
                .AddKimmelKontentActivation()
                .AddKimmelExport()
                .AddSingleton(_ => settings);

            functionsHostBuilder.Services
                .AddSingleton<IKontentApiTracker, KontentApiTracker>()
                .AddSingleton<IKontentRateLimiter, KontentRateLimiter>()
                .AddHttpClient<IKontentStore, KontentStore>();
        }
    }
}