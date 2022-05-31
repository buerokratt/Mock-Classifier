namespace MockClassifier.Api.Interfaces
{
    public interface IEncodingService
    {
        /// <summary>
        /// Decodes a Base64 encoded string to plain text
        /// </summary>
        /// <param name="content">Base64 string to be decoded</param>
        /// <returns>Decoded string as plain text</returns>
        public string DecodeBase64(string content);

        /// <summary>
        /// Encodes a plain text string to Base64
        /// </summary>
        /// <param name="content">Plain string to be encoded</param>
        /// <returns>Base64 encoded string</returns>
        public string EncodeBase64(string content);
    }
}
