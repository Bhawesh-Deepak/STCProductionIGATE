using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.InvoiceDetails
{
    [Table("uploadinvoicedetails")]
    public class UploadInvoiceDetails : BaseModel<int>
    {
        public string InvoiceName { get; set; }
        public string Attachments { get; set; }

    }
}
