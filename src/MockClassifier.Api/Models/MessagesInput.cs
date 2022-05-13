using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Models
{
    // No logic so no unit tests are required
    [ExcludeFromCodeCoverage]
    public class MessagesInput
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
