/// <summary>
///  This interface for Token service which classifies messageBody based on pre-defined tokens 
/// </summary>
namespace MockClassifier.Api.Interfaces
{
    public interface ITokenService
    {
        public string[] Classify(string messageBody);
    }
}
