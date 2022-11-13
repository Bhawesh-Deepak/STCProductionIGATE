using CommonHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.ExcelReader
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReturnVATFileValidate : ControllerBase
    {
        private readonly IHostingEnvironment _IHostingEnviroment;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to the controller constructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public ReturnVATFileValidate(IHostingEnvironment hostingEnvironment, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IHostingEnviroment = hostingEnvironment;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        ///  Validate Return File 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateReturnFile([FromForm] InvoiceDetail model)
        {
            try
            {
                var VATReturnModel = await Task.Run(() => VATReturnExcelData(model.InvoiceExcelFile));
                var errorResult = VATReturnValidationRule.ValidateVATReturn(VATReturnModel);

                return Ok("success");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReturnVATFileValidate),
                       nameof(ValidateReturnFile), ex.Message, ex.ToString());

                return BadRequest("Error");
            }

        }

        #region ObsoleteMethod
        private List<SubsidryErrorDetail> GetErrorDetails(IDictionary<int, (string, string)> error)
        {
            var models = new List<SubsidryErrorDetail>();
            foreach (var data in error)
            {
                var model = new SubsidryErrorDetail();
                model.PropertyName = data.Value.Item1;
                model.ErrorDetail = data.Value.Item2;
                model.rowNumber = data.Key;
                models.Add(model);
            }
            return models;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private List<VATRetunDetailModel> VATReturnExcelData(IFormFile inputFile)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = inputFile.OpenReadStream();
            List<VATRetunDetailModel> models = new List<VATRetunDetailModel>();
            List<VATRetunDetailModel> finalResult = new List<VATRetunDetailModel>();

            try
            {
                if (inputFile != null)
                {
                    if (inputFile.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (inputFile.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        message = "The file format is not supported.";

                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable inputVatInvoiceDetail = dsexcelRecords.Tables[0];

                        for (int i = 1; i < inputVatInvoiceDetail.Rows.Count; i++)
                        {
                            var model = new VATRetunDetailModel();
                            model.VATType = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column2"]);
                            model.VATTypeId = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column4"]);
                            model.VATTypeName = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column6"]);
                            model.SARAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column8"]);
                            model.SARAdjustment = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column10"]);
                            model.SARVATAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column12"]);
                            model.VATReturnDetail = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column7"]);

                            models.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            models.ForEach(data =>
            {
                if (!string.IsNullOrEmpty(data.VATTypeId))
                {
                    finalResult.Add(data);
                }
            });
            return finalResult;
        }
    }
}
