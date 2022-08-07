using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MailHelper
{
    public class EmailAttachmentDetails
    {
        public List<string> ToEmailIds { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
