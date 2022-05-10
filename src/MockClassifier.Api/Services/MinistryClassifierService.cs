using MockClassifier.Api.Interfaces;
using System.Text.RegularExpressions;

namespace MockClassifier.Api.Services
{
    public class MinistryClassifierService : IClassifierService
    {
       private readonly Regex ministryRegex = new("<random>|<justice>|<education>|<defence>|<environment>|<cultural>|<rural>|<economic>|<finance>|<social>|<interior>|<foreign>", RegexOptions.Compiled);


       public string[] Classify(string messageBody)
       {
            var token = "";

            //Is it one of hardcoded expression            
            switch (messageBody)
	        {
		        case "I want to register my child at school" : token = "education"; break;
                case "How do I file my annual tax information" : token = "economic"; break; 
                case "I have a question about the Estonian pension system" : token = "economic"; break ;
                case "How do I get to Lahemaa park?" : token = "environment"; break ;   
                case "How do I arrange for my COVID-19 booster vaccination" : token = "social"; break ;
                case "I wish to understand what benefits my family are entitled to" : token = "social"; break ;
	        }
            
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

            //get random ministry
            if (tokens.Any() && tokens.Contains("random"))
                token = GetRandomMinistry();

            if ( token != "")
                return new string[] { token };
            else 
                return tokens.ToArray();
       }

        private string GetRandomMinistry()
        {
            var ministries = new string[] { "justice", "education", "defence", "environment", "cultural", "rural", "economic", "finance", "social", "interior", "foreign" };
            Random rand = new();
            // Generate a random index less than the size of the array.  
            int index = rand.Next(ministries.Length);
            return ministries[index];
        }

    }
}
