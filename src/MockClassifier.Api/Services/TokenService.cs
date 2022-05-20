using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace MockClassifier.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly Regex ministryRegex;
        private static readonly Random random = new();
        private readonly Array ministryCache;

        public TokenService()
        {
            ministryCache = Enum.GetValues(typeof(Ministry));
            StringBuilder sb = new();
            foreach (var ministry in ministryCache)
            {
                _ = sb.Append('<');
                _ = sb.Append(ministry.ToString());
                _ = sb.Append(">|");
            }
            _ = sb.Append("<random>");
            ministryRegex = new(sb.ToString(), RegexOptions.Compiled);
        }

        public string[] Classify(string messageBody)
        {
            if (string.IsNullOrEmpty(messageBody))
            {
                return Array.Empty<string>();
            }

            //search for 11 ministries' label and strip <> and return in lowercase if found
            var tokenSections = ministryRegex.Matches(messageBody);
            var tokens = tokenSections
                    .SelectMany(s => s.Captures)
                    .Select(c =>
                        c.Value
                            .Replace("<", string.Empty, StringComparison.Ordinal)
                            .Replace(">", string.Empty, StringComparison.Ordinal)
                            )
                    .Select(s => s.ToLower(CultureInfo.CurrentCulture).Trim())
                    .Distinct();

            //get random ministry if random token passed in
            if (tokens.Any() && tokens.Contains("random"))
            {
                var token = GetRandomMinistryName();
                return new string[] { token };
            }
            else
            {
                return tokens.ToArray();
            }
        }

        private string GetRandomMinistryName()
        {
            Ministry randomMinistry = (Ministry)ministryCache.GetValue(random.Next(ministryCache.Length));
            return randomMinistry.ToString();
        }

    }
}
