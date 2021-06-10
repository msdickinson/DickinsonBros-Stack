using DickinsonBros.Core.Correlation.Abstractions;
using DickinsonBros.Infrastructure.DNS.Abstractions;
using DickinsonBros.Infrastructure.DNS.Abstractions.Models;
using DickinsonBros.Test.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.DNS
{
    [ExcludeFromCodeCoverage]
    [TestAPIAttribute(Name = "DNS", Group = "Infrastructure")]
    public class DNSIntegrationTests : IDNSIntegrationTests
    {
        public readonly IDNSService _dnsService;
        public readonly ICorrelationService _correlationService;


        public DNSIntegrationTests
        (
            IDNSService dnsService,
            ICorrelationService correlationService
        )
        {
            _dnsService = dnsService;
            _correlationService = correlationService;
        }

        public async Task ValidateEmailDomainAsync_VaildEmail_ReturnsVaild(List<string> successLog)
        {
            var result = await _dnsService.ValidateEmailDomainAsync("Gmail.com");
            
            Assert.AreEqual(ValidateEmailDomainResult.Vaild, result);
            successLog.Add($"Email domain is vaild."); 
        }
        public async Task ValidateEmailDomainAsync_InvaildEmail_ReturnsVaild(List<string> successLog)
        {
            var result = await _dnsService.ValidateEmailDomainAsync("NotARealDomainFake.com");

            Assert.AreEqual(ValidateEmailDomainResult.Invaild, result);
            successLog.Add($"Email domain is not vaild.");
        }

    }
}
