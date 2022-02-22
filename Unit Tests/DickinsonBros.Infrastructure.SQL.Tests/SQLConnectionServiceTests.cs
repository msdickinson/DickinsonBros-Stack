using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL.Tests
{
    [TestClass]
    public class SQLConnectionServiceTests : BaseTest
    {

        #region Create

        [TestMethod]
        public async Task Create_VaildConnectionStringFormat_ReturnsSQLConnection()
        {
            //--Options
            var sqlServiceOptions = new SQLServiceOptions<SampleType>
            {
                ConnectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
            };

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IDbConnectionService<SampleType>>();
                    var uutConcrete = (SQLConnectionService<SampleType>)uut;

                    // Act
                    var observed = uutConcrete.Create();

                    // Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(sqlServiceOptions.ConnectionString, observed.ConnectionString);
                   
                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection, sqlServiceOptions)
            );
        }

        [TestMethod]
        public async Task Create_InvaildConnectionStringFormat_Throws()
        {
            //--Options
            var sqlServiceOptions = new SQLServiceOptions<SampleType>
            {
                ConnectionString = "SampleConnectionString"
            };

            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    // Arrange

                    //--UUT
                    var uut = serviceProvider.GetRequiredService<IDbConnectionService<SampleType>>();
                    var uutConcrete = (SQLConnectionService<SampleType>)uut;

                    // Act
                    Assert.ThrowsException<ArgumentException>(() => uutConcrete.Create());
                  
                    // Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection, sqlServiceOptions)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection, SQLServiceOptions<SampleType> sqlServiceOptions)
        {
            serviceCollection.AddSingleton<IDbConnectionService<SampleType>, SQLConnectionService<SampleType>>();
            serviceCollection.AddOptions();

            var options = Options.Create(sqlServiceOptions);
            serviceCollection.AddSingleton<IOptions<SQLServiceOptions<SampleType>>>(options);
            return serviceCollection;
        }

        #endregion
    }

    internal class SampleType : SQLServiceOptionsType 
    { 
    }
}
