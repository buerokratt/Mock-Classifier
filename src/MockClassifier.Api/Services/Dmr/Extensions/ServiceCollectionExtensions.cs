using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MockClassifier.Api.Services.Dmr.Extensions
{
    /// <summary>
    /// Extension class to help add all services related to the DMR
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Helper extension to easily install the DMR Service
        /// </summary>
        /// <param name="services">The services collection that <see cref="DmrService"/> and related services will be added to.</param>
        /// <param name="settings">A settings object for the <see cref="DmrService"/></param>
        public static void AddDmrService(this IServiceCollection services, DmrServiceSettings settings)
        {
            services.AddHttpClient(settings.ClientName, client =>
            {
                client.BaseAddress = new Uri(settings.DmrApiUri);
                client.Timeout = TimeSpan.FromMilliseconds(settings.HttpRequestTimeoutMs);
            });

            services.TryAddSingleton(settings);
            services.TryAddSingleton<IDmrService, DmrService>();
            services.AddHostedService<DmrHostedService>();
        }
    }
}
