using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class MessagesControllerTests
    {
        private readonly MessagesController sut;
        private readonly Mock<IDmrService> dmrService = new();
        private readonly TokenService tokenService = new();
        private readonly NaturalLanguageService naturalLanguageService = new();
        private readonly EncodingService encodingService = new();

        public MessagesControllerTests()
        {
            dmrService = new Mock<IDmrService>();
            sut = new MessagesController(dmrService.Object, tokenService, naturalLanguageService, encodingService);
        }

        [Fact]
        public async Task ReturnsAccepted()
        {
            // Arrange
            var payload = "eyJDbGFzc2lmaWNhdGlvbiI6IiIsIk1lc3NhZ2UiOiJtZXNzYWdlMSJ9"; //{"Classification":"","Message":"message1"}
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = await sut.Post().ConfigureAwait(true);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task VerifyDmrRequest()
        {
            // Arrange
            var payload = "eyJDbGFzc2lmaWNhdGlvbiI6IiIsIk1lc3NhZ2UiOiI8ZGVmZW5jZT4ifQ=="; //{"Classification":"","Message":"<defence>"}
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = await sut.Post().ConfigureAwait(true);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService
                .Verify(
                    x => x.RecordRequest(
                        It.Is<DmrRequest>(d => d.Payload.Message == "<defence>")),
                    Times.Once());
            dmrService.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("eyJDbGFzc2lmaWNhdGlvbiI6IiIsIk1lc3NhZ2UiOiI8ZGVmZW5jZT4ifQ==", 1)] //{"Classification":"","Message":"<defence>"}
        [InlineData("eyJDbGFzc2lmaWNhdGlvbiI6IiIsIk1lc3NhZ2UiOiI8ZGVmZW5jZT48ZWR1Y2F0aW9uPiJ9", 2)] //{"Classification":"","Message":"<defence><education>"}
        public async Task VerifyMultipleCallsToDmrServiceWhenThereAreMultipleClassifications(string payload, int expectedDmrServiceCalls)
        {
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(payload)
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = await sut.Post().ConfigureAwait(true);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService
                .Verify(
                    x => x.RecordRequest(It.IsAny<DmrRequest>()),
                    Times.Exactly(expectedDmrServiceCalls));
            dmrService.VerifyNoOtherCalls();
        }

        private static DefaultHttpContext GetContext(string payload)
        {
            var httpContext = new DefaultHttpContext();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;
            return httpContext;
        }
    }
}
