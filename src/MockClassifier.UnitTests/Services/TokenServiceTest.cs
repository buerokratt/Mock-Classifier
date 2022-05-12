using Xunit;
using MockClassifier.Api.Services;
using MockClassifier.Api.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MockClassifier.UnitTests.Services
{
    public class TokenServiceTest
    {
        private readonly TokenService tokenService;

        public TokenServiceTest()
        {
            tokenService = new TokenService();
        }

        [Theory]
        [InlineData("<education>", new string[] { "education" })]
        [InlineData("Please return the <social> ministry", new string[] { "social" })]
        [InlineData("I want to see <rural><social> and <environment>", new string[] {"rural","social", "environment" })]
        [InlineData("<rural> please", new string[] {"rural"}) ]
        [InlineData("Please return < environment>", new string[] { })]
        [InlineData("Please return <educationandresearch>", new string[] { })]
        [InlineData("<Please return <education,social>",new string[] {})]
        [InlineData("", new string[] { })]

        public void TestClassify( string messageBody, string[] expectedTokens)
        {
            var result = tokenService.Classify(messageBody);
            Assert.Equal(expectedTokens, result);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestRandomClassify()
        {
            var messageBody = "I want <random>";
            string[] result = tokenService.Classify(messageBody);
            var expectedMinistries = Enum.GetValues(typeof(Ministry))
                                    .Cast<Ministry>()
                                    .ToList();
            Assert.Contains((Ministry)Enum.Parse<Ministry>(result[0]), expectedMinistries);
        }
    }
}