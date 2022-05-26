using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// The model for Dmr requests, including headers and payload/body
    /// </summary>
    [ExcludeFromCodeCoverage] // No logic so not appropriate for code coverage
    public record DmrRequest
    {
        /// <summary>
        /// The headers  of requests to or from Dmr
        /// </summary>
        public DmrRequestHeaders Headers { get; set; }

        /// <summary>
        /// The payload (request body) of requests to or from Dmr
        /// </summary>
        public DmrRequestPayload Payload { get; set; }
    }
}
