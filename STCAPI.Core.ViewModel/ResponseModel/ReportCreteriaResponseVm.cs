namespace STCAPI.Core.ViewModel.ResponseModel
{
    public class ReportCreteriaResponseVm
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string ReportNumber { get; set; }
        public string Criteria { get; set; }
        public string JsonRule { get; set; }
        public string UserName { get; set; }
        public string CSVPath { get; set; }
    }
}
