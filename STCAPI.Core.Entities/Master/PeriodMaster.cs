using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.Master
{
    [Table("PeriodMaster")]
    public  class PeriodMaster: BaseModel<int>
    {
        public string Period { get; set; }
        public DateTime PeriodDate { get; set; }
        public int Year { get; set; }
    }
}
