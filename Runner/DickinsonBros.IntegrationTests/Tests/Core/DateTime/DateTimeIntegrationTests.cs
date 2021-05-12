using DickinsonBros.Core.DateTime.Abstractions;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Core.DateTime
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "DateTime", Group = "Core")]
    public class DateTimeIntegrationTests : IDateTimeIntegrationTests
    {
        public IDateTimeService _dateTimeService;

        public DateTimeIntegrationTests
        (
            IDateTimeService dateTimeService
        )
        {
            _dateTimeService = dateTimeService;
        }
        public async Task GetDateTimeUTC_Runs_DateIsNotNull(List<string> successLog)
        {
            var date = _dateTimeService.GetDateTimeUTC();
            Assert.IsNotNull(date, "Date is null");
            successLog.Add($"Date: {date}");

            await Task.CompletedTask.ConfigureAwait(false);
        }

    }
}
