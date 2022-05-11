using MockClassifier.Api.Controllers;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace MockClassifier.UnitTests
{
    public class ClassifyControllerTests
    {
        private readonly Mock<IDmrService> mockDmrService;
        private readonly ClassifyController controller;

        public ClassifyControllerTests()
        {
            mockDmrService = new Mock<IDmrService>();

            controller = new ClassifyController(mockDmrService.Object);
        }

        [Fact]
        public void Test1()
        {

        }
    }
}