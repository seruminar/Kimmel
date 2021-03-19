using Kimmel.Core.Activation.Kontent;

using Microsoft.Extensions.DependencyInjection;

namespace Kimmel.Activation.Kontent.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKimmelKontentActivation(this IServiceCollection services)
        {
            services
                .AddSingleton<ITypeActivator, KontentTypeActivator>();

            return services;
        }
    }
}