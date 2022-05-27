using MockClassifier.Api.Services;
using System;
using Xunit;

namespace MockClassifier.UnitTests.Services
{
    public class EncodingServiceTest
    {
        private readonly EncodingService sut;
        private const string plainText = "burokratt";
        private const string base64Text = "YnVyb2tyYXR0";

        public EncodingServiceTest()
        {
            sut = new EncodingService();
        }

        [Fact]
        public void DecodeBase64ReturnsPlain()
        {
            // Act
            var result = sut.DecodeBase64(base64Text);

            // Assert
            Assert.Equal(plainText, result);
        }

        [Fact]
        public void DecodeBase64ReturnsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(delegate { _ = sut.DecodeBase64(null); });
        }

        [Fact]
        public void EncodeBase64ReturnsBase64()
        {
            // Act
            var result = sut.EncodeBase64(plainText);

            // Assert
            Assert.Equal(base64Text, result);
        }

        [Fact]
        public void EncodeBase64ReturnsArgumentNullException()
        {
            _ = Assert.Throws<ArgumentNullException>(delegate { _ = sut.EncodeBase64(null); });
        }
    }
}
