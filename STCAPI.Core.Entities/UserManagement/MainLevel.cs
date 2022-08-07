using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("MainLevel")]
    public class MainLevel : BaseModel<int>
    {
        public int StageId { get; set; }
        public string LevelName { get; set; }
    }
}
