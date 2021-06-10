using System.Threading.Tasks;

namespace DickinsonBros.Infrastructure.DNS.Abstractions
{
    public interface IDNSService
    {
        public Task<bool> ValidateEmailDomainAsync(string emailDomain);
    }
}