using System.Collections.Generic;

namespace STCAPI.Model
{
    public class RequestModel
    {
        public BpmRequest bpmRequest { get; set; }
    }

    public class Detail
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Request
    {
        public List<Detail> details { get; set; }
    }

    public class BpmRequest
    {
        public string requesterEmail { get; set; }
        public string serviceCode { get; set; }
        public Request request { get; set; }
    }
}
