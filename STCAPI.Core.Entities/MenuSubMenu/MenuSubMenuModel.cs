using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.MenuSubMenu
{
    [Table("MenuSubMenu")]
    public class MenuSubMenuModel : BaseModel<int>
    {
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string RouteUrl { get; set; }
    }
}
