namespace CommonHelper
{
    public static class SqlQueryHelper
    {
        public const string GetVATTaxRateMapping = @"select TableName, CompanyName, TaxRateValue , TaxRateName  from vw_STCTaxRateMappingDetails";
        public const string DeActivatePreviousSubsidry = @"Call stcvat_development.usp_DeActivateInvoiceDetail(@in_companyName,@in_periodName)";
    }
}
