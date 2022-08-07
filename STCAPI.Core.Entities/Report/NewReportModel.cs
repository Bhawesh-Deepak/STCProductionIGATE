using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Report
{
    [Table("NewReport")]
    public class NewReportModel : BaseModel<int>
    {
        public string MainStream { get; set; }
        public string Stream { get; set; }
        public string ReportName { get; set; }
        public string ReportNumber { get; set; }
        public string ReportShortName { get; set; }
        public string ReportLongName { get; set; }
        public string ReportDescription { get; set; }
    }
}
