using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.RequestModel
{
    public class ReportCreteriaVm
    {
        public int Id { get; set; }
        public string CSVUrlPath { get; set; }
        public string Creteria { get; set; }
        public string JsonRule { get; set; }
        public string UserName { get; set; }
    }
}
