using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

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
