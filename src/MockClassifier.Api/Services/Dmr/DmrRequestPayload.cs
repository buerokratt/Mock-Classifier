using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// The payload that the DMR handles
    /// </summary>
    [ExcludeFromCodeCoverage] // No logic so not appropriate for code coverage
    public record DmrRequestPayload
    {
        /// <summary>
        /// The ministry that should handle this payload
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// A message being sent to or from the DMR
        /// </summary>
        public string Message { get; set; }
    }
}
