using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("ObjectMapping")]
    public class ObjectMapping : BaseModel<int>
    {
        public string Stage { get; set; }
        public string MainStream { get; set; }
        public string Stream { get; set; }
        public string Object { get; set; }
        public string Name { get; set; }
        public byte Flag { get; set; }
    }
}
