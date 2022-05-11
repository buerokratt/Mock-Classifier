using Microsoft.Extensions.Logging;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A service that handles calls to the DMR API
    /// </summary>
    public class DmrService : IDmrService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<DmrService> logger;

        public DmrService(IHttpClientFactory httpClientFactory, DmrServiceConfig config, ILogger<DmrService> logger)
        {
            this.httpClient = httpClientFactory.CreateClient(config.ClientName);
            this.logger = logger;
        }

        public void SendRequest(DmrRequest request)
        {
            /**
             * THIS IS VERY BAD PRACTICE - we should never call Task.Run like this! If the application crashes/shuts down for any reason, 
             * then all pending or executing tasks would be lost.
             * 
             * However, since this is a Mock service and we can be lax on reliability and fault tolerance and avoid further investment 
             * into code that might be thrown away.
             */
            Task.Run(async () =>
            {
                try
                {
                    var response = await httpClient.PostAsJsonAsync("/", request);
                    logger.LogInformation($"Callback to DMR. Ministry = {request.Payload.Ministry}, Messages = {request.Payload.Messages}");
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Call to DMR Service failed");
                }
            });
        }
    }
}
