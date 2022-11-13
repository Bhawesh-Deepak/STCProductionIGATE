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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.ExcelReader
{
    /// <summary>
    /// VAT File validate code to vaidate the VAT INPUT File, and send the error code to the client as response.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VATFileExcelFileValidate : ControllerBase
    {
        private readonly IHostingEnvironment _IHostingEnviroment;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// inject required service to controller constructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="errorLogRepository"></param>
        public VATFileExcelFileValidate(IHostingEnvironment hostingEnvironment, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IHostingEnviroment = hostingEnvironment;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        ///  Validate VAT Invoice File Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateVATFile([FromForm] InvoiceDetail model)
        {
            try
            {
                IDictionary<int, (string, string)> errorResult = new Dictionary<int, (string, string)>();


                var invoiceFiles = new List<IFormFile>() { model.InvoiceExcelFile };

                var uploadFileDetails = await new BlobHelper().UploadDocument(invoiceFiles, _IHostingEnviroment);

                var inputVATModel = await Task.Run(() => InputVATExcelData(model.InvoiceExcelFile));
                errorResult = InputVATvalidationRule.ValidateInputVatData(inputVATModel);

                var errorDetails = GetErrorDetails(errorResult);

                return Ok(errorDetails);

            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATFileExcelFileValidate),
                    nameof(ValidateVATFile), ex.Message, ex.ToString());

                return BadRequest("Error");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private List<InputVATFileVm> InputVATExcelData(IFormFile inputFile)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = inputFile.OpenReadStream();
            List<InputVATFileVm> models = new List<InputVATFileVm>();

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

                        for (int i = 2; i < inputVatInvoiceDetail.Rows.Count; i++)
                        {
                            var model = new InputVATFileVm();
                            model.InvoiceType = Convert.ToString(inputVatInvoiceDetail.Rows[i][0]);
                            model.InvoiceSource = Convert.ToString(inputVatInvoiceDetail.Rows[i][1]);
                            model.InvoiceNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][2]);
                            model.InvoiceDocNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][3]);
                            model.InvoiceDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][4]);
                            model.GLDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][5]);
                            model.TotalInvoiceAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][6]);
                            model.InvoiceCurrency = Convert.ToString(inputVatInvoiceDetail.Rows[i][7]);
                            model.CurrencyExchangeRate = Convert.ToString(inputVatInvoiceDetail.Rows[i][8]);
                            model.SARInvoiceAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][9]);
                            model.SuppierNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][10]);
                            model.SupplierName = Convert.ToString(inputVatInvoiceDetail.Rows[i][11]);
                            model.SupplierSite = Convert.ToString(inputVatInvoiceDetail.Rows[i][12]);
                            model.SupplierAddress = Convert.ToString(inputVatInvoiceDetail.Rows[i][13]);
                            model.SupplierCountry = Convert.ToString(inputVatInvoiceDetail.Rows[i][14]);
                            model.SupplierBankAccount = Convert.ToString(inputVatInvoiceDetail.Rows[i][15]);
                            model.SupplierVATRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][16]);
                            model.SupplierVATGroupRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][17]);
                            model.InvoiceStatus = Convert.ToString(inputVatInvoiceDetail.Rows[i][18]);
                            model.PaymentStatus = Convert.ToString(inputVatInvoiceDetail.Rows[i][19]);

                            model.PaymentAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][20]);
                            model.PaymentMethod = Convert.ToString(inputVatInvoiceDetail.Rows[i][21]);
                            model.PaymentTerm = Convert.ToString(inputVatInvoiceDetail.Rows[i][22]);
                            model.InvoiceLineNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][23]);
                            model.InvoiceLineDescription = Convert.ToString(inputVatInvoiceDetail.Rows[i][24]);
                            model.PONumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][25]);
                            model.PoDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][26]);
                            model.ReleaseDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][27] ?? DateTime.Now.AddYears(100));
                            model.ReceiptNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][28]);
                            model.ReceiptDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][29]);
                            model.PoItemNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][30]);

                            model.PoItemDescription = Convert.ToString(inputVatInvoiceDetail.Rows[i][31]);
                            //model.InvoiceSource = Convert.ToString(inputVatInvoiceDetail.Rows[i][31]);
                            model.Quantity = Convert.ToString(inputVatInvoiceDetail.Rows[i][32]);
                            model.UnitPrice = Convert.ToString(inputVatInvoiceDetail.Rows[i][33]);
                            model.DiscountAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][34]);
                            model.DiscountPercentage = Convert.ToString(inputVatInvoiceDetail.Rows[i][35]);
                            model.ContractNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][36]);
                            model.ContractStartDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][37] ?? DateTime.Now.AddYears(100));
                            model.ContractEndDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][38] ?? DateTime.Now.AddYears(100));
                            model.OriginalInvoiceNumberForDebitCreditNote = Convert.ToString(inputVatInvoiceDetail.Rows[i][39]);
                           
                            model.TaxLineNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][40]);
                            model.ProductType = Convert.ToString(inputVatInvoiceDetail.Rows[i][41]);
                            model.TaxCode = Convert.ToString(inputVatInvoiceDetail.Rows[i][42]);
                            model.TaxRateCode = Convert.ToString(inputVatInvoiceDetail.Rows[i][43]);
                            model.TaxRate = Convert.ToString(inputVatInvoiceDetail.Rows[i][44]);
                            model.TaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][45]);
                            model.SARTaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][46]);
                            model.RecoverableTaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][47]);
                            model.SARRecoverableTaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][48]);
                            model.NonRecoverableTaxAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][49]);
                            model.SARNonRecoverableTaxAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][50]);
                            model.RecoverableTaxGLAccountNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][51]);
                            model.BuyerName = Convert.ToString(inputVatInvoiceDetail.Rows[i][52]);
                            model.BuyerAddress = Convert.ToString(inputVatInvoiceDetail.Rows[i][53]);
                            model.BuyerVATRegistrationNumber= Convert.ToString(inputVatInvoiceDetail.Rows[i][54]);
                            model.BuyerVATGroupRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][55]);
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
                return models.ToList().DistinctBy(x=>x.PropertyName).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }
    }
}
