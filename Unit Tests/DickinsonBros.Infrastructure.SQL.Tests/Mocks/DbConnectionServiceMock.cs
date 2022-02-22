using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Abstractions.Models;
using DickinsonBros.Infrastructure.SQL.Tests.Mocks.Models;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DickinsonBros.Infrastructure.SQL.Tests.Mocks
{
    public class DbConnectionServiceMock<T> : IDbConnectionService<T> 
    where T : SQLServiceOptionsType
    {
        public Mock<DbCommand> DbCommandMock;
        public Mock<DbConnection> DbConnectionMock;
        public Mock<IDbConnection> IDbConnectionMock;
        
        public string CommandTextObserved;
        public CommandType CommandTypeObserved;
        public ParameterCollection ParameterCollectionObserved;

        public Dictionary<string, object> Parameters 
        { 
            get 
            {
                var dictionary = new Dictionary<string, object>();
                foreach (var dbParameterMock in ParameterCollectionObserved._dbParameterMocks)
                {
                    dictionary.Add(dbParameterMock.Object.ParameterName, dbParameterMock.Object.Value);
                }

                return dictionary;
            }
        }

        public DbConnectionServiceMock()
        {
            //-- DbCommand
            DbCommandMock = new Mock<DbCommand>();
            var dbParameterMocks = new List<Mock<DbParameter>>();
            ParameterCollectionObserved = new ParameterCollection(dbParameterMocks);

            DbCommandMock.Protected()
                               .SetupGet<DbParameterCollection>("DbParameterCollection")
                               .Returns(() =>
                               {
                                   return ParameterCollectionObserved;
                               });

            DbCommandMock.Protected()
            .Setup<DbParameter>("CreateDbParameter")
            .Returns
            (
                () =>
                {
                    var mockDbParameter = new Mock<DbParameter>();
                    mockDbParameter.SetupAllProperties();
                    return mockDbParameter.Object;
                }
            );

            DbCommandMock
            .SetupSet(a => a.CommandType = It.IsAny<CommandType>())
            .Callback<CommandType>(value => CommandTypeObserved = value);

            DbCommandMock
            .SetupSet(a => a.CommandText = It.IsAny<string>())
            .Callback<string>(value => CommandTextObserved = value);


            //-- DbConnectionMock
            DbConnectionMock = new Mock<DbConnection>();
            IDbConnectionMock = DbConnectionMock.As<IDbConnection>();

            IDbConnectionMock
            .Setup(dbConnection => dbConnection.CreateCommand())
            .Returns(() => DbCommandMock.Object);

            IDbConnectionMock
            .SetupGet(dbConnection => dbConnection.State)
            .Returns(ConnectionState.Open);

            DbConnectionMock
            .Protected()
            .Setup<DbCommand>("CreateDbCommand")
            .Returns(DbCommandMock.Object);
        }

        public DbConnection Create()
        {
            return DbConnectionMock.Object;
        }
    }
}
