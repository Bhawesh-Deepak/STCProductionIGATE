using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

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
