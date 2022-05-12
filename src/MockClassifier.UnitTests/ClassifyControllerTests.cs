using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Models;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System.Linq;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class ClassifyControllerTests
    {
        private readonly Mock<IDmrService> mockDmrService;
        private readonly ClassifyController sut;

        public ClassifyControllerTests()
        {
            mockDmrService = new Mock<IDmrService>();
            sut = new ClassifyController(mockDmrService.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData("message1")]
        [InlineData("message1","message2")]
        public void ReturnsAccepted(params string[] messages)
        {
            // Arrange
            var request = new ClassifyRequest
            {
                CallbackUri = "https://callback.fakeurl.com",
                Messages = messages
            };

            // Act
            var result = sut.Post(request);

            // Assert
            Assert.IsType<AcceptedResult>(result);
        }
    }
}
