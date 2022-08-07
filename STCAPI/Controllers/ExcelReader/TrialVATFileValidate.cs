using CommonHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class TrialVATFileValidate : ControllerBase
    {
        private readonly IHostingEnvironment _IHostingEnviroment;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// inject required service to contructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="errorLogRepo"></param>
        public TrialVATFileValidate(IHostingEnvironment hostingEnvironment, IGenericRepository<ErrorLogModel, int> errorLogRepo)
        {
            _IHostingEnviroment = hostingEnvironment;
            _IErrorLogRepository = errorLogRepo;
        }

        /// <summary>
        /// TrialVATValidate 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> TrialVATValidate([FromForm] InvoiceDetail model)
        {
            try
            {
                var balanceDataModel = await Task.Run(() => VATTrialBalance(model.InvoiceExcelFile));
                var errorResult = VATTrialBalanceValidationRule.ValidateVATTrialBalance(balanceDataModel);
                return Ok(GetErrorDetails(errorResult)); ;
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(TrialVATFileValidate),
                     nameof(TrialVATValidate), ex.Message, ex.ToString());

                return BadRequest("The Uploaded excel file do not support !!!");
            }
        }

        #region PrivateMethod
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private List<SubsidryErrorDetail> GetErrorDetails(IDictionary<int, (string, string)> error)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private List<VATTrialBalanceModel> VATTrialBalance(IFormFile inputFile)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = inputFile.OpenReadStream();
            List<VATTrialBalanceModel> models = new List<VATTrialBalanceModel>();

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
                            var model = new VATTrialBalanceModel();
                            model.Account = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column0"]);
                            model.Description = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column1"]);
                            model.BeginingBalance = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column3"]);
                            model.Debit = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column4"]);
                            model.Credit = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column5"]);
                            model.Activity = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column6"]);
                            model.EndingBalance = Convert.ToString(inputVatInvoiceDetail.Rows[i]["Column7"]);
                            models.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return models;
        }

        #endregion
    }
}
