﻿using Microsoft.AspNetCore.Http;
using STCAPI.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace STCAPI.Core.ViewModel.RequestModel
{
    public class InvoiceDetail
    {
        public IFormFile  InvoiceExcelFile { get; set; }
        public IFormFile InvoiceOutpuExcel { get; set; }
        public IFormFile InvoiceTrialExcel { get; set; }
        public IFormFile InvoiceReturnExcel { get; set; }
        public string InvoiceName { get; set; }
        public string UserName { get; set; }
        public List<IFormFile> AttachmentList { get; set; }

        //[EnumDataType(typeof(VATExcelType))]
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        //public VATExcelType ExcelType { get; set; }
        public string Period { get; set; }
    }
}
