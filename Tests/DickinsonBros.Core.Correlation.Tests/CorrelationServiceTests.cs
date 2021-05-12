using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DickinsonBros.Core.Correlation.Tests
{
    [TestClass]
    public class CorrelationServiceTests : BaseTest
    {
        [TestMethod]
        public void CorrelationIdProperty_Get_ReturnsValue()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<ICorrelationService>();
                    var uutConcrete = (CorrelationService)uut;

                    //Act
                    var observed = uutConcrete.CorrelationId;

                    //Assert
                    Assert.IsNull(observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public void CorrelationIdProperty_Set_PropertyUpdated()
        {
            RunDependencyInjectedTest
            (
                (serviceProvider) =>
                {
                    //Setup
                    var expectedResult = "ABC";
                    var uut = serviceProvider.GetRequiredService<ICorrelationService>();
                    var uutConcrete = (CorrelationService)uut;

                    //Act
                    uutConcrete.CorrelationId = expectedResult;
                    var observed = uutConcrete._asyncLocalCorrelationId.Value;

                    //Assert
                    Assert.AreEqual(expectedResult, observed);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICorrelationService, CorrelationService>();

            return serviceCollection;
        }

    }
}
