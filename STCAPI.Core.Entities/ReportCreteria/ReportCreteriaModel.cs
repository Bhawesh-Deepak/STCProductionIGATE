using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.ReportCreteria
{
    [Table("ReportCreteria")]
    public class ReportCreteriaModel: BaseModel<int>
    {
        public int ObjectMappingId { get; set; }
        public string CSVUrlPath { get; set; }
        public string Creteria { get; set; }
        public string JsonRule { get; set; }
        public string UserName { get; set; }
    }
}
