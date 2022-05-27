using RichardSzalay.MockHttp;
using System.Net;

namespace MockClassifier.UnitTests.Extensions
{
    internal static class MockHttpMessageHandlerExtensions
    {
        public static MockHttpMessageHandler SetupWithMessage(
            this MockHttpMessageHandler handler,
            string expectedMessage = "my test message")
        {
            _ = handler
                .Expect("/")
                .WithContent($"{{\"Classification\":\"border\",\"Message\":\"{expectedMessage}\"}}")
                .Respond(HttpStatusCode.Accepted);

            return handler;
        }
    }
}
