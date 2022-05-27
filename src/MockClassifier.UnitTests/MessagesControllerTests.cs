using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System.IO;
using System.Text;
using System.Text.Json;
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

        [Theory]
        [InlineData("")]
        [InlineData("message1")]
        public async Task ReturnsAccepted(string message)
        {
            // Arrange payload
            var payload = new DmrRequestPayload
            {
                Message = message,
                Classification = string.Empty
            };
            var encodedPayload = encodingService.EncodeBase64(JsonSerializer.Serialize(payload));

            // Arrange controller with context
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = GetContext(encodedPayload)
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = await sut.Post().ConfigureAwait(true);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData("<defence>")]
        public void VerifyDmrRequest(string message)
        {
            // Arrange
            var request = new DmrRequestPayload
            {
                Message = message,
                Classification = string.Empty
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = sut.Post();

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService.Verify(x => x.RecordRequest(It.IsAny<DmrRequest>()), Times.Once());
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
