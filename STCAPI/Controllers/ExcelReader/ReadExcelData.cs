using CommonHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Enums;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Subsidry;
using STCAPI.Core.Entities.VATDetailUpload;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.ExcelReader
{

    /// <summary>
    /// Validate the subsidry and Insert the information to the data base
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class ReadExcelData : ControllerBase
    {
        private readonly IHostingEnvironment _IHostingEnviroment;
        private readonly IGenericRepository<InputVATDataFile, int> _IInputVatDataFileRepository;
        private readonly IGenericRepository<VATTrailBalanceModel, int> _IVatTrialBalanceModelRepository;
        private readonly IGenericRepository<STCVATOutputModel, int> _ISTCVATOutputModelRepository;
        private readonly IGenericRepository<VATReturnModel, int> _IVATReturnModelRepository;
        private readonly IGenericRepository<UploadInvoiceDetail, int> _IUploadInvoiceRepository;
        private readonly IGenericRepository<SubsidryUserMapping, int> _IUserSubsidryUserMapping;
        private readonly IGenericRepository<SubsidryInvoiceAttachment, int> _ISubsidryInvoiceAttachmentRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;


        private readonly ILogger<ReadExcelData> _logger;
        public ReadExcelData(IHostingEnvironment hostingEnvironment,
            IGenericRepository<InputVATDataFile, int> inputVATDatFileRepository,
            IGenericRepository<VATTrailBalanceModel, int> vatTrialBalanceRepository,
            IGenericRepository<STCVATOutputModel, int> stcVatOutModelRepo,
            IGenericRepository<VATReturnModel, int> vatReturnModelRepository,
            IGenericRepository<UploadInvoiceDetail, int> uploadInvoiceRepo,
            ILogger<ReadExcelData> logger, IGenericRepository<SubsidryUserMapping, int> userSubsidryMapping,
            IGenericRepository<SubsidryInvoiceAttachment, int> subsidryInvoiceAttachmentRepository,
            IGenericRepository<ErrorLogModel, int> errorLogModelRepo)
        {
            _IHostingEnviroment = hostingEnvironment;
            _IInputVatDataFileRepository = inputVATDatFileRepository;
            _IVatTrialBalanceModelRepository = vatTrialBalanceRepository;
            _ISTCVATOutputModelRepository = stcVatOutModelRepo;
            _IVATReturnModelRepository = vatReturnModelRepository;
            _IUploadInvoiceRepository = uploadInvoiceRepo;
            _logger = logger;
            _IUserSubsidryUserMapping = userSubsidryMapping;
            _ISubsidryInvoiceAttachmentRepository = subsidryInvoiceAttachmentRepository;
            _IErrorLogRepository = errorLogModelRepo;

        }

        /// <summary>
        /// Upload the validated Excel FIle to Data Base.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadExcelData([FromForm] InvoiceDetail model)
        {
            try
            {
                IDictionary<int, (string, string)> errorResult = new Dictionary<int, (string, string)>();

                var inputAttachmentList = await new BlobHelper().UploadDocument(model.InputAttachmentList, _IHostingEnviroment);
                var outPutAttachmentList = await new BlobHelper().UploadDocument(model.OutputAttachmentList, _IHostingEnviroment);
                var returnAttachmentList = await new BlobHelper().UploadDocument(model.ReturnAttachmentList, _IHostingEnviroment);
                var trialAttachmentList = await new BlobHelper().UploadDocument(model.TrialAttachmentList, _IHostingEnviroment);

                var invoiceFiles = new List<IFormFile>() { model.InvoiceExcelFile };

                _logger.LogInformation("File attachment has been done", DateTime.Now);

                #region UploadFile In Folder

                var uploadFileDetails = await new BlobHelper().UploadDocument(invoiceFiles, _IHostingEnviroment);

                var inputFilePath = await new BlobHelper().UploadDocument(model.InvoiceExcelFile, _IHostingEnviroment);

                var outputFilePath = await new BlobHelper().UploadDocument(model.InvoiceOutpuExcel, _IHostingEnviroment);

                var trialFilePath = await new BlobHelper().UploadDocument(model.InvoiceTrialExcel, _IHostingEnviroment);

                var returnFilePath = await new BlobHelper().UploadDocument(model.InvoiceReturnExcel, _IHostingEnviroment);


                var companyDetail = await _IUserSubsidryUserMapping.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var company = companyDetail.TEntities.ToList().Find(x => x.UserName.ToLower().Trim() == model.UserName.ToLower().Trim());
                #endregion

                var uploadInvoiceId = await CreateUploadInvocie(model, null, inputFilePath, outputFilePath,
                    returnFilePath, trialFilePath, company?.CompanyName ?? String.Empty);

                await UploadSubsidryAttachment(model, inputAttachmentList, outPutAttachmentList, returnAttachmentList, trialAttachmentList, uploadInvoiceId);

                #region Invoice Input File
                //Validate and Insert the Invoice Excel file
                var inputVATModel = await Task.Run(() => InputVATExcelData(model.InvoiceExcelFile));
                var dbModels = await ConvertInputModelToDBModel(inputVATModel);
                dbModels.ForEach(data =>
                {
                    data.UploadInvoiceDetailId = uploadInvoiceId;
                });
                var inputDataFileResponse = await CreateInputVATDetail(dbModels, model.UserName);

                #endregion

                #region Invoice OutPutFile

                var outputVATModel = await Task.Run(() => OutputVATExcelFle(model.InvoiceOutpuExcel));
                var dtoModel = await DTOOutModelToOutputDataModel(outputVATModel);
                dtoModel.ForEach(data =>
                {
                    data.UploadInvoiceDetailId = uploadInvoiceId;
                });
                var dbResponse = await CreateSTCOutputModel(dtoModel, model.UserName);
                #endregion

                #region InvoiceReturnFile

                var returnModel = await Task.Run(() => VATReturnExcelData(model.InvoiceReturnExcel));
                //var dbReturnModels = await DTOConvertModelTODBObject(returnModel);
                List<VATReturnModel> returnModels = new List<VATReturnModel>();

                returnModel.ForEach(data =>
                {
                    returnModels.Add(

                        new VATReturnModel()
                        {
                            CreatedBy = model.UserName,
                            SARAdjustment = Convert.ToDecimal(data.SARAdjustment == String.Empty ? 0 : data.SARAdjustment),
                            SARAmount = Convert.ToDecimal(data.SARAmount == String.Empty ? 0 : data.SARAmount),
                            UploadInvoiceDetailId = uploadInvoiceId,
                            SARVATAmount = Convert.ToDecimal(data.SARVATAmount == String.Empty ? 0 : data.SARVATAmount),
                            VATReturnDetail = data.VATReturnDetail ?? String.Empty,
                            VATType = data.VATType ?? String.Empty,
                            VATTypeId = Convert.ToDecimal(data.VATTypeId ?? "0"),
                            VATTypeName = data.VATTypeName ?? String.Empty,
                            CreatedDate = DateTime.Now
                        });
                });


                var dbReturnResponse = await CreateVATReturnData(returnModels, model.UserName);
                #endregion

                #region TrialBalanceFile

                var balanceDataModel = await Task.Run(() => VATTrialBalance(model.InvoiceTrialExcel));
                var trialDBModels = await ConvertVATTrialBalanceModelToDBModel(balanceDataModel);
                trialDBModels.ForEach(data =>
                {
                    data.UploadInvoiceDetailId = uploadInvoiceId;
                });
                var trialDataResponse = await CreateTrialBalance(trialDBModels, model.UserName);

                #endregion

                return await Task.Run(() => Ok("FIle Uploaded and Managed..."));
            }
            catch (Exception ex)
            {
                string exception = ex.Message;

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReadExcelData),
                   nameof(UploadExcelData), exception, ex.ToString());
            }
            return await Task.Run(() => BadRequest("Somthing wents wrong Please contact admin Team.."));
        }

        /// <summary>
        /// Uploda the Subsidry Attachemnent details
        /// </summary>
        /// <param name="model"></param>
        /// <param name="inputAttachmentList"></param>
        /// <param name="outPutAttachmentList"></param>
        /// <param name="returnAttachmentList"></param>
        /// <param name="trialAttachmentList"></param>
        /// <param name="uploadInvoiceId"></param>
        /// <returns></returns>


        /// <summary>
        /// Get all the Upload Excel FIle Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetExcelArchiveDetails()
        {
            try
            {
                var response = await _IUploadInvoiceRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReadExcelData),
                   nameof(GetExcelArchiveDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact Admin Team !");
            }

        }

        /// <summary>
        /// Update the subsisdry Information details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSubsidry([FromForm] UpdateSubsidryVm model)
        {
            try
            {
                var attachment = await new BlobHelper().UploadDocument(model.InvoiceFile, _IHostingEnviroment);

                switch (model.SubsidryName)
                {
                    case "InputInvoice":
                        var uploadInvoiceDetails = await _IInputVatDataFileRepository.GetAllEntities(x => x.UploadInvoiceDetailId == model.Id);

                        uploadInvoiceDetails.TEntities.ToList().ForEach(te =>
                        {
                            te.IsActive = false;
                            te.IsDeleted = true;
                            te.UpdatedBy = model.UserName;
                            te.UpdatedDate = DateTime.Now;
                        });

                        var deleteResponse = await _IInputVatDataFileRepository.DeleteEntity(uploadInvoiceDetails.TEntities.ToArray());

                        var inputVATModel = await Task.Run(() => InputVATExcelData(model.InvoiceFile));
                        var dbModels = await ConvertInputModelToDBModel(inputVATModel);
                        dbModels.ForEach(data =>
                        {
                            data.UploadInvoiceDetailId = model.Id;

                        });
                        var inputDataFileResponse = await CreateInputVATDetail(dbModels, model.UserName);

                        break;

                    case "OutputInvoice":
                        var outputInvoiceDetails = await _ISTCVATOutputModelRepository.GetAllEntities(x => x.UploadInvoiceDetailId == model.Id);

                        outputInvoiceDetails.TEntities.ToList().ForEach(te =>
                        {
                            te.IsActive = false;
                            te.IsDeleted = true;
                        });

                        var deleteOutputInvoiceResponse = await _ISTCVATOutputModelRepository.DeleteEntity(outputInvoiceDetails.TEntities.ToArray());

                        var outputVATModel = await Task.Run(() => OutputVATExcelFle(model.InvoiceFile));
                        var dtoModel = await DTOOutModelToOutputDataModel(outputVATModel);
                        dtoModel.ForEach(data =>
                        {
                            data.UploadInvoiceDetailId = model.Id;
                        });
                        var dbResponse = await CreateSTCOutputModel(dtoModel, model.UserName);

                        break;

                    case "ReturnInvoice":

                        var deleteModels = await _IVATReturnModelRepository.GetAllEntities(x => x.UploadInvoiceDetailId == model.Id);

                        deleteModels.TEntities.ToList().ForEach((te) =>
                        {
                            te.IsActive = false;
                            te.IsDeleted = true;
                        });

                        var deleteReturnInvoiceResponse = await _IVATReturnModelRepository.DeleteEntity(deleteModels.TEntities.ToArray());

                        var returnModel = await Task.Run(() => VATReturnExcelData(model.InvoiceFile));

                        List<VATReturnModel> returnModels = new List<VATReturnModel>();

                        returnModel.ForEach(data =>
                        {
                            returnModels.Add(

                                new VATReturnModel()
                                {
                                    CreatedBy = model.UserName,
                                    SARAdjustment = Convert.ToDecimal(data.SARAdjustment == String.Empty ? 0 : data.SARAdjustment),
                                    SARAmount = Convert.ToDecimal(data.SARAmount == String.Empty ? 0 : data.SARAmount),
                                    UploadInvoiceDetailId = model.Id,
                                    SARVATAmount = Convert.ToDecimal(data.SARVATAmount == String.Empty ? 0 : data.SARVATAmount),
                                    VATReturnDetail = data.VATReturnDetail ?? String.Empty,
                                    VATType = data.VATType ?? String.Empty,
                                    VATTypeId = Convert.ToDecimal(data.VATTypeId ?? "0"),
                                    VATTypeName = data.VATTypeName ?? String.Empty,
                                    CreatedDate = DateTime.Now
                                });
                        });


                        var dbReturnResponse = await CreateVATReturnData(returnModels, model.UserName);

                        break;

                    case "TrialInvoice":
                        var deleteTrialBalanceDetail = await _IVatTrialBalanceModelRepository.GetAllEntities(x => x.UploadInvoiceDetailId == model.Id);

                        deleteTrialBalanceDetail.TEntities.ToList().ForEach(x =>
                        {
                            x.IsActive = false;
                            x.IsDeleted = true;
                        });

                        var trialDeleteResponse = await _IVatTrialBalanceModelRepository.DeleteEntity(deleteTrialBalanceDetail.TEntities.ToArray());

                        var balanceDataModel = await Task.Run(() => VATTrialBalance(model.InvoiceFile));
                        var trialDBModels = await ConvertVATTrialBalanceModelToDBModel(balanceDataModel);
                        trialDBModels.ForEach(data =>
                        {
                            data.UploadInvoiceDetailId = model.Id;
                        });
                        var trialDataResponse = await CreateTrialBalance(trialDBModels, model.UserName);

                        break;

                }

                return Ok("success");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReadExcelData),
                 nameof(UpdateSubsidry), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSubsidryAttachment(int invoiceId)
        {
            try
            {
                var responseData = await _ISubsidryInvoiceAttachmentRepository.GetAllEntities(x => x.UploadInvoiceId == invoiceId);
                var models = new List<SubsidryAttachmentDetailResponse>();

                responseData.TEntities.ToList().ForEach(data =>
                {
                    models.Add(new SubsidryAttachmentDetailResponse()
                    {
                        Id = data.Id,
                        InputAttachments = data.InputAttachmentDetails,
                        TrialAttachments = data.TrialAttachmentDetails,
                        OutputAttachments = data.OutPutAttachmentDetails,
                        ReturnAttachments = data.ReturnAttachmentDetails,
                    });
                });
                return Ok(models);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReadExcelData),
                     nameof(GetSubsidryAttachment), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAttachment([FromForm] UpdateAttachmentVm model)
        {
            try
            {
                var inputAttachmentList = await new BlobHelper().UploadDocument(model.FileData, _IHostingEnviroment);
                var responseData = await _ISubsidryInvoiceAttachmentRepository.GetAllEntities(x => x.Id == model.Id);
                switch (model.Name)
                {
                    case "Input":
                        responseData.TEntities.ToList().ForEach(data =>
                        {
                            data.InputAttachmentDetails = inputAttachmentList;
                        });
                        break;
                    case "Trial":
                        responseData.TEntities.ToList().ForEach(data =>
                        {
                            data.TrialAttachmentDetails = inputAttachmentList;
                        });
                        break;
                    case "Output":
                        responseData.TEntities.ToList().ForEach(data =>
                        {
                            data.OutPutAttachmentDetails = inputAttachmentList;
                        });
                        break;
                    case "Return":
                        responseData.TEntities.ToList().ForEach(data =>
                        {
                            data.ReturnAttachmentDetails = inputAttachmentList;
                        });
                        break;
                    default:
                        break;
                }

                var updateResponse = await _ISubsidryInvoiceAttachmentRepository.UpdateEntity(responseData.TEntities.FirstOrDefault());
                return Ok("Success");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReadExcelData),
                        nameof(UpdateAttachment), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }


        #region PrivateMethod

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

        private async Task<int> CreateUploadInvocie(InvoiceDetail model, List<string> attchementList, string inputFilePath, string outPutFilePath, string returnFilePath, string trialBalancePath, string companyName)
        {
            try
            {
                var invoiceModel = new UploadInvoiceDetail()
                {
                    StreamName = model.InvoiceName,
                    CreatedBy = model.UserName,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    Period = model.Period,
                    InputVatFilePath = inputFilePath,
                    OutputVatFilePath = outPutFilePath,
                    TrialBalanceVatFilePath = trialBalancePath,
                    ReturnVatFilePath = returnFilePath,
                    CompanyName = companyName
                };

                var response = await _IUploadInvoiceRepository.CreateEntity(new List<UploadInvoiceDetail>() { invoiceModel }.ToArray());

                if (response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error)
                {
                    var uploadInvoices = await _IUploadInvoiceRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                    return uploadInvoices.TEntities.Max(x => x.Id);
                }

                return await Task.Run(() => 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

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
                            model.PoItemNumber = Convert.ToString(inputVatInvoiceDetail.Rows[i][29]);
                            model.PoItemDescription = Convert.ToString(inputVatInvoiceDetail.Rows[i][30]);
                            model.InvoiceSource = Convert.ToString(inputVatInvoiceDetail.Rows[i][31]);
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

        private async Task<List<InputVATDataFile>> ConvertInputModelToDBModel(List<InputVATFileVm> models)
        {
            try
            {
                List<InputVATDataFile> dbModels = new List<InputVATDataFile>();
                foreach (var item in models)
                {

                    var dbModel = new InputVATDataFile();
                    dbModel.InvoiceType = item.InvoiceType;
                    dbModel.InvoiceSource = item.InvoiceSource;
                    dbModel.InvoiceNumber = item.InvoiceNumber;
                    dbModel.InvoiceDocNumber = item.InvoiceDocNumber;
                    dbModel.InvoiceDate = IsValidDateTime(item.InvoiceDate) ? item.InvoiceDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.GLDate = IsValidDateTime(item.GLDate) ? item.GLDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.TotalInvoiceAmount = item.TotalInvoiceAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.InvoiceCurrency = item.InvoiceCurrency;
                    dbModel.CurrencyExchangeRate = item.CurrencyExchangeRate.GetDefaultIfStringNull<decimal>();
                    dbModel.SARInvoiceAmount = item.SARInvoiceAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.SuppierNumber = item.SuppierNumber.GetDefaultIfStringNull<int>();
                    dbModel.SupplierName = item.SupplierName.GetDefaultIfStringNull<string>();
                    dbModel.SupplierSite = item.SupplierSite.GetDefaultIfStringNull<string>();
                    dbModel.SupplierAddress = item.SupplierAddress.GetDefaultIfStringNull<string>();
                    dbModel.SupplierCountry = item.SupplierCountry.GetDefaultIfStringNull<string>();
                    dbModel.SupplierBankAccount = item.SupplierBankAccount.GetDefaultIfStringNull<string>();
                    dbModel.SupplierVATRegistrationNumber = item.SupplierVATRegistrationNumber.GetDefaultIfStringNull<string>();
                    dbModel.SupplierVATGroupRegistrationNumber = item.SupplierVATGroupRegistrationNumber.GetDefaultIfStringNull<string>();
                    dbModel.InvoiceStatus = item.InvoiceStatus.GetDefaultIfStringNull<string>();
                    dbModel.PaymentStatus = item.PaymentStatus.GetDefaultIfStringNull<string>();
                    dbModel.PaymentAmount = item.PaymentAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.PaymentMethod = item.PaymentMethod.GetDefaultIfStringNull<string>();
                    dbModel.PaymentTerm = item.PaymentTerm.GetDefaultIfStringNull<string>();
                    dbModel.InvoiceLineNumber = item.InvoiceLineNumber.GetDefaultIfStringNull<int>();
                    dbModel.InvoiceLineDescription = item.InvoiceLineDescription.GetDefaultIfStringNull<string>();
                    dbModel.PONumber = item.PONumber.GetDefaultIfStringNull<string>();
                    dbModel.PoDate = IsValidDateTime(item.PoDate) ? item.PoDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.ReleaseDate = IsValidDateTime(item.ReleaseDate) ? item.ReleaseDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.ReceiptNumber = item.ReceiptNumber.GetDefaultIfStringNull<string>();
                    dbModel.ReceiptDate = IsValidDateTime(item.ReceiptDate) ? item.ReceiptDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.PoItemNumber = item.PoItemNumber.GetDefaultIfStringNull<string>();
                    dbModel.PoItemDescription = item.PoItemDescription.GetDefaultIfStringNull<string>();
                    dbModel.Quantity = item.Quantity.GetDefaultIfStringNull<decimal>();
                    dbModel.UnitPrice = item.UnitPrice.GetDefaultIfStringNull<decimal>();
                    dbModel.DiscountAmount = item.DiscountAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.DiscountPercentage = item.DiscountPercentage.GetDefaultIfStringNull<decimal>();
                    dbModel.ContractNumber = item.ContractNumber.GetDefaultIfStringNull<string>();
                    dbModel.ContractStartDate = IsValidDateTime(item.ContractStartDate) ? item.ContractStartDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.ContractEndDate = IsValidDateTime(item.ContractEndDate) ? item.ContractEndDate.GetDefaultIfStringNull<DateTime>() : null;
                    dbModel.OriginalInvoiceNumberForDebitCreditNote = item.OriginalInvoiceNumberForDebitCreditNote;
                    dbModel.TaxLineNumber = item.TaxLineNumber.GetDefaultIfStringNull<int>();
                    dbModel.ProductType = item.ProductType;
                    dbModel.TaxCode = item.TaxCode;
                    dbModel.TaxRateCode = item.TaxRateCode;
                    dbModel.TaxRate = item.TaxRate.GetDefaultIfStringNull<int>();
                    dbModel.TaxableAmount = item.TaxableAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.SARTaxableAmount = item.SARTaxableAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.RecoverableTaxableAmount = item.RecoverableTaxableAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.SARRecoverableTaxableAmount = item.SARRecoverableTaxableAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.NonRecoverableTaxAmount = item.NonRecoverableTaxAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.SARNonRecoverableTaxAmount = item.SARNonRecoverableTaxAmount.GetDefaultIfStringNull<decimal>();
                    dbModel.RecoverableTaxGLAccountNumber = item.RecoverableTaxGLAccountNumber;

                    dbModels.Add(dbModel);
                };

                return await Task.Run(() => dbModels);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private bool IsValidDateTime(string dateTime)
        {
            return DateTime.TryParse(dateTime, out DateTime ouDate);
        }
        private async Task<bool> CreateInputVATDetail(List<InputVATDataFile> models, string userName)
        {
            try
            {
                models.ForEach(item =>
                {
                    item.CreatedBy = userName;
                });

                var response = await _IInputVatDataFileRepository.CreateEntity(models.ToArray());
                return response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task<List<VATTrailBalanceModel>> ConvertVATTrialBalanceModelToDBModel(List<VATTrialBalanceModel> models)
        {
            try
            {
                var dbModels = new List<VATTrailBalanceModel>();

                foreach (var data in models)
                {
                    var dbModel = new VATTrailBalanceModel();
                    dbModel.Account = data.Account.GetDefaultIfStringNull<string>();
                    dbModel.Description = data.Description.GetDefaultIfStringNull<string>();
                    dbModel.BeginingBalance = data.BeginingBalance.GetDefaultIfStringNull<decimal>();
                    dbModel.Debit = data.Debit.GetDefaultIfStringNull<decimal>();
                    dbModel.Credit = data.Credit.GetDefaultIfStringNull<decimal>();
                    dbModel.Activity = data.Activity.GetDefaultIfStringNull<decimal>();
                    dbModel.EndingBalance = data.EndingBalance.GetDefaultIfStringNull<decimal>();

                    dbModels.Add(dbModel);

                }

                return await Task.Run(() => dbModels);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task<bool> CreateTrialBalance(List<VATTrailBalanceModel> models, string userName)
        {
            try
            {
                models.ForEach(item =>
                {
                    item.CreatedBy = userName;
                });

                var response = await _IVatTrialBalanceModelRepository.CreateEntity(models.ToArray());
                return response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task<List<STCVATOutputModel>> DTOOutModelToOutputDataModel(List<OutPutVATModel> models)
        {
            var dtoModels = new List<STCVATOutputModel>();
            try
            {
                foreach (var item in models)
                {


                    var dtoModel = new STCVATOutputModel();
                    dtoModel.InvoiceNumber = item.InvoiceNumber.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceDocSequence = item.InvoiceDocSequence.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceSource = item.InvoiceSource.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceType = item.InvoiceType.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceDate = IsValidDateTime(item.InvoiceDate) ? item.InvoiceDate.GetDefaultIfStringNull<DateTime>() : null;
                    dtoModel.GLDate = IsValidDateTime(item.GLDate) ? item.GLDate.GetDefaultIfStringNull<DateTime>() : null;
                    dtoModel.InvoiceAmount = item.InvoiceAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.InvoiceCurrency = item.InvoiceCurrency.GetDefaultIfStringNull<string>();
                    dtoModel.CurrencyExchangeRate = item.CurrencyExchangeRate.GetDefaultIfStringNull<decimal>();
                    dtoModel.SARInvoiceAmount = item.SARInvoiceAmount.GetDefaultIfStringNull<decimal>();

                    dtoModel.CustomerNumber = item.CustomerNumber.GetDefaultIfStringNull<string>();
                    dtoModel.CustomerName = item.CustomerName.GetDefaultIfStringNull<string>();
                    dtoModel.BillToAddress = item.BillToAdress.GetDefaultIfStringNull<string>();
                    dtoModel.CustomerCountryName = item.CustomerCountryName.GetDefaultIfStringNull<string>();
                    dtoModel.CustomerVATRegistrationNumber = item.CustomerVATRegistrationNumber.GetDefaultIfStringNull<string>();
                    dtoModel.CustomerCommercialRegistrationNumber = item.CustomerCommercialRegistrationNumber.GetDefaultIfStringNull<string>();
                    dtoModel.SellerName = item.SellerName.GetDefaultIfStringNull<string>();
                    dtoModel.SellerVATRegistrationNumber = item.SellerVATRegistrationNumber.GetDefaultIfStringNull<string>();
                    dtoModel.SellerAddress = item.SellerAddress.GetDefaultIfStringNull<string>();
                    dtoModel.GroupVATRegistrationNumber = item.GroupVARRegistrationNumber.GetDefaultIfStringNull<string>();

                    dtoModel.SellerCommercialNumber = item.SellerCommercialNumber.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceLineNumber = item.InvoiceLineNumber.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceLineDescription = item.InvoiceLineDescription.GetDefaultIfStringNull<string>();
                    dtoModel.IssueDate = item.IssueDate.GetDefaultIfStringNull<string>();
                    dtoModel.Quantity = item.Quantity.GetDefaultIfStringNull<decimal>();
                    dtoModel.UnitPrice = item.UnitPrice.GetDefaultIfStringNull<decimal>();
                    dtoModel.DiscountAmount = item.DiscountAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.DiscountPercentage = item.DiscountPercentage.GetDefaultIfStringNull<decimal>();
                    dtoModel.PaymentMethod = item.PaymentMethod.GetDefaultIfStringNull<string>();
                    dtoModel.PaymentTerm = item.PaymentTerm.GetDefaultIfStringNull<string>();


                    dtoModel.InvoiceLineAmount = item.InvoiceLineAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.SARInvoiceLineAmount = item.SARInvoiceLineAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.TaxRateName = item.TaxRateName.GetDefaultIfStringNull<string>();
                    dtoModel.TaxableAmount = item.TaxableAmount.GetDefaultIfStringNull<decimal>();

                    dtoModel.SARTaxAmount = item.SARTaxAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.TaxAmount = item.TaxAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.SARTaxAmount = item.SARTaxAmount.GetDefaultIfStringNull<decimal>();
                    dtoModel.TaxClassificationCode = item.TaxClassificationCode.GetDefaultIfStringNull<string>();
                    dtoModel.TaxRate = item.TaxRate.GetDefaultIfStringNull<decimal>();
                    dtoModel.TaxAccount = item.TaxAccount.GetDefaultIfStringNull<string>();


                    dtoModel.ContractNumber = item.ContractNumber.GetDefaultIfStringNull<string>();
                    dtoModel.ContractDescription = item.ContractDescription.GetDefaultIfStringNull<string>();

                    dtoModel.ContractStartDate = IsValidDateTime(item.ContractStartDate) ? item.ContractStartDate.GetDefaultIfStringNull<DateTime>() : null;
                    dtoModel.ContractEndDate = IsValidDateTime(item.ContractEndDate) ? item.ContractEndDate.GetDefaultIfStringNull<DateTime>() : null;
                    dtoModel.OriginalInvoice = item.OriginalInvoice.GetDefaultIfStringNull<string>();
                    dtoModel.PoNumber = item.PONumber.GetDefaultIfStringNull<string>();
                    dtoModel.UniversalUniqueInvoiceIndentifier = item.UniversalUniqueInvoiceIdentifier.GetDefaultIfStringNull<string>();
                    dtoModel.QRCode = item.QRCode.GetDefaultIfStringNull<string>();
                    dtoModel.PreviousInvoiceNoteHash = item.PreviousInvoiceNoteHash.GetDefaultIfStringNull<string>();
                    dtoModel.InvoiceTamperResistantCounterValue = item.InvoiceTamperResistantCounterValue.GetDefaultIfStringNull<string>();


                    dtoModels.Add(dtoModel);

                };
                return await Task.Run(() => dtoModels);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private async Task<bool> CreateSTCOutputModel(List<STCVATOutputModel> models, string userName)
        {
            try
            {
                models.ForEach(data =>
                {
                    data.CreatedBy = userName;
                    data.CreatedDate = DateTime.Now;

                });
                var response = await _ISTCVATOutputModelRepository.CreateEntity(models.ToArray());
                return response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #region ObsoleteMethod
        private async Task<List<VATRetunDetailModel>> GetVATReturnDetail(IFormFile fileData)
        {

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = fileData.OpenReadStream();
            List<VATRetunDetailModel> models = new List<VATRetunDetailModel>();

            try
            {
                if (fileData != null)
                {
                    if (fileData.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (fileData.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        message = "The file format is not supported.";

                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable inputVatInvoiceDetail = dsexcelRecords.Tables[0];
                        var newDT = DataTableHelper.MakeFirstRowAsColumnName(inputVatInvoiceDetail);
                        var detailModels = ConvertDataTableToList.ConvertDataTable<VATRetunDetailModel>(newDT);
                        models = detailModels.Item2;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }

            return await Task.Run(() => models);
        }

        private async Task<(string, List<VATReturnModel>)> DTOConvertModelTODBObject(List<VATRetunDetailModel> models)
        {
            var dbModels = new List<VATReturnModel>();
            int count = 0;
            string vatType = "VAT on Sales";
            string vatrTypeDetail = string.Empty;
            //try
            //{
            //    foreach (var data in models)
            //    {
            //        count++;
            //        if (count > 5)
            //        {
            //            if (data.Column2.Contains("Purchases"))
            //            {
            //                vatType = "VAT on Purchases";
            //            }
            //            var dbModel = new VATReturnModel();
            //            dbModel.VATType = vatType;
            //            dbModel.VATTypeId = data.Column4..GetDefaultIfStringNull<decimal>();
            //            dbModel.VATTypeName = data.Column6;
            //            dbModel.SARAmount = data.Column8.GetDefaultIfStringNull<decimal>();
            //            dbModel.SARAdjustment = data.Column10.GetDefaultIfStringNull<decimal>();
            //            dbModel.SARVATAmount = data.Column12.GetDefaultIfStringNull<decimal>();
            //            dbModels.Add(dbModel);
            //        }
            //        if (count == 2)
            //        {
            //            vatrTypeDetail = data.Column8;
            //        }
            //    }

            //    dbModels.ForEach(data =>
            //    {
            //        data.VATReturnDetail = vatrTypeDetail;
            //        data.CreatedDate = DateTime.Now;
            //    });
            //}
            //catch (Exception ex)
            //{
            //    string message = ex.Message;
            //    return (message, null);
            //}
            return await Task.Run(() => ("", dbModels));
        }

        #endregion

        private async Task<bool> CreateVATReturnData(List<VATReturnModel> models, string userName)
        {
            try
            {
                models.ForEach(data =>
                {
                    data.CreatedBy = userName;
                });

                models.RemoveAll(data => string.IsNullOrEmpty(data.VATTypeName));

                var response = await _IVATReturnModelRepository.CreateEntity(models.ToArray());
                return response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        private async Task UploadSubsidryAttachment(InvoiceDetail model, List<string> inputAttachmentList, List<string> outPutAttachmentList, List<string> returnAttachmentList, List<string> trialAttachmentList, int uploadInvoiceId)
        {
            try
            {
                var counts = new List<int>() { inputAttachmentList.Count(), outPutAttachmentList.Count(), returnAttachmentList.Count(), trialAttachmentList.Count() };
                int maxCount = counts.Max();
                var models = new List<SubsidryInvoiceAttachment>();

                for (int i = 0; i < maxCount; i++)
                {
                    var subsidryModel = new SubsidryInvoiceAttachment()
                    {
                        UploadInvoiceId = uploadInvoiceId,
                        InputAttachmentDetails = inputAttachmentList.ElementAtOrDefault(i) ?? string.Empty,
                        OutPutAttachmentDetails = outPutAttachmentList.ElementAtOrDefault(i) ?? string.Empty,
                        TrialAttachmentDetails = trialAttachmentList.ElementAtOrDefault(i) ?? string.Empty,
                        ReturnAttachmentDetails = returnAttachmentList.ElementAtOrDefault(i) ?? string.Empty,
                        CreatedBy = model.UserName
                    };

                    models.Add(subsidryModel);
                }

                var uploadAttachmentResponse = await _ISubsidryInvoiceAttachmentRepository.CreateEntity(models.ToArray());
            }
            catch (Exception ex) {
                throw new Exception(ex.Message, ex);
            }

        }
        #endregion
    }
}
