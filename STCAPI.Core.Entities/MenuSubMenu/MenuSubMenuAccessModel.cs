using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.MenuSubMenu
{
    [Table("MenuSubMenuAccess")]
    public class MenuSubMenuAccessModel:BaseModel<int>
    {
        public int SubMenuId { get; set; }
        public string UserName { get; set; }
    }
}
