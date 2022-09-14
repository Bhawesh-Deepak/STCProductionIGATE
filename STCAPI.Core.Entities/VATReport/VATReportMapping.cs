using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.VATReport
{
    [Table("VATReportMapping")]
    public class VATReportMapping : BaseModel<int>
    {
        public string VATPlatForm { get; set; }
        public decimal Rate { get; set; }
        public string Source { get; set; }
        public string TaxCode { get; set; }
        public decimal TaxCodeRate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Description { get; set; }
        public string STCVATReportItemNumber { get; set; }
    }
}
