using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;


namespace STCAPI.Core.Entities.Subsidry
{
    [Table("SubsidryInvoiceAttachment")]
    public class SubsidryInvoiceAttachment : BaseModel<int>
    {
        public int UploadInvoiceId { get; set; }
        public string InputAttachmentDetails { get; set; }
        public string OutPutAttachmentDetails { get; set; }
        public string TrialAttachmentDetails { get; set; }
        public string ReturnAttachmentDetails { get; set; }
    }
}
