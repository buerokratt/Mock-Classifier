﻿using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;
using System.Text.RegularExpressions;
using System.Text;

namespace MockClassifier.Api.Services
{
    public class TokenService : IClassifierService
    {
        private readonly Regex ministryRegex;
        private static readonly Random random = new();
        private readonly Array ministryCache;

        public TokenService()      
        {
            ministryCache = Enum.GetValues(typeof(Ministry));
            StringBuilder sb = new();            
            foreach (var ministry in ministryCache) {
                sb.Append("<");
                sb.Append(ministry.ToString());
                sb.Append(">|");
            }
            sb.Append("<random>");
            ministryRegex = new(sb.ToString(), RegexOptions.Compiled);
        }

        public string[] Classify(string messageBody)
        {
            if(messageBody == null || messageBody.Length == 0)
                return Array.Empty<string>();
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

        private string GetRandomMinistry()
        {
            Ministry randomMinistry = (Ministry)ministryCache.GetValue(random.Next(ministryCache.Length));
            return randomMinistry.ToString();
        }

    }
}
