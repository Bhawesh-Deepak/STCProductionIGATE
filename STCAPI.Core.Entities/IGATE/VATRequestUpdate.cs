using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.IGATE
{
    [Table("vatrequestupdate")]
    public class VATRequestUpdate : BaseModel<int>
    {
        public string FormId { get; set; }
        public string Decision { get; set; }
        public string Comments { get; set; }
        public string ApproverEmail { get; set; }
        public string PendingwithEmail { get; set; }
        public string RequestStatus { get; set; }
        public string AssignedToEmail { get; set; }
    }
}
