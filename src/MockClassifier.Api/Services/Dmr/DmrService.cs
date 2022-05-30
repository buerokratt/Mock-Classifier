using MockClassifier.Api.Interfaces;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr.Extensions;
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
        private readonly HttpClient _httpClient;
        private readonly ILogger<DmrService> _logger;
        private readonly IEncodingService _encodingService;

        private readonly ConcurrentQueue<DmrRequest> requests;

        public DmrService(IHttpClientFactory httpClientFactory, DmrServiceSettings config, ILogger<DmrService> logger, IEncodingService encodingService)
        {
            if (httpClientFactory == null)
            {
                throw new ArgumentNullException(nameof(httpClientFactory));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _httpClient = httpClientFactory.CreateClient(config.ClientName);
            _httpClient.BaseAddress = config.DmrApiUri;
            _logger = logger;

            requests = new ConcurrentQueue<DmrRequest>();

            _encodingService = encodingService;
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
                    // Setup content
                    var jsonPayload = JsonSerializer.Serialize(request.Payload);
                    var jsonPayloadBase64 = _encodingService.EncodeBase64(jsonPayload);
                    using var content = new StringContent(jsonPayloadBase64, Encoding.UTF8, MediaTypeNames.Application.Json);

                    // Setup message
                    using var requestMessage = new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        Content = content,
                    };
                    requestMessage.Headers.Add(Constants.MessageIdHeaderKey, request.Headers[Constants.MessageIdHeaderKey]);
                    requestMessage.Headers.Add(Constants.MessageIdRefHeaderKey, request.Headers[Constants.MessageIdRefHeaderKey]);
                    requestMessage.Headers.Add(Constants.SendToHeaderKey, request.Headers[Constants.SendToHeaderKey]);
                    requestMessage.Headers.Add(Constants.SentByHeaderKey, request.Headers[Constants.SentByHeaderKey]);

                    // Send request
                    var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);
                    _ = response.EnsureSuccessStatusCode();

                    // Not sure how to resolve rule CA1848 so removing logging for now
                    //_logger.LogInformation($"Callback to DMR. Classification = {request.Payload.Classification}, Message = {request.Payload.Message}");
                    _logger.DmrCallback(request.Payload.Classification, request.Payload.Message);
                }
                catch (HttpRequestException exception)
                {
                    // Not sure how to resolve rule CA1848 so removing logging for now
                    _logger.DmrCallbackFailed(exception);
                }
            }
        }
    }
}
