using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.UserManagement
{
    [Table("Dashboard")]
    public class DashboardMaster
    {
        public int MainStreamId { get; set; }
        public int StreamId { get; set; }
        public string DashboardName { get; set; }
        public string DashboardNumber { get; set; }
        public string DashboardShortName { get; set; }
        public string DashboardLongName { get; set; }
        public string DashboardDescription { get; set; }
    }
}
