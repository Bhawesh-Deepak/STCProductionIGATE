using System.Collections.Generic;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class STCPostValidationModel
    {
        public string UserName { get; set; }
        public string PostValidation { get; set; }
        public List<string> HeaderKey { get; set; }
        public string CreatedBy { get; set; }
        public string EmailTemplate { get; set; }
        public string EmailSubject { get; set; }
        public string Comment { get; set; }
        public string EmailId { get; set; }
        public bool IsEmailSend { get; set; }

    }
}
