using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class STCVATFormModel
    {
        public List<string> HeaderKeyIds { get; set; }
        public string TaxClassificationCode { get; set; }
        public string TaxCode { get; set; }
        public string SupplierInvoiceNumber { get; set; }
        public string ReconcileApprove { get; set; }
        public List<IFormFile> Images { get; set; }
        public string ImagePath { get; set; }
        public string UserName { get; set; }
        public List<string> EmailTo { get; set; }
        public string EmailTemplate { get; set; }
        public bool IsEmaiSend { get; set; }
        public string EmailSubject { get; set; }
        public string Comments { get; set; }
    }
}
