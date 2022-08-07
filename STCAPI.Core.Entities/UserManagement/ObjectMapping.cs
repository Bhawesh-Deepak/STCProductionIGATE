using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("ObjectMapping")]
    public class ObjectMapping : BaseModel<int>
    {
        public string Stage { get; set; }
        public string MainStream { get; set; }
        public string Stream { get; set; }
        public string Object { get; set; }
        public string Name { get; set; }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public string ObjectReference { get; set; }
        public string ObjectNumber { get; set; }
        public byte Flag { get; set; }

        [NotMapped]
        public bool IsMapped { get; set; }
    }
}
