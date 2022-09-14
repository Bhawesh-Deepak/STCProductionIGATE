using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("PortalUserAccess")]
    public class PortalAccess : BaseModel<int>
    {
        public int PortalId { get; set; }
        public string UserName { get; set; }
        public bool IsMapped { get; set; }
    }
}
