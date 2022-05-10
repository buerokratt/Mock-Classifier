using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;
using System.Text.RegularExpressions;


namespace MockClassifier.Api.Services
{
    public class TokenService : IClassifierService
    {
       private readonly Regex ministryRegex = new("<random>|<justice>|<education>|<defence>|<environment>|<cultural>|<rural>|<economic>|<finance>|<social>|<interior>|<foreign>", RegexOptions.Compiled);

       public string[] Classify(string messageBody)
       {
            //search for 11 ministries' label and strip <> and return in lowercase if found
            var tokenSections = ministryRegex.Matches(messageBody);
            var tokens = tokenSections
                    .SelectMany(s => s.Captures)
                    .SelectMany(c =>
                        c.Value
                            .Replace("<", string.Empty)
                            .Replace(">", string.Empty)
                            .Split(","))
                    .Select(s => s.ToLower().Trim())
                    .Distinct();

            //get random ministry if random token passed in
            if (tokens.Any() && tokens.Contains("random"))
            {
                var token = GetRandomMinistry();
                return new string[] { token };
            }
            else
                return tokens.ToArray();
       }

        private static string GetRandomMinistry()
        {
            Array values = Enum.GetValues(typeof(Ministry));
            Random random = new Random();
            Ministry randomMinistry = (Ministry)values.GetValue(random.Next(values.Length));
            return randomMinistry.ToString();
        }

    }
}
