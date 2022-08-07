using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Logger
{
    [Table("ErrorLog")]
    public class ErrorLogModel : BaseModel<int>
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ClientId { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }
        public bool Status { get; set; }
    }
}
