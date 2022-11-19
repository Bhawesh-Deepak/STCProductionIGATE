using System;

namespace STCAPI.Model
{
    public class UpdateFormModel
    {
        public string FormId { get; set; }
        public string Decision { get; set; }
        public string Comments { get; set; }
        public string ApproverEmail { get; set; }
        public string PendingwithEmail { get; set; }
        public string RequestStatus { get; set; }
        public DateTime SentDate { get; set; }
        public string DocumentId { get; set; }
    }
}
