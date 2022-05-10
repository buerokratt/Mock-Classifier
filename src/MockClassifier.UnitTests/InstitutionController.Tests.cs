using Microsoft.AspNetCore.Mvc;
using MockClassifier.Api.Controllers;
using MockClassifier.Api.Models;
using System.Linq;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class InstitutionControllerTests
    {
        private readonly InstitutionController sut;

        public InstitutionControllerTests()
        {
            sut = new InstitutionController();
        }

        [Theory]
        [InlineData("message1")]
        [InlineData("message1","message2")]
        public void TestReturnsAccepted(params string[] messages)
        {
            // Arrange
            var messagesInput = new MessagesInput() { Messages = messages.ToList() };

            // Act
            var result = sut.Post(messagesInput);

            // Assert
            Assert.IsType<AcceptedResult>(result);
        }
    }
}
