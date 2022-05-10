/// <summary>
///  This interface for Classifier service
/// </summary>
namespace MockClassifier.Api.Interfaces
{
    public interface IClassifierService
    {
        string[] Classify(string messageBody);
    }
}
