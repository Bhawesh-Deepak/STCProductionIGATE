using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.IGATE
{
    [Table("IGATERequestDetails")]
    public class IGATERequestDetails : BaseModel<int>
    {
        public string FormId { get; set; }
        public string RequestText { get; set; }
     
    }
}
