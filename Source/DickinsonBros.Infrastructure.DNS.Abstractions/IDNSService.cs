using DickinsonBros.Infrastructure.DNS.Abstractions.Models;
using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.DNS.Abstractions
{
    public interface IDNSService
    {
        public Task<ValidateEmailDomainResult> ValidateEmailDomainAsync(string emailDomain);
    }
}