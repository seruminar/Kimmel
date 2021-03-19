using Kimmel.Core.Export;

using Microsoft.Extensions.DependencyInjection;

namespace Kimmel.Export.Zip.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKimmelExport(this IServiceCollection services)
        {
            services
                .AddSingleton<IZipper, Zipper>();

            return services;
        }
    }
}