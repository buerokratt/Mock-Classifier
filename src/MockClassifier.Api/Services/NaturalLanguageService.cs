using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;

namespace MockClassifier.Api.Services
{
    public class NaturalLanguageService : IClassifierService
    {
        public string[] Classify(string messageBody)
        {
            if (messageBody == null || messageBody.Length == 0)
                return Array.Empty<string>(); 

            var token = messageBody.ToLower() switch
            {
                "i want to register my child at school" => Ministry.education.ToString(),
                "how do i file my annual tax information" => Ministry.economic.ToString(),
                "i have a question about the estonian pension system" => Ministry.economic.ToString(),
                "how do i get to lahemaa park?" => Ministry.environment.ToString(),
                "how do i arrange for my covid-19 booster vaccination" => Ministry.social.ToString(),
                "i wish to understand what benefits my family are entitled to" => Ministry.social.ToString(),
                _ => null,
            };
            if (token == null)
                return Array.Empty<string>();
            else
                return new string[] { token };
        }
    }
}
