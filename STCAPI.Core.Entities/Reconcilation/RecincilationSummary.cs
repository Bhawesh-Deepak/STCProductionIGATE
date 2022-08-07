using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Reconcilation
{
    [Table("ReconcilationSummary")]
    public class RecincilationSummary : BaseModel<int>
    {
        public string HeaderLineKey { get; set; }
        public string ReconcilationStatus { get; set; }
        public string EmailTo { get; set; }
        public string AdjustmentValue { get; set; }
        public string Attachment { get; set; }
        public string Comments { get; set; }
        public bool IsEmailSend { get; set; }
        public string ImagePath { get; set; }
    }
}
