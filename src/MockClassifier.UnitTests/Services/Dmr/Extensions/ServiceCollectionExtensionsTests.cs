using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MockClassifier.Api.Services.Dmr;
using MockClassifier.Api.Services.Dmr.Extensions;
using System;
using System.Linq;
using Xunit;

namespace MockClassifier.UnitTests.Services.Dmr.Extensions
{
    public sealed class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDmrServiceAddsServices()
        {
            //Arrange
            DmrServiceSettings DefaultServiceConfig = new()
            {
                DmrApiUri = new Uri("https://dmr.fakeurl.com"),
                ClientName = "Foo",
            };
            var collection = new ServiceCollection();

            // Act
            collection.AddDmrService(DefaultServiceConfig);
            var dmrService = collection.FirstOrDefault(e => e.ServiceType == typeof(IDmrService));
            var dmrServiceSettings = collection.FirstOrDefault(e => e.ServiceType == typeof(DmrServiceSettings));
            var dmrHostedService = collection.FirstOrDefault(e => e.ServiceType == typeof(IHostedService));

            // Assert
            Assert.Equal(ServiceLifetime.Singleton, dmrService.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrServiceSettings.Lifetime);
            Assert.Equal(ServiceLifetime.Singleton, dmrHostedService.Lifetime);
        }
    }
}
