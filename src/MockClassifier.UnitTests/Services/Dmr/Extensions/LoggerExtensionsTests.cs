using Microsoft.Extensions.Logging;
using MockClassifier.Api.Services.Dmr.Extensions;
using Moq;
using System;
using Xunit;

namespace MockClassifier.UnitTests.Services.Dmr.Extensions
{
    public class LoggerExtensionsTests
    {
        private readonly Mock<ILogger> _mockLogger;

        public LoggerExtensionsTests()
        {
            _mockLogger = new Mock<ILogger>();
            _ = _mockLogger
                .Setup(m => m.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true);
        }

        [Theory]
        [InlineData("border", "message to border")]
        [InlineData("education", "message to education")]
        public void DmrCallbackShouldLogClassificationAndMessage(string classification, string message)
        {
            var logger = _mockLogger.Object;

            logger.DmrCallback(classification, message);

            _mockLogger.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.Is<EventId>(e => e.Id == 1 && e.Name == "DmrCallbackPosted"),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Fact]
        public void DmrCallbackFailedShouldLogException()
        {
            var logger = _mockLogger.Object;

            var exception = new InvalidOperationException("my test exception");
            logger.DmrCallbackFailed(exception);

            _mockLogger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.Is<EventId>(e => e.Id == 2 && e.Name == "DmrCallbackFailed"),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.Is<Exception>(ex => ex.Message == "my test exception"),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}
