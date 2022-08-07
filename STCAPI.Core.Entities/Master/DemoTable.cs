using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Master
{
    [Table("DemoTable")]
    public class DemoTable : BaseModel<int>
    {
        public string DemoName { get; set; }
    }
}
