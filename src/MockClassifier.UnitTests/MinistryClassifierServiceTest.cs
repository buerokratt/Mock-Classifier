using Xunit;
using MockClassifier.Api.Services;

namespace MockClassifier.UnitTests
{
    public class MinistryClassifierServiceTest
    {
        private readonly MinistryClassifierService ministryClassifierService;

        public MinistryClassifierServiceTest()
        {
            ministryClassifierService = new MinistryClassifierService();
        }

        [Theory]
        [InlineData("I want to register my child at school", new string[] { "education" })]
        [InlineData("<education>", new string[] { "education" })]
        [InlineData("Please return the <social> minsitry", new string[] { "social" })]
        [InlineData("I want to see <rural><social> and <environment>", new string[] {"rural","social", "environment" })]
        [InlineData("<rural> please", new string[] {"rural"}) ]
        [InlineData("<random> please", new string[] { "rural" })]
        [InlineData("Please return < environment>", new string[] { })]
        [InlineData("Please return <educationandresearch>", new string[] { })]
        public void TestClassify( string messageBody, string[] expectedTokens)
        {
            var result = ministryClassifierService.Classify(messageBody);
            Assert.Equal(expectedTokens, result);
            Assert.NotNull(result);
        }
    }
}