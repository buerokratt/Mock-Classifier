/// <summary>
///  This interface for Natural Language service which classifies messageBody based on pre-defined phrases
/// </summary>
namespace MockClassifier.Api.Interfaces
{
    public interface INaturalLanguageService
    {
        public string[] Classify(string messageBody);
    }
}
