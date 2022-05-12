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
    public class DmrServiceTests
    {
        private static readonly DmrServiceSettings DefaultServiceConfig = new DmrServiceSettings
        {
            DmrApiUri = "https://dmr.fakeurl.com"
        };

        private readonly Mock<IHttpClientFactory> httpClientFactory = new Mock<IHttpClientFactory>();
        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();
        private readonly Mock<ILogger<DmrService>> logger = new Mock<ILogger<DmrService>>();

        [Fact]
        public async Task ShouldCallDmrApiWithGivenRequest_WhenRequestIsRecorded()
        {
            httpMessageHandler.SetupWithMessage();

            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.RecordRequest(GetDmrRequest());

            await sut.ProcessRequestsAsync();

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldCallDmrApiForEachGivenRequest_WhenMultipleRequestsAreRecorded()
        {
            httpMessageHandler
                .SetupWithMessage("my first message")
                .SetupWithMessage("my second message");
            
            var clientFactory = GetHttpClientFactory(httpMessageHandler);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);
            
            sut.RecordRequest(GetDmrRequest("my first message"));
            sut.RecordRequest(GetDmrRequest("my second message"));

            await sut.ProcessRequestsAsync();

            httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ShouldNotThrowException_WhenCallToDmrApiErrors()
        {
            var dmrHttpClient = new MockHttpMessageHandler();
            dmrHttpClient.When("/").Respond(HttpStatusCode.BadGateway);

            var clientFactory = GetHttpClientFactory(dmrHttpClient);

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.RecordRequest(GetDmrRequest());

            await sut.ProcessRequestsAsync();
        }

        private Mock<IHttpClientFactory> GetHttpClientFactory(MockHttpMessageHandler messageHandler)
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(() =>
                {
                    var client = messageHandler.ToHttpClient();
                    client.BaseAddress = new Uri("https://dmr.fakeurl.com");

                    return client;
                });

            return mockHttpClientFactory;
        }


        private DmrRequest GetDmrRequest(string message = "my test message")
        {
            return new DmrRequest
            {
                ForwardUri = "https://forwarduri.fakeurl.com",
                Payload = new Payload
                {
                    CallbackUri = "https://callbackuri.fakeurl.com",
                    Messages = new[] { message },
                    Ministry = "border"
                }
            };
        }
    }
}
