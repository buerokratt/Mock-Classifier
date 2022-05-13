using System.Diagnostics.CodeAnalysis;

namespace MockClassifier.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class MessagesInput
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
