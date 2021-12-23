using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Core.ViewModel.ResponseModel
{
    public class SourceDataMappingVM
    {
        public int Id { get; set; }
        public string MainStreamName { get; set; }
        public string StreamName { get; set; }
        public string SourceName { get; set; }
        public string RawDataName { get; set; }
        public string CompanyName { get; set; }
    }
}
