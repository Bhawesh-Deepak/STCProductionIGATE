using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.DataLayer.AdminPortal
{
    [Table("RawDataStream")]
    public class RawDataStream : BaseModel<int>
    {
        public int StreamId { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string referenceName { get; set; }
    }
}
