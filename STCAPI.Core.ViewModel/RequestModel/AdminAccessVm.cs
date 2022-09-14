using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public  class AdminAccessVm
    {
        public string UserName { get; set; }
        public bool IsAdminAccess { get; set; }
        public string CreatedBy { get; set; }
    }
}
