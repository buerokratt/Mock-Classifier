using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// The model for the DMR API enpoint including headers and body
    /// </summary>
    [ExcludeFromCodeCoverage] // No logic so not appropriate for code coverage
    public record DmrRequest
    {
        /// <summary>
        /// Name of the participant that sent the message.
        /// </summary>
        public string SentBy { get; set; }

        /// <summary>
        /// The payload being sent to the <see cref="ForwardUri"/>
        /// </summary>
        public DmrRequestPayload Payload { get; set; }
    }
}
