using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.ResponseModel
{
    public  class IGATEResponseModel
    {
        public string FormId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string VATOnSale { get; set; }
        public string VATOnPurchase { get; set; }
        public string VATReturnDetails { get; set; }
        public string OtherVAT { get; set; }
        public string CurrentStatus { get; set; }
        public string Email { get; set; }
        public string Decision { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CurrentDecision { get; set; }
    }
}
