using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Test.Integration.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class TestResult
    {
        public string TestName { get; set; }
        public string TestsName { get; set; }
        public string TestGroup { get; set; }
        public bool Pass { get; set; }
        public string CorrelationId { get; set; }
        public Exception Exception { get; set; }
        public List<string> SuccessLog { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public System.Guid ExecutionId { get; set; }
        public System.Guid TestId { get; set; }
        public System.Guid TestType { get; set; }
        public string ClassName { get; set; }
    }
}
