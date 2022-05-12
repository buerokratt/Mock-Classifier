namespace MockClassifier.Api.Models
{
    public record MessagesInput
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
