using Microsoft.AspNetCore.Http;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class UpdateSubsidryVm
    {
        public int Id { get; set; }
        public string SubsidryName { get; set; }
        public IFormFile InvoiceFile { get; set; }
        public string UserName { get; set; }
    }
}
