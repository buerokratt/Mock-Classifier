using MockClassifier.Api.Services;
using MockClassifier.Api.Services.Dmr;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace MockClassifier.UnitTests.Extensions
{
    internal static class MockHttpMessageHandlerExtensions
    {
        public static MockHttpMessageHandler SetupWithMessage(
            this MockHttpMessageHandler handler,
            string expectedMessage = "my test message")
        {
            var payload = new DmrRequestPayload
            {
                Message = expectedMessage,
                Classification = string.Empty,
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            var jsonPayloadBase64 = new EncodingService().EncodeBase64(jsonPayload);
            _ = handler
                .Expect("/")
                .WithContent(jsonPayloadBase64)
                .Respond(HttpStatusCode.Accepted);

            return handler;
        }
    }
}
