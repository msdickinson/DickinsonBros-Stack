using Dickinsonbros.Core.Guid.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Dickinsonbros.Core.Guid
{
    [ExcludeFromCodeCoverage]
    public class GuidService : IGuidService
    {
        public System.Guid NewGuid()
        {
            return System.Guid.NewGuid();
        }
    }
}
