using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.VATDetailUpload
{
    [Table("OutputVATDetail")]
    public class STCVATOutputModel : BaseModel<int>
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDocSequence { get; set; }
        public string InvoiceSource { get; set; }
        public string InvoiceType { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? GLDate { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public string InvoiceCurrency { get; set; }
        public decimal? CurrencyExchangeRate { get; set; }
        public decimal? SARInvoiceAmount { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string BillToAddress { get; set; }
        public string CustomerCountryName { get; set; }
        public string CustomerVATRegistrationNumber { get; set; }
        public string CustomerCommercialRegistrationNumber { get; set; }
        public string SellerName { get; set; }
        public string SellerVATRegistrationNumber { get; set; }
        public string SellerAddress { get; set; }
        public string GroupVATRegistrationNumber { get; set; }
        public string SellerCommercialNumber { get; set; }
        public string InvoiceLineNumber { get; set; }
        public string InvoiceLineDescription { get; set; }
        public string IssueDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public decimal? InvoiceLineAmount { get; set; }
        public decimal? SARInvoiceLineAmount { get; set; }
        public string TaxRateName { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? SARTaxAmount { get; set; }
        public string TaxClassificationCode { get; set; }
        public decimal? TaxRate { get; set; }
        public string TaxAccount { get; set; }
        public string ContractNumber { get; set; }
        public string ContractDescription { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string OriginalInvoice { get; set; }
        public string PoNumber { get; set; }
        public string UniversalUniqueInvoiceIndentifier { get; set; }
        public string QRCode { get; set; }
        public string PreviousInvoiceNoteHash { get; set; }
        public string InvoiceTamperResistantCounterValue { get; set; }
        public int UploadInvoiceDetailId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
        public decimal SARTaxableAmount { get; set; }
        public string CustomerVATGroupRegistrationNumber { get; set; }
        public string CustomerType { get; set; }
        public string ProductServiceGoodType { get; set; }
    }
}
