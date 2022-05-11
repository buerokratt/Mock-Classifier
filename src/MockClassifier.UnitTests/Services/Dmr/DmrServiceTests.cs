using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MockClassifier.Api.Services.Dmr;
using MockClassifier.UnitTests.Mocks;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MockClassifier.UnitTests.Services.Dmr
{
    public class DmrServiceTests
    {
        private static readonly DmrServiceConfig DefaultServiceConfig = new DmrServiceConfig
        {
            DmrApiUri = "https://dmr.fakeurl.com"
        };

        [Fact]
        public void Test1()
        {
            var httpMessageHandler = MockHttpMessageHandler.ShouldReturnResponse(new HttpResponseMessage(HttpStatusCode.Accepted));
            var clientFactory = GetHttpClientFactory(httpMessageHandler);
            var logger = GetLogger();

            var sut = new DmrService(clientFactory.Object, DefaultServiceConfig, logger.Object);

            sut.SendRequest(new DmrRequest
            {
                ForwardUri = "https://forwarduri.fakeurl.com",
                Payload = new Payload
                {
                    CallbackUri = "https://callbackuri.fakeurl.com",
                    Messages = new[] { "my test message" },
                    Ministry = "border"
                }
            });

            Thread.Sleep(100);


        }

        private Mock<IHttpClientFactory> GetHttpClientFactory(HttpMessageHandler messageHandler)
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory
                .Setup(m => m.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(messageHandler));

            return mockHttpClientFactory;
        }

        private Mock<ILogger<DmrService>> GetLogger()
        {
            return new Mock<ILogger<DmrService>>();
        }
    }
}
