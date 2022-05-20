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
                .WithContent($"{{\"forwardUri\":\"https://forwarduri.fakeurl.com\",\"payload\":{{\"callbackUri\":\"https://callbackuri.fakeurl.com\",\"ministry\":\"border\",\"messages\":[\"{expectedMessage}\"]}}}}")
                .Respond(HttpStatusCode.Accepted);

            return handler;
        }
    }
}
