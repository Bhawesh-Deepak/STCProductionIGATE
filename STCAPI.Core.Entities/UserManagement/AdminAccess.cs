using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("AdminAccess")]
    public  class AdminAccess : BaseModel<int>
    {
        public string UserName { get; set; }
        public bool IsAdminAccess { get; set; }
    }
}
