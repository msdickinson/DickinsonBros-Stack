using DickinsonBros.Infrastructure.DNS.Abstractions;
using DickinsonBros.Infrastructure.DNS.AspDI.Extensions;
using DnsClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DickinsonBros.Infrastructure.DNS.AspDI.Tests.Extensions
{
    [TestClass]
    public class IServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void AddDNSService_Should_Succeed()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();

            // Act
            serviceCollection.AddDNSService();

            // Assert
            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(IDNSService) &&
                                                       serviceDefinition.ImplementationType == typeof(DNSService) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));


            Assert.IsTrue(serviceCollection.Any(serviceDefinition => serviceDefinition.ServiceType == typeof(ILookupClient) &&
                                                       serviceDefinition.ImplementationType == typeof(LookupClient) &&
                                                       serviceDefinition.Lifetime == ServiceLifetime.Singleton));
        }
    }
}
