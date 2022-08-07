using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class ReconcilationModel
    {
        public List<string> HeaderLineKey { get; set; }
        public string ReconcilationStatus { get; set; }
        public string EmailTo { get; set; }
        public string AdjustmentValue { get; set; }
        public List<IFormFile> Attachment { get; set; }
        public string Comments { get; set; }
        public bool IsEmailSend { get; set; }
        public string CreatedBy { get; set; }
        public string EmailTemplate { get; set; }
        public string EmailSubject { get; set; }
        public string ImagePath { get; set; }

    }
}
