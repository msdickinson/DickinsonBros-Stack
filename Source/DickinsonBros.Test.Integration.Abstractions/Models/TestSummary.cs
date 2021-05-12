using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DickinsonBros.Test.Integration.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TestSummary
    {
        public System.DateTime StartDateTime { get; set; }
        public System.DateTime EndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
        public string Id { get; set; }

    }
}
