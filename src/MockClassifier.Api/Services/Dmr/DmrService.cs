using System.Collections.Concurrent;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

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
                    // Setup uri
                    var uri = new Uri(httpClient.BaseAddress + "/app/fromclassifier");

                    // Setup content
                    var jsonPayload = JsonSerializer.Serialize(request.Payload);
                    using var content = new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json);

                    // Setup message
                    using var requestMessage = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        Content = content,
                        RequestUri = uri
                    };
                    requestMessage.Headers.Add("X-Message-Id", request.Headers.MessageId);
                    requestMessage.Headers.Add("X-Message-Id-Ref", request.Headers.MessageIdRef);
                    requestMessage.Headers.Add("X-Sent-By", request.Headers.SendTo);
                    requestMessage.Headers.Add("X-Send-To", request.Headers.SentBy);

                    // Send request
                    var response = await httpClient.SendAsync(requestMessage).ConfigureAwait(true);
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
