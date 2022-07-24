using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.Subsidry
{
    [Table("UserSubsidryMapping")]
    public class SubsidryUserMapping: BaseModel<int>
    {
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string UserEmail { get; set; }
    }
}
