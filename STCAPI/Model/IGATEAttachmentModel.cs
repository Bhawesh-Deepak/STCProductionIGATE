using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace STCAPI.Model
{
    public class IGATEAttachmentModel
    {
        public List<IFormFile> Attachment { get; set; }
    }
}
