using STCAPI.Core.ViewModel.RequestModel;
using System.Collections.Generic;

namespace CommonHelper
{
    public static class InputVATvalidationRule
    {
        public static IDictionary<int, (string, string)> ValidateInputVatData(List<InputVATFileVm> models)
        {
            IDictionary<int, (string, string)> errorResult = new Dictionary<int, (string, string)>();
            int count = 3;
            foreach (var data in models)
            {
                if (string.IsNullOrEmpty(data.InvoiceType))
                    errorResult.Add(count++, ("InvoiceType", $"Invoice Type is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceNumber))
                    errorResult.Add(count++, ("Invoice Number", $"Invoice Number is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceDocNumber))
                    errorResult.Add(count++, ("InvoiceDocNumber", $"InvoiceDocNumber is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceDate))
                    errorResult.Add(count++, ("InvoiceDate", $"InvoiceDate is mandatory"));


                if (string.IsNullOrEmpty(data.GLDate))
                    errorResult.Add(count++, ("GL Date", $"GL Date is mandatory"));

                if (string.IsNullOrEmpty(data.TotalInvoiceAmount))
                    errorResult.Add(count++, ("TotalInvoiceAmount", $"TotalInvoiceAmount is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceCurrency))
                    errorResult.Add(count++, ("InvoiceCurrency", $"InvoiceCurrency is mandatory"));

                if (!string.IsNullOrEmpty(data.InvoiceCurrency) && data.InvoiceCurrency != "SAR" && string.IsNullOrEmpty(data.CurrencyExchangeRate))
                    errorResult.Add(count++, ("CurrencyExchangeRate", $"CurrencyExchangeRate is mandatory"));

                if (string.IsNullOrEmpty(data.SARInvoiceAmount))
                    errorResult.Add(count++, ("SARInvoiceAmount", $"SARInvoiceAmount is mandatory"));

                if (string.IsNullOrEmpty(data.SuppierNumber))
                    errorResult.Add(count++, ("SuppierNumber", $"SuppierNumber is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierName))
                    errorResult.Add(count++, ("SupplierName", $"SupplierName is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierSite))
                    errorResult.Add(count++, ("SupplierSite", $"SupplierSite is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierAddress))
                    errorResult.Add(count++, ("SupplierAddress", $"SupplierAddress is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierCountry))
                    errorResult.Add(count++, ("SupplierCountry", $"SupplierCountry is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierCountry))
                    errorResult.Add(count++, ("SupplierCountry", $"SupplierCountry is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierBankAccount) && data.PaymentMethod.Trim().ToLower() == "transfer")
                    errorResult.Add(count++, ("SupplierBankAccount", $"SupplierBankAccount is mandatory"));

                if (string.IsNullOrEmpty(data.SupplierVATRegistrationNumber))
                    errorResult.Add(count++, ("SupplierVATRegistrationNumber", $"SupplierVATRegistrationNumber is mandatory"));

                if (string.IsNullOrEmpty(data.PaymentMethod) && data.InvoiceType == "VAT-E Invoice")
                    errorResult.Add(count++, ("PaymentMethod", $"PaymentMethod is mandatory"));

                if (string.IsNullOrEmpty(data.PaymentTerm))
                    errorResult.Add(count++, ("PaymentTerm", $"PaymentTerm is mandatory"));


                if (string.IsNullOrEmpty(data.InvoiceLineNumber))
                    errorResult.Add(count++, ("InvoiceLineNumber", $"InvoiceLineNumber is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceLineDescription))
                    errorResult.Add(count++, ("InvoiceLineDescription", $"InvoiceLineDescription is mandatory"));

                if (string.IsNullOrEmpty(data.InvoiceLineDescription))
                    errorResult.Add(count++, ("InvoiceLineDescription", $"InvoiceLineDescription is mandatory"));

                if (string.IsNullOrEmpty(data.Quantity) && data.InvoiceType == "VAT E-Invoice")
                    errorResult.Add(count++, ("Quantity", $"Quantity is mandatory"));

                if (string.IsNullOrEmpty(data.UnitPrice) && data.InvoiceType == "VAT E-Invoice")
                    errorResult.Add(count++, ("UnitPrice", $"UnitPrice is mandatory"));

                if (string.IsNullOrEmpty(data.DiscountAmount) && data.InvoiceType == "VAT E-Invoice")
                    errorResult.Add(count++, ("DiscountAmount", $"DiscountAmount is mandatory"));

                if (string.IsNullOrEmpty(data.DiscountPercentage) && data.InvoiceType == "VAT E-Invoice")
                    errorResult.Add(count++, ("DiscountPercentage", $"DiscountAmount is mandatory"));

                if (string.IsNullOrEmpty(data.OriginalInvoiceNumberForDebitCreditNote))
                    errorResult.Add(count++, ("OriginalInvoiceNumberForDebitCreditNote", $"OriginalInvoiceNumberForDebitCreditNote is mandatory"));

                if (string.IsNullOrEmpty(data.TaxLineNumber))
                    errorResult.Add(count++, ("TaxLineNumber", $"TaxLineNumber is mandatory"));

                if (string.IsNullOrEmpty(data.ProductType))
                    errorResult.Add(count++, ("ProductType", $"ProductType is mandatory"));

                if (string.IsNullOrEmpty(data.TaxRate))
                    errorResult.Add(count++, ("TaxRate", $"TaxRate is mandatory"));

                if (string.IsNullOrEmpty(data.TaxCode))
                    errorResult.Add(count++, ("TaxCode", $"TaxCode is mandatory"));

                if (string.IsNullOrEmpty(data.TaxRateCode))
                    errorResult.Add(count++, ("TaxRateCode", $"TaxRateCode is mandatory"));

                if (string.IsNullOrEmpty(data.TaxableAmount))
                    errorResult.Add(count++, ("TaxableAmount", $"TaxableAmount is mandatory"));

                if (string.IsNullOrEmpty(data.SARTaxableAmount))
                    errorResult.Add(count++, ("SARTaxableAmount", $"SARTaxableAmount is mandatory"));

                if (string.IsNullOrEmpty(data.RecoverableTaxableAmount))
                    errorResult.Add(count++, ("RecoverableTaxableAmount", $"RecoverableTaxableAmount is mandatory"));

                if (string.IsNullOrEmpty(data.NonRecoverableTaxAmount))
                    errorResult.Add(count++, ("NonRecoverableTaxAmount", $"NonRecoverableTaxAmount is mandatory"));


                if (string.IsNullOrEmpty(data.SARNonRecoverableTaxAmount))
                    errorResult.Add(count++, ("SARNonRecoverableTaxAmount", $"SARNonRecoverableTaxAmount is mandatory"));


                if (string.IsNullOrEmpty(data.RecoverableTaxGLAccountNumber))
                    errorResult.Add(count++, ("RecoverableTaxGLAccountNumber", $"RecoverableTaxGLAccountNumber is mandatory"));
            }
            return errorResult;

        }

    }
}
