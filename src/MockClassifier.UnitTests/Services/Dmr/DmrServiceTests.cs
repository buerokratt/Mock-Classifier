using Microsoft.Extensions.Logging;
using MockClassifier.Api.Services.Dmr;
using MockClassifier.UnitTests.Extensions;
using Moq;
using RequestProcessor.Models;
using RequestProcessor.Services.Encoder;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MockClassifier.UnitTests.Services.Dmr
{
    public sealed class DmrServiceTests : IDisposable
    {
        private static readonly DmrServiceSettings DefaultServiceConfig = new()
        {
            DmrApiUri = new Uri("https://dmr.fakeurl.com")
        };

        private readonly MockHttpMessageHandler httpMessageHandler = new();
        private readonly Mock<ILogger<DmrService>> logger = new();
        private readonly IEncodingService encodingService = new EncodingService();

        [Fact]
        public async Task ShouldCallDmrApiWithGivenRequestWhenRequestIsRecorded()
        {
            _ = httpMessageHandler.SetupWithExpectedMessage();

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object, encodingService);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequestWhenMultipleRequestsAreRecorded()
        {
            _ = httpMessageHandler
                .SetupWithExpectedMessage("my first message", "education")
                .SetupWithExpectedMessage("my second message", "social");

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object, encodingService);

            sut.Enqueue(GetDmrRequest("my first message", "education"));
            sut.Enqueue(GetDmrRequest("my second message", "social"));

            await sut.ProcessRequestsAsync().ConfigureAwait(false);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object, encodingService);

            sut.Enqueue(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(false);
        }

        private static Mock<IHttpClientFactory> GetHttpClientFactory(MockHttpMessageHandler messageHandler, DmrServiceSettings settings = null)
        {
            settings ??= DefaultServiceConfig;

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _ = mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();
                    client.BaseAddress = settings.DmrApiUri;

                    return client;
                });

            return mockHttpClientFactory;
        }


        private static DmrRequest GetDmrRequest(string message = "my test message", string classification = "border")
        {
            // Setup headers
            var dmrHeaders = new HeadersInput
            {
                XSentBy = "MockClassifier.UnitTests.Services.Dmr.DmrServiceTests",
                XMessageId = "1f7b356d-a6f4-4aeb-85cd-9d570dbc7606",
                XSendTo = "Classifier",
                XMessageIdRef = "5822c6ef-177d-4dd7-b4c5-0d9d8c8d2c35",
                XModelType = "MyModelType",
            };

            var request = new DmrRequest(dmrHeaders)
            {
                Payload = new DmrRequestPayload
                {
                    Message = message,
                    Classification = classification
                }
            };

            return request;
        }

        public void Dispose()
        {
            httpMessageHandler.Dispose();
        }
    }
}
