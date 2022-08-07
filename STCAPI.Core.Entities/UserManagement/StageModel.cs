using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("Stages")]
    public class StageModel : BaseModel<int>
    {
        public string StageName { get; set; }
    }
}
