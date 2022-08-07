using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("DataSource")]
    public class DataSource : BaseModel<int>
    {
        public int StreamId { get; set; }
        public string DataSourceName { get; set; }
    }
}
