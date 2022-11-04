namespace STCAPI.Core.ViewModel.RequestModel
{
    public class OutPutVATModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDocSequence { get; set; }
        public string InvoiceSource { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceDate { get; set; }
        public string GLDate { get; set; }
        public string InvoiceAmount { get; set; }
        public string InvoiceCurrency { get; set; }
        public string CurrencyExchangeRate { get; set; }
        public string SARInvoiceAmount { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string BillToAdress { get; set; }
        public string CustomerCountryName { get; set; }
        public string CustomerVATRegistrationNumber { get; set; }
        public string CustomerCommercialRegistrationNumber { get; set; }
        public string SellerNumber { get; set; }
        public string SellerVATRegistrationNumber { get; set; }
        public string SellerAddress { get; set; }
        public string GroupVARRegistrationNumber { get; set; }
        public string SellerCommercialNumber { get; set; }
        public string InvoiceLineNumber { get; set; }
        public string InvoiceLineDescription { get; set; }
        public string IssueDate { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public string InvoiceLineAmount { get; set; }
        public string SARInvoiceLineAmount { get; set; }
        public string TaxRateName { get; set; }
        public string TaxableAmount { get; set; }
        public string SARTaxableAmount { get; set; }
        public string TaxAmount { get; set; }
        public string SARTaxAmount { get; set; }
        public string TaxClassificationCode { get; set; }
        public string TaxRate { get; set; }
        public string TaxAccount { get; set; }
        public string ContractNumber { get; set; }
        public string ContractDescription { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string OriginalInvoice { get; set; }
        public string PONumber { get; set; }
        public string UniversalUniqueInvoiceIdentifier { get; set; }
        public string QRCode { get; set; }
        public string PreviousInvoiceNoteHash { get; set; }
        public string InvoiceTamperResistantCounterValue { get; set; }
        public string SellerName { get; set; }
        public string CustomerVATGroupRegistrationNumber { get; set; }
        public string CustomerType { get; set; }
        public string ProductServiceGoodType { get; set; }
    }
}
