using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.IGATE
{
    [Table("IGATEUploadDocument")]
    public  class IGATEUploadDocument : BaseModel<int>
    {
        public string DocumentId { get; set; }
        public string DocumentPath { get; set; }
    }
}
