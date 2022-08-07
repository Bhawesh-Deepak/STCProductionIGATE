namespace STCAPI.Core.Entities.UserManagement
{
    public class PortalAccessVm
    {
        public int Id { get; set; }
        public string StageName { get; set; }
        public string StageLongName { get; set; }
        public string StageShortName { get; set; }
        public string MainStreamName { get; set; }
        public string MainStreamLongName { get; set; }
        public string MainStreamShortName { get; set; }
        public string StreamName { get; set; }
        public string StreamLongName { get; set; }
        public string StreamShortName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectLongName { get; set; }
        public string ObjectShortName { get; set; }
        public bool Flag { get; set; }
    }
}
