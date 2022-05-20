using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class InstitutionControllerTests
    {
        private readonly InstitutionController sut;
        private readonly Mock<IDmrService> dmrService = new();
        private readonly TokenService tokenService = new();
        private readonly NaturalLanguageService naturalLanguageService = new();

        public InstitutionControllerTests()
        {
            dmrService = new Mock<IDmrService>();
            sut = new InstitutionController(dmrService.Object, tokenService, naturalLanguageService);
        }

        [Theory]
        [InlineData("")]
        [InlineData("message1")]
        [InlineData("message1", "message2")]
        public void ReturnsAccepted(params string[] messages)
        {
            // Arrange
            var request = new MessagesInput
            {
                CallbackUri = new Uri("https://callback.fakeurl.com"),
                Messages = messages
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
        public void VerifyDmrRequest(params string[] messages)
        {
            // Arrange
            var request = new MessagesInput
            {
                CallbackUri = new Uri("https://callback.fakeurl.com"),
                Messages = messages
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
