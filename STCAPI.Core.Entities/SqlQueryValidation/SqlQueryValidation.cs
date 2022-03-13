using STCAPI.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.Entities.SqlQueryValidation
{
    [Table("SqlQueryValidation")]
    public class SqlQueryValidationModel: BaseModel<int>
    {
        public string UserName { get; set; }
        public string SqlQuery { get; set; }
    }
}
