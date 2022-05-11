namespace MockClassifier.Api.Models
{
    public record ClassifierRequest
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
