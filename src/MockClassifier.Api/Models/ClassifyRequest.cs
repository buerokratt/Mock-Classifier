namespace MockClassifier.Api.Models
{
    public record ClassifyRequest
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
