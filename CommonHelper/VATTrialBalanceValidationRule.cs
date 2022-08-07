using STCAPI.Core.ViewModel.RequestModel;
using System.Collections.Generic;

namespace CommonHelper
{
    public static class VATTrialBalanceValidationRule
    {
        public static IDictionary<int, (string, string)> ValidateVATTrialBalance(List<VATTrialBalanceModel> models)
        {
            IDictionary<int, (string, string)> errorResult = new Dictionary<int, (string, string)>();
            int count = 0;
            foreach (var data in models)
            {
                count++;

                if (string.IsNullOrEmpty(data.Account))
                    errorResult.Add(count++, ("Account", $"Account is mandatory"));
                if (string.IsNullOrEmpty(data.Description))
                    errorResult.Add(count++, ("Description", $"  Description is mandatory {data.Account } Description is blank"));
                if (string.IsNullOrEmpty(data.BeginingBalance))
                    errorResult.Add(count++, ("BeginingBalance", $"BeginingBalance is mandatory {data.Account } Begining balance is blank"));
                if (string.IsNullOrEmpty(data.Debit))
                    errorResult.Add(count++, ("Debit", $"Debit is mandatory {data.Account} debit is blank"));
                if (string.IsNullOrEmpty(data.Credit))
                    errorResult.Add(count++, ("Credit", $"Credit is mandatory {data.Account} Credit is blank"));
                if (string.IsNullOrEmpty(data.Activity))
                    errorResult.Add(count++, ("Activity", $"Activity is mandatory {data.Account} activity is blank"));
                if (string.IsNullOrEmpty(data.EndingBalance))
                    errorResult.Add(count++, ("EndingBalance", $"EndingBalance is mandatory {data.Account} Ending balance is blank"));
            }
            return errorResult;

        }
    }
}
