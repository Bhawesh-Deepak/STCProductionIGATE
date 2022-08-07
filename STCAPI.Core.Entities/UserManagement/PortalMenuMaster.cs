using Microsoft.AspNetCore.Http;
using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("PortalMenuMaster")]
    public class PortalMenuMaster : BaseModel<int>
    {
        public string Stage { get; set; }
        public string MainStream { get; set; }
        public string StreamLongName { get; set; }
        public string Stream { get; set; }
        public string ObjectName { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Flag { get; set; }
        [NotMapped]
        public IFormFile PortalFile { get; set; }

    }
}
