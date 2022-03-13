using CommonHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.Core.ViewModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.ExcelReader
{
    /// <summary>
    /// Validate output file details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OutputVATFileValidate : ControllerBase
    {
        private readonly IHostingEnvironment _IHostingEnviroment;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public OutputVATFileValidate(IHostingEnvironment hostingEnvironment)
        {
            _IHostingEnviroment = hostingEnvironment;
        }

        /// <summary>
        /// Validate output file and return the issue detail is any present.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ValidateOutPutFile([FromForm] InvoiceDetail model)
        {
            try
            {
                IDictionary<int, (string, string)> errorResult = new Dictionary<int, (string, string)>();


                var invoiceFiles = new List<IFormFile>() { model.InvoiceExcelFile };

                var uploadFileDetails = await new BlobHelper().UploadDocument(invoiceFiles,
                    _IHostingEnviroment);

                var OutputVATModel = await Task.Run(() => OutputVATExcelFle(model.InvoiceExcelFile));
                errorResult = OutputVATValidationRule.ValidateOutputVatData(OutputVATModel);

                var errorDetails = GetErrorDetails(errorResult);

                return Ok(errorDetails);
            }
            catch (Exception ex)
            {
                return BadRequest("Internal server error. Please contact admin");
            }

        }

        private List<OutPutVATModel> OutputVATExcelFle(IFormFile inputFile)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = inputFile.OpenReadStream();
            List<OutPutVATModel> models = new List<OutPutVATModel>();

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
                        //var newDT = DataTableHelper.MakeFirstRowAsColumnName(inputVatInvoiceDetail);
                        //var detailModels = ConvertDataTableToList.ConvertDataTable<OutPutVATModel>(newDT);
                        int row = 2;
                        for (int i = 2; i < inputVatInvoiceDetail.Rows.Count; i++)
                        {
                            var model = new OutPutVATModel();
                            model.InvoiceNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][0]);
                            model.InvoiceDocSequence = Convert.ToString(inputVatInvoiceDetail.Rows[i][1]);
                            model.InvoiceSource = Convert.ToString(inputVatInvoiceDetail.Rows[i][2]);
                            model.InvoiceType = Convert.ToString(inputVatInvoiceDetail.Rows[i][3]);
                            model.InvoiceDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][4]);
                            model.GLDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][5]);
                            model.InvoiceAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][6]);
                            model.InvoiceCurrency = Convert.ToString(inputVatInvoiceDetail.Rows[i][7]);
                            model.CurrencyExchangeRate = Convert.ToString(inputVatInvoiceDetail.Rows[i][8]);
                            model.SARInvoiceAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][9]);
                            model.CustomerNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][10]);
                            model.CustomerName = Convert.ToString(inputVatInvoiceDetail.Rows[i][11]);
                            model.BillToAdress = Convert.ToString(inputVatInvoiceDetail.Rows[i][12]);
                            model.CustomerCountryName = Convert.ToString(inputVatInvoiceDetail.Rows[i][13]);
                            model.CustomerVATRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][14]);
                            model.CustomerCommercialRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][15]);
                            model.SellerNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][16]);
                            model.SellerVATRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][17]);
                            model.SellerAddress = Convert.ToString(inputVatInvoiceDetail.Rows[i][18]);
                            model.GroupVARRegistrationNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][19]);
                            model.SellerCommercialNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][20]);
                            model.InvoiceLineNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][21]);

                            model.InvoiceLineDescription = Convert.ToString(inputVatInvoiceDetail.Rows[i][22]);
                            model.IssueDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][23]);
                            model.Quantity = Convert.ToString(inputVatInvoiceDetail.Rows[i][24]);
                            model.UnitPrice = Convert.ToString(inputVatInvoiceDetail.Rows[i][25]);
                            model.DiscountAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][26]);
                            model.DiscountPercentage = Convert.ToString(inputVatInvoiceDetail.Rows[i][27]);
                            model.PaymentMethod = Convert.ToString(inputVatInvoiceDetail.Rows[i][28]);
                            model.PaymentTerm = Convert.ToString(inputVatInvoiceDetail.Rows[i][29]);
                            model.InvoiceLineAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][30]);
                            model.SARInvoiceLineAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][31]);
                            model.TaxRateName = Convert.ToString(inputVatInvoiceDetail.Rows[i][32]);
                            model.TaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][33]);
                            model.SARTaxableAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][34]);
                            model.TaxAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][35]);
                            model.SARTaxAmount = Convert.ToString(inputVatInvoiceDetail.Rows[i][36]);
                            model.TaxClassificationCode = Convert.ToString(inputVatInvoiceDetail.Rows[i][37]);
                            model.TaxRate = Convert.ToString(inputVatInvoiceDetail.Rows[i][38]);
                            model.TaxAccount = Convert.ToString(inputVatInvoiceDetail.Rows[i][39]);
                            model.ContractNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][40]);
                            model.ContractDescription = Convert.ToString(inputVatInvoiceDetail.Rows[i][41]);

                            model.ContractStartDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][42]);
                            model.ContractEndDate = Convert.ToString(inputVatInvoiceDetail.Rows[i][43]);
                            model.OriginalInvoice = Convert.ToString(inputVatInvoiceDetail.Rows[i][44]);
                            model.PONumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][45]);
                            model.UniversalUniqueInvoiceIdentifier = Convert.ToString(inputVatInvoiceDetail.Rows[i][46]);
                            model.QRCode = Convert.ToString(inputVatInvoiceDetail.Rows[i][47]);
                            model.PreviousInvoiceNoteHash = Convert.ToString(inputVatInvoiceDetail.Rows[i][48]);
                            model.InvoiceTamperResistantCounterValue = Convert.ToString(inputVatInvoiceDetail.Rows[i][49]);
                            row++;
                            models.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }

            return models;
        }

        private List<SubsidryErrorDetail> GetErrorDetails(IDictionary<int, (string, string)> error)
        {
            var models = new List<SubsidryErrorDetail>();
            foreach (var data in error)
            {
                var model = new SubsidryErrorDetail();
                model.rowNumber = data.Key;
                model.PropertyName = data.Value.Item1;
                model.ErrorDetail = data.Value.Item2;

                models.Add(model);
            }
            return models;
        }
    }
}
