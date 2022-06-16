using Microsoft.Extensions.DependencyInjection.Extensions;
using RequestProcessor.AsyncProcessor;

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
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _ = services.AddHttpClient(settings.ClientName, client =>
            {
                client.BaseAddress = settings.DmrApiUri;
                client.Timeout = TimeSpan.FromMilliseconds(settings.HttpRequestTimeoutMs);
            });

            services.TryAddSingleton(settings);
            services.TryAddSingleton(settings as AsyncProcessorSettings);
            services.TryAddSingleton<IAsyncProcessorService<DmrRequest>, DmrService>();
            _ = services.AddHostedService<AsyncProcessorHostedService<DmrRequest>>();
        }
    }
}
