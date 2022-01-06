using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class UpdateAttachmentVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile FileData { get; set; }
    }
}
