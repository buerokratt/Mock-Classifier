﻿using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;
using System.Globalization;

namespace MockClassifier.Api.Services
{
    public class NaturalLanguageService : INaturalLanguageService
    {
        public string[] Classify(string messageBody)
        {
            if (string.IsNullOrEmpty(messageBody))
            {
                return Array.Empty<string>();
            }

            var ministry = messageBody.ToLower(CultureInfo.CurrentCulture) switch
            {
                "i want to register my child at school" => Ministry.education.ToString(),
                "how do i file my annual tax information" => Ministry.economic.ToString(),
                "i have a question about the estonian pension system" => Ministry.economic.ToString(),
                "how do i get to lahemaa park?" => Ministry.environment.ToString(),
                "how do i arrange for my covid-19 booster vaccination" => Ministry.social.ToString(),
                "i wish to understand what benefits my family are entitled to" => Ministry.social.ToString(),
                "where can i buy school uniform" => Ministry.education.ToString(),
                _ => null,
            };
            return ministry == null ? Array.Empty<string>() : (new string[] { ministry });
        }
    }
}
