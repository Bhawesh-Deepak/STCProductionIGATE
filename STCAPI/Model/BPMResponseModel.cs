using System.Collections.Generic;

namespace STCAPI.Model
{
    public class BPMResponseModel
    {
        public BPMResponseModelDetail bpmResponse { get; set; }
        public ServiceOutput serviceOutput { get; set; }
    }
    public class Message
    {
        public string type { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ErrorDetails
    {
        public List<Message> messages { get; set; }
    }

    public class ServiceOutput
    {
        public string Status { get; set; }
        public string XGlobalTransactionID { get; set; }
        public ErrorDetails errorDetails { get; set; }
    }

    public class BPMResponseModelDetail
    {

        public string BpmRequestId { get; set; }
        public string BpmRequestStatus { get; set; }
        public string AssignedToEmail { get; set; }
    }
}
