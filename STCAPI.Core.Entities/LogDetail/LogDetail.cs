using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.LogDetail
{
    [Table("LogTable")]
    public class LogDetail : BaseModel<int>
    {
        public string ModuleName { get; set; }
        public string SubModuleName { get; set; }
        public string TableName { get; set; }
        public string Description { get; set; }
    }
}
