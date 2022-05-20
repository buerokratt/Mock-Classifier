using Microsoft.Extensions.Logging;
using MockClassifier.Api.Services.Dmr;
using MockClassifier.UnitTests.Extensions;
using Moq;
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

        [Fact]
        public async Task ShouldCallDmrApiWithGivenRequestWhenRequestIsRecorded()
        {
            _ = httpMessageHandler.SetupWithMessage();

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.RecordRequest(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(true);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequestWhenMultipleRequestsAreRecorded()
        {
            _ = httpMessageHandler
                .SetupWithMessage("my first message")
                .SetupWithMessage("my second message");

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.RecordRequest(GetDmrRequest("my first message"));
            sut.RecordRequest(GetDmrRequest("my second message"));

            await sut.ProcessRequestsAsync().ConfigureAwait(true);

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotThrowExceptionWhenCallToDmrApiErrors()
        {
            using var dmrHttpClient = new MockHttpMessageHandler();
            _ = dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.RecordRequest(GetDmrRequest());

            await sut.ProcessRequestsAsync().ConfigureAwait(true);
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


        private static DmrRequest GetDmrRequest(string message = "my test message")
        {
            return new DmrRequest
            {
                ForwardUri = new Uri("https://forwarduri.fakeurl.com"),
                Payload = new Payload
                {
                    CallbackUri = new Uri("https://callbackuri.fakeurl.com"),
                    Messages = new[] { message },
                    Ministry = "border"
                }
            };
        }

        public void Dispose()
        {
            httpMessageHandler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
