namespace MockClassifier.Api.Models
{
    public class MessagesInput
    {
        public string CallbackUri { get; set; }
        public string[] Messages { get; set; }
    }
}
