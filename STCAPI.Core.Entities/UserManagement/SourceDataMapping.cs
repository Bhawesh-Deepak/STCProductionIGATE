using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("SourceDataMapping")]
    public class SourceDataMapping : BaseModel<int>
    {
        public int MainStreemId { get; set; }
        public int StreamId { get; set; }
        public int CompanyId { get; set; }
        public int SourceId { get; set; }
        public int RawDataId { get; set; }
    }
}
