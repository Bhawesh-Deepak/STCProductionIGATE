namespace STCAPI.Core.ViewModel.RequestModel
{
    public class NewReportViewModel
    {
        public int Id { get; set; }
        public string Main_Stream { get; set; }
        public string Stream { get; set; }
        public string Report_Name { get; set; }
        public string Report_Number { get; set; }
        public string Report_Short_Name { get; set; }
        public string Report_Long_Name { get; set; }
        public string Report_Description { get; set; }
        public string CreatedByName { get; set; }
    }
}
