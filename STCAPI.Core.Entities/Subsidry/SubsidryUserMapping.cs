using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Subsidry
{
    [Table("UserSubsidryMapping")]
    public class SubsidryUserMapping : BaseModel<int>
    {
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
    }
}
