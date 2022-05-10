using MockClassifier.Api.Models;
using MockClassifier.Api.Interfaces;

namespace MockClassifier.Api.Services
{
    public class NaturalLanguageService : IClassifierService
    {

        public string[] Classify(string messageBody)
        {
            var token = "";
            switch (messageBody)
            {
                case "I want to register my child at school": token = Ministry.education.ToString(); break;
                case "How do I file my annual tax information": token = Ministry.economic.ToString(); break;
                case "I have a question about the Estonian pension system": token = Ministry.economic.ToString(); break;
                case "How do I get to Lahemaa park?": token = Ministry.environment.ToString(); break;
                case "How do I arrange for my COVID-19 booster vaccination": token = Ministry.social.ToString(); break;
                case "I wish to understand what benefits my family are entitled to": token = Ministry.social.ToString(); break;
            }

            return new string[] { token };
        }
    }
}
