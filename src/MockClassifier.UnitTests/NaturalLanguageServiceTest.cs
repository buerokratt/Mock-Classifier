using MockClassifier.Api.Services;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class NaturalLanguageServiceTest
    {
        private readonly NaturalLanguageService naturalLanguageService;

        public NaturalLanguageServiceTest()
        {
            naturalLanguageService = new NaturalLanguageService();
        }

        [Theory]
        [InlineData("I want to register my child at school", new string[] { "education" })]
        [InlineData("I wish to understand what benefits my family are entitled to", new string[] { "social" })]
        [InlineData("How do I arrange for my COVID-19 booster vaccination", new string[] { "social" })]
        [InlineData("How do I get to Lahemaa park?", new string[] { "environment" })]
        [InlineData("I have a question about the Estonian pension system", new string[] { "economic" })]
        [InlineData("How do I file my annual tax information", new string[] { "economic" })]
        [InlineData("i want to register my child at school", new string[] { "education" })]
        [InlineData("", new string[] { })]
        [InlineData("not mapped", new string[] { })]
        public void TestClassify(string messageBody, string[] expectedMinistries)
        {
            var result = naturalLanguageService.Classify(messageBody);
            Assert.Equal(expectedMinistries, result);
            Assert.NotNull(result);
        }
    }
}