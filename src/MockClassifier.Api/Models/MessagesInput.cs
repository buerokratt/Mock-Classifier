using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class MessagesInput
    {
        public Uri CallbackUri { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
