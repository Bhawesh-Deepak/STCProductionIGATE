using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace STCAPI.Core.Entities.VATDetailUpload
{
    [Table("UploadInvoiceDetails")]
    public class UploadInvoiceDetail : BaseModel<int>
    {
        public string StreamName { get; set; }
        public DateTime Period { get; set; }
        public string InputVatFilePath { get; set; }
        public string OutputVatFilePath { get; set; }
        public string TrialBalanceVatFilePath { get; set; }
        public string ReturnVatFilePath { get; set; }
        public string CompanyName { get; set; }

        [NotMapped]
        public string PeriodName { get; set; }

    }
}
