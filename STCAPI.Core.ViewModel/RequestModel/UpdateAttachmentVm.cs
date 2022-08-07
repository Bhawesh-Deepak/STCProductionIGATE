using Microsoft.AspNetCore.Http;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class UpdateAttachmentVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile FileData { get; set; }
    }
}
