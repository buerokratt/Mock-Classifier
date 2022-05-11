/// <summary>
///  This interface for Token service which classifies messageBody based on  pre-defined phrases
/// </summary>
namespace MockClassifier.Api.Interfaces
{
    public interface INaturalLanguageService
    {
        public string[] Classify(string messageBody);
    }
}
