using Microsoft.AspNetCore.Http;
using STCAPI.Core.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.RequestDetail
{
    [Table("NewRequestDetails")]
    public class RequestDetailModel : BaseModel<int>
    {
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Attachments { get; set; }
        [NotMapped]
        public List<string> Emails { get; set; }

        [NotMapped]
        public List<IFormFile> AttachmentDetails { get; set; }
    }
}
