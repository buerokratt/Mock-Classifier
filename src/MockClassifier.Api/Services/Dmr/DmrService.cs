using Microsoft.Extensions.Logging;
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
            this.httpClient = httpClientFactory.CreateClient(config.ClientName);
            this.logger = logger;

            this.requests = new ConcurrentQueue<DmrRequest>();
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
                    var response = await httpClient.PostAsJsonAsync("/", request);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"Callback to DMR. Ministry = {request.Payload.Ministry}, Messages = {string.Join(", ", request.Payload.Messages)}");
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Call to DMR Service failed");
                }
            }
        }
    }
}
