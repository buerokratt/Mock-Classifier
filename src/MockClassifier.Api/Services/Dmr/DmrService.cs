using System.Collections.Concurrent;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class DmrService : IDmrService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DmrService> logger;

        private readonly ConcurrentQueue<DmrRequest> requests;

        public DmrService(IHttpClientFactory httpClientFactory, DmrServiceSettings config, ILogger<DmrService> logger)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            httpClient = httpClientFactory.CreateClient(config.ClientName);
            httpClient.BaseAddress = config.DmrApiUri;
            this.logger = logger;

            requests = new ConcurrentQueue<DmrRequest>();
        }

        public void RecordRequest(DmrRequest request)
        {
            requests.Enqueue(request);
        }

        public async Task ProcessRequestsAsync()
        {
            while (requests.TryDequeue(out var request))
            {
                try
                {
                    var response = await httpClient.PostAsJsonAsync("/app/FromClassifier", request.Payload).ConfigureAwait(true);
                    _ = response.EnsureSuccessStatusCode();


                    // Not sure how to resolve rule CA1848 so removing logging for now
                    //logger.LogInformation($"Callback to DMR. Ministry = {request.Payload.Ministry}, Messages = {string.Join(", ", request.Payload.Messages)}");
                }
                catch (HttpRequestException)
                {
                    // Not sure how to resolve rule CA1848 so removing logging for now
                    //logger.LogError(exception, "Call to DMR Service failed");
                }
            }
        }
    }
}
