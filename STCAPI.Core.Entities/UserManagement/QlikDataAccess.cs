using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.DataLayer.AdminPortal
{
    [Table("QlikDataAccess")]
    public class QlikDataAccess : BaseModel<int>
    {
        public string StreamName { get; set; }
        public string UserName { get; set; }
        public string AppName { get; set; }
        public string AccessLevel { get; set; }
        public string DataGranularity { get; set; }
        public string ActionName { get; set; }
        public string Comments { get; set; }
    }
}
