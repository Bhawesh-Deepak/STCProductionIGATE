using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("Stream")]
    public class StreamModel : BaseModel<int>
    {
        public int MainLevelId { get; set; }
        public string StreamName { get; set; }
    }
}
