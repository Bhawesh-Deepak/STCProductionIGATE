namespace STCAPI.Core.ViewModel.RequestModel
{
    public class InputVATFileVm
    {
        public string InvoiceType { get; set; }
        public string InvoiceSource { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDocNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string GLDate { get; set; }
        public string TotalInvoiceAmount { get; set; }
        public string InvoiceCurrency { get; set; }
        public string CurrencyExchangeRate { get; set; }
        public string SARInvoiceAmount { get; set; }
        public string SuppierNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierSite { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierBankAccount { get; set; }
        public string SupplierVATRegistrationNumber { get; set; }
        public string SupplierVATGroupRegistrationNumber { get; set; }
        public string InvoiceStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public string InvoiceLineNumber { get; set; }
        public string InvoiceLineDescription { get; set; }
        public string PONumber { get; set; }
        public string PoDate { get; set; }
        public string ReleaseDate { get; set; }
        public string ReceiptNumber { get; set; }
        public string ReceiptDate { get; set; }
        public string PoItemNumber { get; set; }
        public string PoItemDescription { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }
        public string ContractNumber { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string OriginalInvoiceNumberForDebitCreditNote { get; set; }
        public string TaxLineNumber { get; set; }
        public string ProductType { get; set; }
        public string TaxCode { get; set; }
        public string TaxRateCode { get; set; }
        public string TaxRate { get; set; }
        public string TaxableAmount { get; set; }
        public string SARTaxableAmount { get; set; }
        public string RecoverableTaxableAmount { get; set; }
        public string SARRecoverableTaxableAmount { get; set; }
        public string NonRecoverableTaxAmount { get; set; }
        public string SARNonRecoverableTaxAmount { get; set; }
        public string RecoverableTaxGLAccountNumber { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerVATRegistrationNumber { get; set; }
        public string BuyerVATGroupRegistrationNumber { get; set; }
    }
}
