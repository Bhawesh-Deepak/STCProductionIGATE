using STCAPI.Core.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.Configuration
{
    [Table("configurationmastertype")]
    public class ConfigurationMaster : BaseModel<int>
    {
        public string MasterType { get; set; }
        public int StageId { get; set; }
        public int MainStreamId { get; set; }
        public int StreamId { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public decimal ReportNumber { get; set; }
        public string Description { get; set; }
        public string ConfigurationType { get; set; }
    }
}
