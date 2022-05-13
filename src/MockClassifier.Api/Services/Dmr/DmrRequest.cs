using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /**
     * NOTE:
     * These models live here temporarily until they can be referenced via a NuGet package library
     */

    /// <summary>
    /// The model for the DMR API endpoint
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record DmrRequest
    {
        /// <summary>
        /// The destination URI for the <see cref="Payload"/>
        /// </summary>
        public string ForwardUri { get; set; }

        /// <summary>
        /// The payload being sent to the <see cref="ForwardUri"/>
        /// </summary>
        public Payload Payload { get; set; }
    }

    /// <summary>
    /// The payload that the DMR handles
    /// </summary>
    [ExcludeFromCodeCoverage]
    public record Payload
    {
        /// <summary>
        /// The callback URI
        /// </summary>
        public string CallbackUri { get; set; }

        /// <summary>
        /// The ministry that should handle this payload
        /// </summary>
        public string Ministry { get; set; }

        /// <summary>
        /// One or more messages being sent to the DMR
        /// </summary>
        public string[] Messages { get; set; }
    }
}
