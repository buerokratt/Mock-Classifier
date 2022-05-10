namespace MockClassifier.Api.Interfaces
{
public interface IClassifierService
{
    string[] Classify(string messageBody);
}
}
