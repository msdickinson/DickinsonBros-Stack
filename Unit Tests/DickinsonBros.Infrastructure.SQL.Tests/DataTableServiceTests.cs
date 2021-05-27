using DickinsonBros.Infrastructure.SQL.Abstractions;
using DickinsonBros.Infrastructure.SQL.Models;
using DickinsonBros.Test.Unit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.SQL.Tests
{
    [TestClass]
    public class DataTableServiceTests : BaseTest
    {

        #region ToDataTable

        [TestMethod]
        public async Task ToDataTable_ExistInMemoryCache_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<StringSample>
                    {
                        new StringSample
                        {
                            Name = "1"
                        },
                        new StringSample
                        {
                            Name = "2"
                        },
                    }.AsEnumerable();

                    string expectedTableName = "SampleTableName";
                    var expectedAssemblyQualifiedName = typeof(string).AssemblyQualifiedName;
                    var dataTable = new System.Data.DataTable(expectedTableName);
                    dataTable.Columns.Add("Name", typeof(string));

                    var properties = new List<PropertyInfo>();
                    properties.AddRange(typeof(StringSample).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));


                    var expectedValue = (object)new DataTableWithPropertyInfo
                    {
                        DataTable = dataTable,
                        Properties = properties.ToArray()
                    };

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
                    memoryCache.Set(expectedAssemblyQualifiedName, expectedValue);

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);
                    var observedTwo = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Name", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(2, observed.Rows.Count);
                    Assert.AreEqual("1", observed.Rows[0]["Name"]);
                    Assert.AreEqual("2", observed.Rows[1]["Name"]);
                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToDataTable_IsAnonymousType_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<string>
                    {
                        "1",
                        "2"
                    };

                    string expectedTableName = "SampleTableName";

                    var expectedAssemblyQualifiedName = typeof(string).AssemblyQualifiedName;

                    var dataTable = new System.Data.DataTable(expectedTableName);
                    dataTable.Columns.Add("Item", typeof(string));

                    var properties = new List<PropertyInfo>();
                    properties.AddRange(typeof(AnonymousWarpper<string>).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));

                    var expectedValue = (object)new DataTableWithPropertyInfo
                    {
                        DataTable = dataTable,
                        Properties = properties.ToArray()
                    };

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Item", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(2, observed.Rows.Count);
                    Assert.AreEqual("1", observed.Rows[0]["Item"]);
                    Assert.AreEqual("2", observed.Rows[1]["Item"]);
                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToDataTable_NamedType_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<StringSample>
                    {
                        new StringSample
                        {
                            Name = "1"
                        },
                        new StringSample
                        {
                            Name = "2"
                        },
                    }.AsEnumerable();

                    string expectedTableName = "SampleTableName";

                    var expectedAssemblyQualifiedName = typeof(StringSample).AssemblyQualifiedName;

                    var dataTable = new System.Data.DataTable(expectedTableName);
                    dataTable.Columns.Add("Item", typeof(string));

                    var properties = new List<PropertyInfo>();
                    properties.AddRange(typeof(AnonymousWarpper<string>).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));

                    var expectedValue = (object)new DataTableWithPropertyInfo
                    {
                        DataTable = dataTable,
                        Properties = properties.ToArray()
                    };

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Name", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(2, observed.Rows.Count);
                    Assert.AreEqual("1", observed.Rows[0]["Name"]);
                    Assert.AreEqual("2", observed.Rows[1]["Name"]);
                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToDataTable_StringType_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<string>
                    {
                        "1",
                        "2"
                    };

                    string expectedTableName = "SampleTableName";

                    var expectedAssemblyQualifiedName = typeof(string).AssemblyQualifiedName;

                    var dataTable = new System.Data.DataTable(expectedTableName);
                    dataTable.Columns.Add("Item", typeof(string));

                    var properties = new List<PropertyInfo>();
                    properties.AddRange(typeof(AnonymousWarpper<string>).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));

                    var expectedValue = (object)new DataTableWithPropertyInfo
                    {
                        DataTable = dataTable,
                        Properties = properties.ToArray()
                    };

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Item", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(2, observed.Rows.Count);
                    Assert.AreEqual(enumerable[0], observed.Rows[0]["Item"]);
                    Assert.AreEqual(enumerable[1], observed.Rows[1]["Item"]);
                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToDataTable_ValueTypesSample_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<ValueTypesSample>
                    {
                        new ValueTypesSample
                        {
                            Bool = true,
                            Byte = 0,
                            SByte = 1,
                            Char = 'a',
                            Decimal = 2,
                            Double = 3,
                            Float = 4,
                            Int = 5,
                            UInt = 6,
                            Long = 7,
                            ULong = 8,
                            Short = 9,
                            UShort = 10,
                            BoolNullable = true,
                            ByteNullable = 11,
                            SByteNullable = 12,
                            CharNullable = 'b',
                            DecimalNullable = 13,
                            DoubleNullable = 14,
                            FloatNullable = 15,
                            IntNullable = 16,
                            UIntNullable = 17,
                            LongNullable = 18,
                            ULongNullable = 19,
                            ShortNullable = 20,
                            UShortNullable = 21,
                            DateTime = new System.DateTime(2020, 1, 1),
                            DateTimeNullable = new System.DateTime(2020, 1, 2),
                            Guid = new Guid("f0e50482-cfe7-4b6c-b02f-cfbc3003f426"),
                            GuidNullable = new Guid("dc7564fe-aac8-4e52-87d6-4a190c7a79cd"),
                            TimeSpan = new TimeSpan(1, 0, 0),
                            TimeSpanNullable = new TimeSpan(2, 0, 0)
                        }
                    };

                    string expectedTableName = "SampleTableName";
                    var expectedAssemblyQualifiedName = typeof(ValueTypesSample).AssemblyQualifiedName;

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Bool", observed.Columns[0].ColumnName);
                    Assert.AreEqual("Byte", observed.Columns[1].ColumnName);
                    Assert.AreEqual("SByte", observed.Columns[2].ColumnName);
                    Assert.AreEqual("Char", observed.Columns[3].ColumnName);
                    Assert.AreEqual("Decimal", observed.Columns[4].ColumnName);
                    Assert.AreEqual("Double", observed.Columns[5].ColumnName);
                    Assert.AreEqual("Float", observed.Columns[6].ColumnName);
                    Assert.AreEqual("Int", observed.Columns[7].ColumnName);
                    Assert.AreEqual("UInt", observed.Columns[8].ColumnName);
                    Assert.AreEqual("Long", observed.Columns[9].ColumnName);
                    Assert.AreEqual("ULong", observed.Columns[10].ColumnName);
                    Assert.AreEqual("Short", observed.Columns[11].ColumnName);
                    Assert.AreEqual("UShort", observed.Columns[12].ColumnName);
                    Assert.AreEqual("BoolNullable", observed.Columns[13].ColumnName);
                    Assert.AreEqual("ByteNullable", observed.Columns[14].ColumnName);
                    Assert.AreEqual("SByteNullable", observed.Columns[15].ColumnName);
                    Assert.AreEqual("CharNullable", observed.Columns[16].ColumnName);
                    Assert.AreEqual("DecimalNullable", observed.Columns[17].ColumnName);
                    Assert.AreEqual("DoubleNullable", observed.Columns[18].ColumnName);
                    Assert.AreEqual("FloatNullable", observed.Columns[19].ColumnName);
                    Assert.AreEqual("IntNullable", observed.Columns[20].ColumnName);
                    Assert.AreEqual("UIntNullable", observed.Columns[21].ColumnName);
                    Assert.AreEqual("LongNullable", observed.Columns[22].ColumnName);
                    Assert.AreEqual("ULongNullable", observed.Columns[23].ColumnName);
                    Assert.AreEqual("ShortNullable", observed.Columns[24].ColumnName);
                    Assert.AreEqual("UShortNullable", observed.Columns[25].ColumnName);
                    Assert.AreEqual("Guid", observed.Columns[26].ColumnName);
                    Assert.AreEqual("GuidNullable", observed.Columns[27].ColumnName);
                    Assert.AreEqual("DateTime", observed.Columns[28].ColumnName);
                    Assert.AreEqual("DateTimeNullable", observed.Columns[29].ColumnName);
                    Assert.AreEqual("TimeSpan", observed.Columns[30].ColumnName);
                    Assert.AreEqual("TimeSpanNullable", observed.Columns[31].ColumnName);

                    Assert.AreEqual(32, observed.Columns.Count);
                    Assert.AreEqual(1, observed.Rows.Count);

                    Assert.AreEqual(enumerable[0].Bool, observed.Rows[0]["Bool"]);
                    Assert.AreEqual(enumerable[0].Byte, observed.Rows[0]["Byte"]);
                    Assert.AreEqual(enumerable[0].SByte, observed.Rows[0]["SByte"]);
                    Assert.AreEqual(enumerable[0].Char, observed.Rows[0]["Char"]);
                    Assert.AreEqual(enumerable[0].Decimal, observed.Rows[0]["Decimal"]);
                    Assert.AreEqual(enumerable[0].Double, observed.Rows[0]["Double"]);
                    Assert.AreEqual(enumerable[0].Float, observed.Rows[0]["Float"]);
                    Assert.AreEqual(enumerable[0].Int, observed.Rows[0]["Int"]);
                    Assert.AreEqual(enumerable[0].UInt, observed.Rows[0]["UInt"]);
                    Assert.AreEqual(enumerable[0].Long, observed.Rows[0]["Long"]);
                    Assert.AreEqual(enumerable[0].ULong, observed.Rows[0]["ULong"]);
                    Assert.AreEqual(enumerable[0].Short, observed.Rows[0]["Short"]);
                    Assert.AreEqual(enumerable[0].UShort, observed.Rows[0]["UShort"]);
                    Assert.AreEqual(enumerable[0].BoolNullable, observed.Rows[0]["BoolNullable"]);
                    Assert.AreEqual(enumerable[0].ByteNullable, observed.Rows[0]["ByteNullable"]);
                    Assert.AreEqual(enumerable[0].SByteNullable, observed.Rows[0]["SByteNullable"]);
                    Assert.AreEqual(enumerable[0].CharNullable, observed.Rows[0]["CharNullable"]);
                    Assert.AreEqual(enumerable[0].DecimalNullable, observed.Rows[0]["DecimalNullable"]);
                    Assert.AreEqual(enumerable[0].DoubleNullable, observed.Rows[0]["DoubleNullable"]);
                    Assert.AreEqual(enumerable[0].FloatNullable, observed.Rows[0]["FloatNullable"]);
                    Assert.AreEqual(enumerable[0].IntNullable, observed.Rows[0]["IntNullable"]);
                    Assert.AreEqual(enumerable[0].UIntNullable, observed.Rows[0]["UIntNullable"]);
                    Assert.AreEqual(enumerable[0].LongNullable, observed.Rows[0]["LongNullable"]);
                    Assert.AreEqual(enumerable[0].ULongNullable, observed.Rows[0]["ULongNullable"]);
                    Assert.AreEqual(enumerable[0].ShortNullable, observed.Rows[0]["ShortNullable"]);
                    Assert.AreEqual(enumerable[0].UShortNullable, observed.Rows[0]["UShortNullable"]);
                    Assert.AreEqual(enumerable[0].Guid, observed.Rows[0]["Guid"]);
                    Assert.AreEqual(enumerable[0].GuidNullable, observed.Rows[0]["GuidNullable"]);
                    Assert.AreEqual(enumerable[0].DateTime, observed.Rows[0]["DateTime"]);
                    Assert.AreEqual(enumerable[0].DateTimeNullable, observed.Rows[0]["DateTimeNullable"]);
                    Assert.AreEqual(enumerable[0].TimeSpan, observed.Rows[0]["TimeSpan"]);
                    Assert.AreEqual(enumerable[0].TimeSpanNullable, observed.Rows[0]["TimeSpanNullable"]);
                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToDataTable_ByteArraySample_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<ByteArraySample>
                    {
                        new ByteArraySample
                        {
                            ByteArray = new byte[]{1,2,3}
                        }
                    };

                    string expectedTableName = "SampleTableName";
                    var expectedAssemblyQualifiedName = typeof(ByteArraySample).AssemblyQualifiedName;

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    //Assert
                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("ByteArray", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(1, observed.Rows.Count);
                    Assert.AreEqual(enumerable[0].ByteArray, observed.Rows[0]["ByteArray"]);

                    Assert.IsTrue(memoryCache.TryGetValue(expectedAssemblyQualifiedName, out _));

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task ToDataTable_EnumSampleWithInvaildValues_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = new List<EnumSample>
                    {
                        new EnumSample
                        {
                            Enum = (SampleEnum)4,
                            EnumNullable = (SampleEnum)5
                        }
                    };

                    string expectedTableName = "SampleTableName";
                    var expectedAssemblyQualifiedName = typeof(EnumSample).AssemblyQualifiedName;

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public async Task ToDataTable_NullEnumerable_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var enumerable = (List<EnumSample>)null;

                    string expectedTableName = "SampleTableName";
                    var expectedAssemblyQualifiedName = typeof(EnumSample).AssemblyQualifiedName;

                    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToDataTable(enumerable, expectedTableName);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region CreateDataTable

        [TestMethod]
        public async Task CreateDataTable_Runs_ReturnsExpectedDataTable()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    string expectedTableName = "SampleTableName";

                    var properties = new List<PropertyInfo>();
                    properties.AddRange(typeof(StringSample).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance));

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.CreateDataTable(properties.ToArray(), expectedTableName);

                    //Assert

                    Assert.IsNotNull(observed);
                    Assert.AreEqual(expectedTableName, observed.TableName);
                    Assert.AreEqual("Name", observed.Columns[0].ColumnName);
                    Assert.AreEqual(1, observed.Columns.Count);
                    Assert.AreEqual(0, observed.Rows.Count);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region ToUnderlyingDataTableType

        [TestMethod]
        public async Task ToUnderlyingDataTableType_NonNullableEnumType_ReturnsType()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableType(typeof(SampleEnum));

                    //Assert

                    Assert.AreEqual(typeof(int), observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToUnderlyingDataTableType_NullableNonEnumType_ReturnsType()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableType(typeof(int?));

                    //Assert

                    Assert.AreEqual(typeof(int), observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToUnderlyingDataTableType_NonNullableNonEnumType_ReturnsType()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableType(typeof(int));

                    //Assert

                    Assert.AreEqual(typeof(int), observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        //NOTE: This may cause in error.
        [TestMethod]
        public async Task ToUnderlyingDataTableType_NullableEnumType_ReturnsType()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableType(typeof(SampleEnum?));

                    //Assert

                    Assert.AreEqual(typeof(SampleEnum), observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }
        #endregion

        #region ToUnderlyingDataTableValue

        [TestMethod]
        public async Task ToUnderlyingDataTableValue_IsNull_ReturnDBNull()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    int? input = null;

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableValue(input, typeof(int?));

                    //Assert

                    Assert.AreEqual(DBNull.Value, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToUnderlyingDataTableValue_IsNullableTypeAndValueIsNotNull_ReturnsInputAsNonNullableType()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    int? input = 1;

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableValue(input, typeof(int?));

                    //Assert

                    Assert.AreEqual(1, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToUnderlyingDataTableValue_IsVaildEnum_ReturnsInputAsInt()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    SampleEnum input = SampleEnum.Blue;

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableValue(input, typeof(SampleEnum));

                    //Assert

                    Assert.AreEqual((int)input, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public async Task ToUnderlyingDataTableValue_IsNotVaildEnum_Throws()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    SampleEnum input = (SampleEnum)6;

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableValue(input, typeof(SampleEnum));

                    //Assert

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task ToUnderlyingDataTableValue_IsNotNullNullableOrEnum_ReturnsInputAsTypeSentIn()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    int input = 6;

                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.ToUnderlyingDataTableValue(input, typeof(int));

                    //Assert

                    Assert.AreEqual(input, observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region PublicProperties

        [TestMethod]
        public async Task PublicProperties_IsInterface_ReturnsPublicInstanceWithGetPropertys()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.PublicProperties(typeof(IValueTypesSample));

                    //Assert

                    Assert.AreEqual(33, observed.Length);

                    Assert.AreEqual("Name", observed[0].Name);
                    Assert.AreEqual(typeof(string), observed[0].PropertyType);

                    Assert.AreEqual("Bool", observed[1].Name);
                    Assert.AreEqual(typeof(bool), observed[1].PropertyType);

                    Assert.AreEqual("Byte", observed[2].Name);
                    Assert.AreEqual(typeof(byte), observed[2].PropertyType);

                    Assert.AreEqual("SByte", observed[3].Name);
                    Assert.AreEqual(typeof(sbyte), observed[3].PropertyType);

                    Assert.AreEqual("Char", observed[4].Name);
                    Assert.AreEqual(typeof(char), observed[4].PropertyType);

                    Assert.AreEqual("Decimal", observed[5].Name);
                    Assert.AreEqual(typeof(decimal), observed[5].PropertyType);

                    Assert.AreEqual("Double", observed[6].Name);
                    Assert.AreEqual(typeof(double), observed[6].PropertyType);

                    Assert.AreEqual("Float", observed[7].Name);
                    Assert.AreEqual(typeof(float), observed[7].PropertyType);

                    Assert.AreEqual("Int", observed[8].Name);
                    Assert.AreEqual(typeof(int), observed[8].PropertyType);

                    Assert.AreEqual("UInt", observed[9].Name);
                    Assert.AreEqual(typeof(UInt32), observed[9].PropertyType);

                    Assert.AreEqual("Long", observed[10].Name);
                    Assert.AreEqual(typeof(long), observed[10].PropertyType);

                    Assert.AreEqual("ULong", observed[11].Name);
                    Assert.AreEqual(typeof(ulong), observed[11].PropertyType);

                    Assert.AreEqual("Short", observed[12].Name);
                    Assert.AreEqual(typeof(short), observed[12].PropertyType);

                    Assert.AreEqual("UShort", observed[13].Name);
                    Assert.AreEqual(typeof(ushort), observed[13].PropertyType);

                    Assert.AreEqual("BoolNullable", observed[14].Name);
                    Assert.AreEqual(typeof(bool?), observed[14].PropertyType);

                    Assert.AreEqual("ByteNullable", observed[15].Name);
                    Assert.AreEqual(typeof(byte?), observed[15].PropertyType);

                    Assert.AreEqual("SByteNullable", observed[16].Name);
                    Assert.AreEqual(typeof(sbyte?), observed[16].PropertyType);

                    Assert.AreEqual("CharNullable", observed[17].Name);
                    Assert.AreEqual(typeof(char?), observed[17].PropertyType);

                    Assert.AreEqual("DecimalNullable", observed[18].Name);
                    Assert.AreEqual(typeof(decimal?), observed[18].PropertyType);

                    Assert.AreEqual("DoubleNullable", observed[19].Name);
                    Assert.AreEqual(typeof(double?), observed[19].PropertyType);

                    Assert.AreEqual("FloatNullable", observed[20].Name);
                    Assert.AreEqual(typeof(float?), observed[20].PropertyType);

                    Assert.AreEqual("IntNullable", observed[21].Name);
                    Assert.AreEqual(typeof(int?), observed[21].PropertyType);

                    Assert.AreEqual("UIntNullable", observed[22].Name);
                    Assert.AreEqual(typeof(uint?), observed[22].PropertyType);

                    Assert.AreEqual("LongNullable", observed[23].Name);
                    Assert.AreEqual(typeof(long?), observed[23].PropertyType);

                    Assert.AreEqual("ULongNullable", observed[24].Name);
                    Assert.AreEqual(typeof(ulong?), observed[24].PropertyType);

                    Assert.AreEqual("ShortNullable", observed[25].Name);
                    Assert.AreEqual(typeof(short?), observed[25].PropertyType);

                    Assert.AreEqual("UShortNullable", observed[26].Name);
                    Assert.AreEqual(typeof(ushort?), observed[26].PropertyType);

                    Assert.AreEqual("Guid", observed[27].Name);
                    Assert.AreEqual(typeof(Guid), observed[27].PropertyType);

                    Assert.AreEqual("GuidNullable", observed[28].Name);
                    Assert.AreEqual(typeof(Guid?), observed[28].PropertyType);

                    Assert.AreEqual("DateTime", observed[29].Name);
                    Assert.AreEqual(typeof(System.DateTime), observed[29].PropertyType);

                    Assert.AreEqual("DateTimeNullable", observed[30].Name);
                    Assert.AreEqual(typeof(System.DateTime?), observed[30].PropertyType);

                    Assert.AreEqual("TimeSpan", observed[31].Name);
                    Assert.AreEqual(typeof(TimeSpan), observed[31].PropertyType);

                    Assert.AreEqual("TimeSpanNullable", observed[32].Name);
                    Assert.AreEqual(typeof(TimeSpan?), observed[32].PropertyType);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task PublicProperties_IsNotInterface_ReturnsPublicInstanceWithGetPropertys()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.PublicProperties(typeof(ValueTypesSample));

                    //Assert

                    Assert.AreEqual(32, observed.Length);

                    Assert.AreEqual("Bool", observed[0].Name);
                    Assert.AreEqual(typeof(bool), observed[0].PropertyType);

                    Assert.AreEqual("Byte", observed[1].Name);
                    Assert.AreEqual(typeof(byte), observed[1].PropertyType);

                    Assert.AreEqual("SByte", observed[2].Name);
                    Assert.AreEqual(typeof(sbyte), observed[2].PropertyType);

                    Assert.AreEqual("Char", observed[3].Name);
                    Assert.AreEqual(typeof(char), observed[3].PropertyType);

                    Assert.AreEqual("Decimal", observed[4].Name);
                    Assert.AreEqual(typeof(decimal), observed[4].PropertyType);

                    Assert.AreEqual("Double", observed[5].Name);
                    Assert.AreEqual(typeof(double), observed[5].PropertyType);

                    Assert.AreEqual("Float", observed[6].Name);
                    Assert.AreEqual(typeof(float), observed[6].PropertyType);

                    Assert.AreEqual("Int", observed[7].Name);
                    Assert.AreEqual(typeof(int), observed[7].PropertyType);

                    Assert.AreEqual("UInt", observed[8].Name);
                    Assert.AreEqual(typeof(UInt32), observed[8].PropertyType);

                    Assert.AreEqual("Long", observed[9].Name);
                    Assert.AreEqual(typeof(long), observed[9].PropertyType);

                    Assert.AreEqual("ULong", observed[10].Name);
                    Assert.AreEqual(typeof(ulong), observed[10].PropertyType);

                    Assert.AreEqual("Short", observed[11].Name);
                    Assert.AreEqual(typeof(short), observed[11].PropertyType);

                    Assert.AreEqual("UShort", observed[12].Name);
                    Assert.AreEqual(typeof(ushort), observed[12].PropertyType);

                    Assert.AreEqual("BoolNullable", observed[13].Name);
                    Assert.AreEqual(typeof(bool?), observed[13].PropertyType);

                    Assert.AreEqual("ByteNullable", observed[14].Name);
                    Assert.AreEqual(typeof(byte?), observed[14].PropertyType);

                    Assert.AreEqual("SByteNullable", observed[15].Name);
                    Assert.AreEqual(typeof(sbyte?), observed[15].PropertyType);

                    Assert.AreEqual("CharNullable", observed[16].Name);
                    Assert.AreEqual(typeof(char?), observed[16].PropertyType);

                    Assert.AreEqual("DecimalNullable", observed[17].Name);
                    Assert.AreEqual(typeof(decimal?), observed[17].PropertyType);

                    Assert.AreEqual("DoubleNullable", observed[18].Name);
                    Assert.AreEqual(typeof(double?), observed[18].PropertyType);

                    Assert.AreEqual("FloatNullable", observed[19].Name);
                    Assert.AreEqual(typeof(float?), observed[19].PropertyType);

                    Assert.AreEqual("IntNullable", observed[20].Name);
                    Assert.AreEqual(typeof(int?), observed[20].PropertyType);

                    Assert.AreEqual("UIntNullable", observed[21].Name);
                    Assert.AreEqual(typeof(uint?), observed[21].PropertyType);

                    Assert.AreEqual("LongNullable", observed[22].Name);
                    Assert.AreEqual(typeof(long?), observed[22].PropertyType);

                    Assert.AreEqual("ULongNullable", observed[23].Name);
                    Assert.AreEqual(typeof(ulong?), observed[23].PropertyType);

                    Assert.AreEqual("ShortNullable", observed[24].Name);
                    Assert.AreEqual(typeof(short?), observed[24].PropertyType);

                    Assert.AreEqual("UShortNullable", observed[25].Name);
                    Assert.AreEqual(typeof(ushort?), observed[25].PropertyType);

                    Assert.AreEqual("Guid", observed[26].Name);
                    Assert.AreEqual(typeof(Guid), observed[26].PropertyType);

                    Assert.AreEqual("GuidNullable", observed[27].Name);
                    Assert.AreEqual(typeof(Guid?), observed[27].PropertyType);

                    Assert.AreEqual("DateTime", observed[28].Name);
                    Assert.AreEqual(typeof(System.DateTime), observed[28].PropertyType);

                    Assert.AreEqual("DateTimeNullable", observed[29].Name);
                    Assert.AreEqual(typeof(System.DateTime?), observed[29].PropertyType);

                    Assert.AreEqual("TimeSpan", observed[30].Name);
                    Assert.AreEqual(typeof(TimeSpan), observed[30].PropertyType);

                    Assert.AreEqual("TimeSpanNullable", observed[31].Name);
                    Assert.AreEqual(typeof(TimeSpan?), observed[31].PropertyType);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }



        #endregion

        #region  IsAnonymousType

        [TestMethod]
        public async Task IsAnonymousType_Primitive_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAnonymousType(typeof(byte));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAnonymousType_ValueType_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAnonymousType(typeof(byte?));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAnonymousType_String_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAnonymousType(typeof(string));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region  IsAllowedDataTableType

        [TestMethod]
        public async Task IsAllowedDataTableType_Primitive_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAllowedDataTableType(typeof(byte));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAllowedDataTableType_ValueType_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAllowedDataTableType(typeof(byte?));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAllowedDataTableType_NullableEnum_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAllowedDataTableType(typeof(SampleEnum?));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAllowedDataTableType_String_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAllowedDataTableType(typeof(string));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsAllowedDataTableType_byteArray_ReturnsTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsAllowedDataTableType(typeof(byte[]));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region IsNullableType

        [TestMethod]
        public async Task IsNullableType_TypeIsNotGeneric_ReturnFalse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsNullableType(typeof(byte));

                    //Assert

                    Assert.IsFalse(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsNullableType_TypeIsGenericAndNotTypeOfNullable_ReturnFalse()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsNullableType(typeof(GenericType<>));

                    //Assert

                    Assert.IsFalse(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        [TestMethod]
        public async Task IsNullableType_TypeIsGenericAndIsTypeOfNullable_ReturnTrue()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var uut = serviceProvider.GetRequiredService<IDataTableService>();
                    var uutConcrete = (DataTableService)uut;

                    //Act

                    var observed = uutConcrete.IsNullableType(typeof(int?));

                    //Assert

                    Assert.IsTrue(observed);

                    await Task.CompletedTask.ConfigureAwait(false);
                },
                serviceCollection => ConfigureServices(serviceCollection)
            );
        }

        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDataTableService, DataTableService>();
            serviceCollection.AddMemoryCache();
            return serviceCollection;
        }

        public interface IAddtionalExtInterface
        {
            string Middle { set; }
        }

        public interface IExtInterface : IAddtionalExtInterface
        {
            string Name { get; set; }

            string Last { set; }
        }

        public class StringSample
        {
            public string Name { get; set; }
        }

        public interface IValueTypesSample : IExtInterface
        {
            bool Bool { get; set; }
            byte Byte { get; set; }
            sbyte SByte { get; set; }
            char Char { get; set; }
            decimal Decimal { get; set; }
            double Double { get; set; }
            float Float { get; set; }
            int Int { get; set; }
            uint UInt { get; set; }
            long Long { get; set; }
            ulong ULong { get; set; }
            short Short { get; set; }
            ushort UShort { get; set; }
            bool? BoolNullable { get; set; }
            byte? ByteNullable { get; set; }
            sbyte? SByteNullable { get; set; }
            char? CharNullable { get; set; }
            decimal? DecimalNullable { get; set; }
            double? DoubleNullable { get; set; }
            float? FloatNullable { get; set; }
            int? IntNullable { get; set; }
            uint? UIntNullable { get; set; }
            long? LongNullable { get; set; }
            ulong? ULongNullable { get; set; }
            short? ShortNullable { get; set; }
            ushort? UShortNullable { get; set; }
            Guid Guid { get; set; }
            Guid? GuidNullable { get; set; }
            System.DateTime DateTime { get; set; }
            System.DateTime? DateTimeNullable { get; set; }
            TimeSpan TimeSpan { get; set; }
            TimeSpan? TimeSpanNullable { get; set; }
        }

        public class ValueTypesSample
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public char Char { get; set; }
            public decimal Decimal { get; set; }
            public double Double { get; set; }
            public float Float { get; set; }
            public int Int { get; set; }
            public uint UInt { get; set; }
            public long Long { get; set; }
            public ulong ULong { get; set; }
            public short Short { get; set; }
            public ushort UShort { get; set; }
            public bool? BoolNullable { get; set; }
            public byte? ByteNullable { get; set; }
            public sbyte? SByteNullable { get; set; }
            public char? CharNullable { get; set; }
            public decimal? DecimalNullable { get; set; }
            public double? DoubleNullable { get; set; }
            public float? FloatNullable { get; set; }
            public int? IntNullable { get; set; }
            public uint? UIntNullable { get; set; }
            public long? LongNullable { get; set; }
            public ulong? ULongNullable { get; set; }
            public short? ShortNullable { get; set; }
            public ushort? UShortNullable { get; set; }
            public Guid Guid { get; set; }
            public Guid? GuidNullable { get; set; }
            public System.DateTime DateTime { get; set; }
            public System.DateTime? DateTimeNullable { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public TimeSpan? TimeSpanNullable { get; set; }
        }

        public enum SampleEnum
        {
            Red,
            Blue
        }

        public class EnumSample
        {
            public SampleEnum Enum { get; set; }
            public SampleEnum? EnumNullable { get; set; }
        }

        public class DateTimeSample
        {
            public System.DateTime DateTime { get; set; }
            public System.DateTime? DateTimeNullable { get; set; }
        }

        public class GuidSample
        {
            public Guid Guid { get; set; }
            public Guid? GuidNullable { get; set; }
        }

        public class TimeSpanSample
        {
            public TimeSpan TimeSpan { get; set; }
            public TimeSpan TimeSpanNullable { get; set; }

        }

        public class ByteArraySample
        {
            public byte[] ByteArray { get; set; }
        }

        public class GenericType<T> { }




        #endregion

    }
}
