using MockClassifier.Api.Interfaces;
using System.Text;

namespace MockClassifier.Api.Services
{
    public class EncodingService : IEncodingService
    {
        public string DecodeBase64(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            };

            Span<byte> buffer = new(new byte[content.Length]);
            var isBase64 = Convert.TryFromBase64String(content, buffer, out _);
            if (isBase64)
            {
                byte[] bytes = Convert.FromBase64String(content);
                var s = Encoding.Default.GetString(bytes);
                return s;
            }
            else
            {
                throw new ArgumentException("Argument is not valid Base64");
            }
        }

        public string EncodeBase64(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            };

            byte[] bytes = Encoding.ASCII.GetBytes(content);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }
    }
}
