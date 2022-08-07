namespace STCAPI.Core.ViewModel.RequestModel
{
    public class VATRetunDetailModel
    {
        public string VATType { get; set; }
        public string VATTypeId { get; set; }
        public string VATTypeName { get; set; }
        public string SARAmount { get; set; }
        public string SARAdjustment { get; set; }
        public string SARVATAmount { get; set; }
        public string VATReturnDetail { get; set; }
        public int? UploadInvoiceDetailId { get; set; }

    }
}
