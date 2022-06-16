using RequestProcessor.AsyncProcessor;
using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Services.Dmr
{
    /// <summary>
    /// A settings object for <see cref="DmrService"/>
    /// </summary>
    [ExcludeFromCodeCoverage] // No logic so not appropriate for code coverage
    public class DmrServiceSettings : AsyncProcessorSettings
    {
        public Uri DmrApiUri { get; set; }
    }
}
