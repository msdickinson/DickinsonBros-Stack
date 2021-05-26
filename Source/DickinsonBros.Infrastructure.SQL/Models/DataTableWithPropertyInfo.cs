using System.Data;
using System.Reflection;

namespace DickinsonBros.Infrastructure.SQL.Models
{
    public class DataTableWithPropertyInfo
    {
        public DataTable DataTable { get; set; }
        public PropertyInfo[] Properties { get; set; }
    }
}
