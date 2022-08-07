using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.VATDetailUpload
{
    [Table("VATReturnDetail")]
    public class VATReturnModel : BaseModel<int>
    {
        public string VATType { get; set; }
        public decimal? VATTypeId { get; set; }
        public string VATTypeName { get; set; }
        public decimal? SARAmount { get; set; }
        public decimal? SARAdjustment { get; set; }
        public decimal? SARVATAmount { get; set; }
        public string VATReturnDetail { get; set; }
        public int UploadInvoiceDetailId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
    }
}
