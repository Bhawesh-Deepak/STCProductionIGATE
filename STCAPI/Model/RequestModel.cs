using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace STCAPI.Model
{
    /// <summary>
    /// Request Model
    /// </summary>
    public class RequestModel
    {
        public BpmRequest bpmRequest { get; set; }
    }

    /// <summary>
    /// Request Details
    /// </summary>
    public class Detail
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    /// <summary>
    /// Request 
    /// </summary>
    public class Request
    {
        public List<Detail> details { get; set; }
    }

    public class BpmRequest
    {
        public string requesterEmail { get; set; }
        public string serviceCode { get; set; }
        public Request request { get; set; }
        public AttachmentModel attachmentModel { get; set; }
    }

    public class Attachment
    {
        public string fileName { get; set; }
        public string mimeType { get; set; }
        public string fileContents { get; set; }
    }

    public class AttachmentModel
    {
        public string fileName { get; set; }
        public string mimeType { get; set; }
        public string attachmentId { get; set; }
    }
}
