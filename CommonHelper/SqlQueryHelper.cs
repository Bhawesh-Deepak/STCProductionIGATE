namespace CommonHelper
{
    public static class SqlQueryHelper
    {
        public const string GetVATTaxRateMapping = @"select CompanyName, TaxRate , TaxRateName  from vw_TaxRateMapping";
        public const string DeActivatePreviousSubsidry = @"Call stcvat_development.usp_DeActivateInvoiceDetail(@in_companyName,@in_periodName)";
    }
}
