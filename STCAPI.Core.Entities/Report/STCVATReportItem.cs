using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.Report
{
    [Table("STCVATReportItem")]
    public  class STCVATReportItem: BaseModel<int>
    {
        public string STCVATItemForm { get; set; }
        public string STCVATReportItemNumber { get; set; }
        public string STCVATReportItemName { get; set; }
        public decimal STCVATReportItemValue { get; set; }
        public string Description { get; set; }
    }
}
