using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.MenuSubMenu
{
    [Table("MenuSubMenu")]
    public class MenuSubMenuModel: BaseModel<int>
    {
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string RouteUrl { get; set; }
    }
}
