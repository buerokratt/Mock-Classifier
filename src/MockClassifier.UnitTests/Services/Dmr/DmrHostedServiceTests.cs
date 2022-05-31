using Microsoft.Extensions.Logging;
using MockClassifier.Api.Services.Dmr;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MockClassifier.UnitTests.Services.Dmr
{
    public sealed class DmrHostedServiceTests : IDisposable
    {
        private readonly DmrHostedService sut;

        public DmrHostedServiceTests()
        {
            DmrServiceSettings DefaultServiceConfig = new()
            {
                DmrApiUri = new Uri("https://dmr.fakeurl.com")
            };
            Mock<ILogger<DmrHostedService>> logger = new();
            Mock<IDmrService> dmrService = new();
            sut = new DmrHostedService(dmrService.Object, DefaultServiceConfig, logger.Object);
        }

        [Fact]
        public async Task StartAsyncReturnsCompletedTask()
        {
            Task result = sut.StartAsync(default);
            await result.ConfigureAwait(false);
            Assert.Equal(TaskStatus.RanToCompletion, result.Status);
        }

        [Fact]
        public async Task StopAsyncReturnsCompletedTask()
        {
            Task result = sut.StopAsync(default);
            await result.ConfigureAwait(false);
            Assert.Equal(TaskStatus.RanToCompletion, result.Status);
        }

        public void Dispose()
        {
            sut.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
