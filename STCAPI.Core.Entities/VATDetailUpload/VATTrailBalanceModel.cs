using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.VATDetailUpload
{
    [Table("InputVATTrialBalance")]
    public class VATTrailBalanceModel : BaseModel<int>
    {
        public string Account { get; set; }
        public string Description { get; set; }
        public decimal BeginingBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Activity { get; set; }
        public decimal EndingBalance { get; set; }
        public int UploadInvoiceDetailId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
    }
}
