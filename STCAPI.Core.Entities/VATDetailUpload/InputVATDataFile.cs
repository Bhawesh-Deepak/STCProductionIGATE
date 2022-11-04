using STCAPI.Core.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace STCAPI.Core.Entities.VATDetailUpload
{
    [Table("InputVATDetail")]
    public class InputVATDataFile : BaseModel<int>
    {
        public string InvoiceType { get; set; }
        public string InvoiceSource { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDocNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? GLDate { get; set; }
        public decimal? TotalInvoiceAmount { get; set; }
        public string InvoiceCurrency { get; set; }
        public decimal? CurrencyExchangeRate { get; set; }
        public decimal? SARInvoiceAmount { get; set; }
        public int? SuppierNumber { get; set; }
        public string SupplierName { get; set; }
        public string SupplierSite { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierBankAccount { get; set; }
        public string SupplierVATRegistrationNumber { get; set; }
        public string SupplierVATGroupRegistrationNumber { get; set; }
        public string InvoiceStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentTerm { get; set; }
        public int? InvoiceLineNumber { get; set; }
        public string InvoiceLineDescription { get; set; }
        public string PONumber { get; set; }
        public DateTime? PoDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string PoItemNumber { get; set; }
        public string PoItemDescription { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string OriginalInvoiceNumberForDebitCreditNote { get; set; }
        public int? TaxLineNumber { get; set; }
        public string ProductType { get; set; }
        public string TaxCode { get; set; }
        public string TaxRateCode { get; set; }
        public int? TaxRate { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? SARTaxableAmount { get; set; }
        public decimal? RecoverableTaxableAmount { get; set; }
        public decimal? SARRecoverableTaxableAmount { get; set; }
        public decimal? NonRecoverableTaxAmount { get; set; }
        public decimal? SARNonRecoverableTaxAmount { get; set; }
        public string RecoverableTaxGLAccountNumber { get; set; }
        public int UploadInvoiceDetailId { get; set; }
        public string CompanyName { get; set; }
        public DateTime Period { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerVATRegistrationNumber { get; set; }
        public string BuyerVATGroupRegistrationNumber { get; set; }
    }
}
