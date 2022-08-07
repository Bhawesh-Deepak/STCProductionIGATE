using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.MenuSubMenu
{
    [Table("MenuSubMenuAccess")]
    public class MenuSubMenuAccessModel : BaseModel<int>
    {
        public int SubMenuId { get; set; }
        public string UserName { get; set; }
    }
}
