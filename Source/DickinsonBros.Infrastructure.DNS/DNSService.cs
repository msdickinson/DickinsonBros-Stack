using DickinsonBros.Infrastructure.DNS.Abstractions;
using DnsClient;
using System.Linq;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.DNS
{
    public class DNSService : IDNSService
    {
        internal readonly ILookupClient _lookupClient;
        public DNSService
        (
            ILookupClient lookupClient
        )
        {
            _lookupClient = lookupClient;
        }
        public async Task<bool> ValidateEmailDomainAsync(string emailDomain)
        {
            return (await _lookupClient.QueryAsync(emailDomain, QueryType.MX).ConfigureAwait(false)).Answers.MxRecords().Any();
        }
    }
}
