using Xunit;
using MockClassifier.Api.Services;
using System.Collections.Generic;

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
        [InlineData("I wish to understand what benefits my family are entitled to", new string[] { "social" })]
        [InlineData("How do I arrange for my COVID-19 booster vaccination", new string[] { "social" })]
        [InlineData("How do I get to Lahemaa park?", new string[] { "environment" })]
        [InlineData("I have a question about the Estonian pension system", new string[] { "economic" })]
        [InlineData("How do I file my annual tax information", new string[] { "economic" })]
        [InlineData("<education>", new string[] { "education" })]
        [InlineData("Please return the <social> minsitry", new string[] { "social" })]
        [InlineData("I want to see <rural><social> and <environment>", new string[] {"rural","social", "environment" })]
        [InlineData("<rural> please", new string[] {"rural"}) ]
        [InlineData("Please return < environment>", new string[] { })]
        [InlineData("Please return <educationandresearch>", new string[] { })]
        [InlineData("<Please return <education,social>",new string[] {})]

        public void TestClassify( string messageBody, string[] expectedTokens)
        {
            var result = ministryClassifierService.Classify(messageBody);
            Assert.Equal(expectedTokens, result);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestRandomClassify()
        {
            var messageBody = "I want <random>";
            string[] result = ministryClassifierService.Classify(messageBody);
            var expectedMinistries = new List<string> { "justice", "education", "defence", "environment", "cultural", "rural", "economic", "finance", "social", "interior", "foreign" };
            Assert.Contains<string>(result[0], expectedMinistries);
        }
    }
}