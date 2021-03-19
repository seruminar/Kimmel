using System.IO;

using Kimmel.Core.Models;
using Kimmel.Core.Parsing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kimmel.Parsing.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKimmel(this IServiceCollection services, string applicationRootPath)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(applicationRootPath, "kimmel.json"))
                .Build();

            var options = new Options();

            ConfigurationBinder.Bind(configuration, options);

            services
                .AddSingleton(_ => options)
                .AddTransient<IKmlParser, KmlParser>()
                .AddTransient<IPropertyDescriber, PropertyDescriber>()
                .AddTransient<ITypeDescriber, TypeDescriber>();

            return services;
        }
    }
}