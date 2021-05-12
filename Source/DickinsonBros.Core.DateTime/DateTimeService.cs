using DickinsonBros.Core.DateTime.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace DickinsonBros.Core.DateTime
{
    [ExcludeFromCodeCoverage]
    public class DateTimeService : IDateTimeService
    {
        public System.DateTime GetDateTimeUTC()
        {
            return System.DateTime.UtcNow;
        }
    }
}
