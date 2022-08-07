using STCAPI.Core.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.STCVAT
{
    [Table("STCVAT_Form")]
    public class STCVATForm : BaseModel<int>
    {
        public string HeaderLineKey { get; set; }
        public string TaxClassificationCode { get; set; }
        public string TaxCode { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public string ReconcileApprove { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public List<string> ImagesUrl { get; set; }
        public string EmailTo { get; set; }
        public bool IsEmailSend { get; set; }
        public string Comments { get; set; }
    }
}
