using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("RawDataLink")]
    public class RawDataLink : BaseModel<int>
    {
        public int MainStreamId { get; set; }
        public int StreamId { get; set; }
        public int CompanyId { get; set; }
        public int SourceId { get; set; }
        public int RawDataId { get; set; }
    }
}
