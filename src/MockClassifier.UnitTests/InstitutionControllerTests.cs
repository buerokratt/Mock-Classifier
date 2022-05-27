using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class InstitutionControllerTests
    {
        private readonly MessagesController sut;
        private readonly Mock<IDmrService> dmrService = new();
        private readonly TokenService tokenService = new();
        private readonly NaturalLanguageService naturalLanguageService = new();

        public InstitutionControllerTests()
        {
            dmrService = new Mock<IDmrService>();
            sut = new MessagesController(dmrService.Object, tokenService, naturalLanguageService);
        }

        [Theory]
        [InlineData("")]
        [InlineData("message1")]
        public void ReturnsAccepted(string message)
        {
            // Arrange
            var request = new DmrRequestPayload
            {
                Message = message,
                Classification = string.Empty
            };

            _ = dmrService.Setup(m => m.RecordRequest(It.IsAny<DmrRequest>()));

            // Act
            var result = sut.Post(request);

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
            var result = sut.Post(request);

            // Assert
            _ = Assert.IsType<AcceptedResult>(result);
            dmrService.Verify(x => x.RecordRequest(It.IsAny<DmrRequest>()), Times.Once());
        }
    }
}
