using System;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Infrastructure.SMTP.Abstractions.Models
{
    [ExcludeFromCodeCoverage]
    public class SMTPServiceOptions
    {
        public int MaxConnections { get; set; }
        public TimeSpan EmailTimeout { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public int IdealEmailLoad { get; set; }
        public TimeSpan InactivityTimeout { get; set; }
        public TimeSpan PullingDelay { get; set; }
    }
}
