using DickinsonBros.Infrastructure.AzureTables.Factories;
using DickinsonBros.Test.Unit;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.AzureTables.Tests.Factories
{
    [TestClass]
    public class CloudTableClientFactoryTests : BaseTest
    {
        [TestMethod]
        public async Task CreateCloudTableClient_Runs_CloudTableClientReturned()
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

                    var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

                    //--uut
                    var uut = serviceProvider.GetRequiredService<ICloudTableClientFactory>();
                    var uutConcrete = (CloudTableClientFactory)uut;

                    //Act
                    var observed = uutConcrete.CreateCloudTableClient(cloudStorageAccount);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(defaultEndpointsProtocol, observed.BaseUri.Scheme);
                    Assert.AreEqual(accountName, observed.Credentials.AccountName);
                    Assert.AreEqual(accountKey, observed.Credentials.Key);
                    Assert.IsTrue(observed.BaseUri.OriginalString.Contains(endpointSuffix));

                    await Task.CompletedTask.ConfigureAwait(false);

                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICloudTableClientFactory, CloudTableClientFactory>();

            return serviceCollection;
        }

        #endregion
    }
}
