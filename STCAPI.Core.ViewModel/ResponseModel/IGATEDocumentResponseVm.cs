using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.ResponseModel
{
    public class Response
    {
        public string documentID { get; set; }
    }

    public class IGATEDocumentResponseVm
    {
        public Response response { get; set; }
        public ServiceOutput serviceOutput { get; set; }
    }

    public class ServiceOutput
    {
        public string XGlobalTransactionID { get; set; }
    }

}
