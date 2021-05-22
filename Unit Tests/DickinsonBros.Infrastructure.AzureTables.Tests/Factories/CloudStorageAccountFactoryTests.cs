using DickinsonBros.Infrastructure.AzureTables.Factories;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Tests.Factories
{
    [TestClass]
    public class CloudStorageAccountFactoryTests : BaseTest
    {
        [TestMethod]
        public async Task CreateCloudStorageAccount_Runs_CloudStorageAccountReturned()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var defaultEndpointsProtocol = "https";
                    var accountName = "SampleAccountName";
                    var accountKey = "SampleAccountKey";
                    var endpointSuffix = "SampleEndPointSuffix";
                    var connectionString = $"DefaultEndpointsProtocol={defaultEndpointsProtocol};AccountName={accountName};AccountKey={accountKey};EndpointSuffix={endpointSuffix}";

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICloudStorageAccountFactory>();
                    var uutConcrete = (CloudStorageAccountFactory)uut;

                    //Act
                    var observed = uutConcrete.CreateCloudStorageAccount(connectionString);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(defaultEndpointsProtocol, observed.TableEndpoint.Scheme);
                    Assert.AreEqual(accountName, observed.Credentials.AccountName);
                    Assert.AreEqual(accountKey, observed.Credentials.Key);
                    Assert.IsTrue(observed.TableEndpoint.OriginalString.Contains(endpointSuffix));

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICloudStorageAccountFactory, CloudStorageAccountFactory>();

            return serviceCollection;
        }

        #endregion
    }
}
