using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.IGATE
{
    [Table("vatrequestupdate")]
    public class VATRequestUpdate : BaseModel<int>
    {
        public string FormId { get; set; }
        public string Decision { get; set; }
        public string Comments { get; set; }
        public string ApproverEmail { get; set; }
        public string PendingwithEmail { get; set; }
        public string RequestStatus { get; set; }
        public string AssignedToEmail { get; set; }
        public string DocumentId { get; set; }
    }
}
