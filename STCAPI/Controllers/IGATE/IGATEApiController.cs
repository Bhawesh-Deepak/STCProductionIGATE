using CommonHelper;
using Magnum.FileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.IGATE;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.ErrorLogService;
using STCAPI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.IGATE
{
    /// <summary>
    /// I GATE Api's for Authentication and request Processing 
    /// </summary>
    /// <remarks>
    ///  I GATE API To initiate the I GATE Approval Cycle
    /// </remarks>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IGATEApiController : ControllerBase
    {
        private readonly IGenericRepository<VATRequestUpdate, int> _IVATRequestUpdateRepo;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        private readonly IConfiguration _IConfiguration;
        private readonly IGenericRepository<IGATERequestDetails, int> _IGateRequestRepository;
        private readonly IGenericRepository<IGATEUploadDocument, int> _IGateUploadDocumentRepository;
        private readonly IHostingEnvironment _IHostingEnviroment;


        /// <summary>
        /// Inject required service to controller contructor
        /// </summary>
        /// <param name="vatRequestUpdateRepo"></param>
        public IGATEApiController(IGenericRepository<VATRequestUpdate, int> vatRequestUpdateRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository,
            IConfiguration configuration, IGenericRepository<IGATERequestDetails, int> iGateRequestRepository,
            IGenericRepository<IGATEUploadDocument, int> iGateUploadDocumentRepository, IHostingEnvironment hostingEnvironment)
        {
            _IVATRequestUpdateRepo = vatRequestUpdateRepo;
            _IErrorLogRepository = errorLogRepository;
            _IConfiguration = configuration;
            _IGateRequestRepository = iGateRequestRepository;
            _IGateUploadDocumentRepository = iGateUploadDocumentRepository;
            _IHostingEnviroment = hostingEnvironment;
        }
        /// <summary>
        /// Authenticate Response From IGATE API
        /// </summary>
        /// 
        ///<remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code With Token and Expiry Time
        /// We need to send this Token to further request for I GATE Approval Cycle, For Each Request we send the VAT Request Detail with Auth Token
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// </remarks>
        /// <returns>
        ///    200  With Authentication Token
        /// </returns>


        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetResponseTokenOnAuthenticate()
        {
            try
            {
                using HttpClient client = new HttpClient { BaseAddress = new Uri(_IConfiguration.GetSection("IGATE:baseUrl").Value) };
                var stringContent = new StringContent(JsonConvert.SerializeObject(GetAuthModel()), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("stcOeOAuthConnection/1.0/accessToken", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseDetails = await response.Content.ReadAsStringAsync();
                    return Ok(responseDetails);
                }
                else
                {
                    return BadRequest("Something wents wrong.");
                }
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
                           nameof(GetResponseTokenOnAuthenticate), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }
        }


        /// <summary>
        /// Initiate the VAT Approval Cycle with VAT Details and Authentication Token
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// 
        ///<remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization  required.
        /// 
        /// We need to send the VAT Request detail with Authentication Token which comes from  Header
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Successful execution of VAT Request Details</response>
        /// <response code="400">If Some Exception occured in VAT Request Detail from IGATE</response>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> VATRequestDetails(RequestModel model, [FromHeader] string token)
        {
            try
            {
                using HttpClient client = new HttpClient { BaseAddress = new Uri(_IConfiguration.GetSection("IGATE:baseUrl").Value) };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("stcOeEsbVATServices/1.0/vat/en/initiate", stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseDetails = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<BPMResponseModel>(responseDetails);

                    return Ok(responseData);
                }
                else
                {
                    return BadRequest("Something wents wrong.");
                }
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
                          nameof(VATRequestDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }
        }
        /// <summary>
        /// Upload attachment and get the response with MIME Type file name and file content details
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> UploadAttachment([FromForm] IGATEAttachmentModel attachment, [FromQuery] string token)
        {

            try
            {
                double imageSize = Double.Parse("0");
                string responseDetails = string.Empty;
                imageSize = calcBase64SizeInKBytes(ImageToByteArray(attachment.Attachment).ToString());

                var model = new Attachment()
                {
                    mimeType = attachment.Attachment.ContentType,
                    fileName = attachment.Attachment.FileName,
                    fileContents = ImageToByteArray(attachment.Attachment).ToString()
                };

                // request.AddHeader("Authorization", "Bearer b1cf8950e24b44fa9de0ee95e3006a89");

                using HttpClient client = new HttpClient { BaseAddress = new Uri("https://10.21.13.29:9016/") };
                //client.DefaultRequestHeaders.Authorization=
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = client.PostAsync("esb/dms/1.0/en/upload", stringContent).Result;

                var documentPath = await new BlobHelper().UploadDocument(attachment.Attachment, _IHostingEnviroment);

                if (response.IsSuccessStatusCode)
                {
                    responseDetails = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<IGATEDocumentResponseVm>(responseDetails);

                    var documentResponse = new AttachmentModel()
                    {
                        mimeType = model.mimeType,
                        fileName = model.fileName,
                        attachmentId = responseData.response.documentID
                    };

                    var dbResponse = await _IGateUploadDocumentRepository.CreateEntity(new List<IGATEUploadDocument>() {
                             new IGATEUploadDocument(){
                                IsDeleted = true,
                                IsActive = true,
                                DocumentId= documentResponse.attachmentId,
                                CreatedBy="Admin",
                                CreatedDate=DateTime.Now,
                                DocumentPath=documentPath
                             }
                }.ToArray());

                    return Ok(documentResponse);
                }
                else
                {
                    await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
                             nameof(UploadAttachment), response.StatusCode.ToString(), response.Content.ToString());

                    return BadRequest("Something wents wrong.");
                }
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
              nameof(UploadAttachment), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }


        /// <summary>
        /// Update the VAT Request and initiate the Approval Cycle.
        /// Every Time you call the Api with Form ID and approval detail to update the Approval request.
        /// 
        /// </summary>
        /// <remarks> Using API to get the complete Complete Main Stream Details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> UpdateVATAPI(UpdateFormModel model)
        {
            try
            {
                var dbModel = new VATRequestUpdate();
                dbModel.FormId = model.FormId;
                dbModel.ApproverEmail = model.ApproverEmail;
                dbModel.PendingwithEmail = model.PendingwithEmail;
                dbModel.Comments = model.Comments;
                dbModel.Decision = model.Decision;
                dbModel.RequestStatus = model.RequestStatus;
                dbModel.IsActive = true;
                dbModel.IsDeleted = false;
                dbModel.CreatedBy = "Admin";
                dbModel.CreatedDate = DateTime.Now;
                dbModel.UpdatedDate = DateTime.Now;
                dbModel.UpdatedBy = "Admin";
                dbModel.DocumentId = model.DocumentId;


                var response = await _IVATRequestUpdateRepo.CreateEntity(new List<VATRequestUpdate>() { dbModel }.ToArray());
                return Ok(new { message = "success" });
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
                       nameof(UpdateVATAPI), ex.Message, ex.ToString());

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Get VAT Request Details to Display the Dashboard.
        /// 
        /// </summary>
        /// <remarks> Using API to get the complete Complete Main Stream Details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetVATRequest()
        {
            try
            {
                var response = await _IVATRequestUpdateRepo.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var models = new List<UpdateFormModel>();

                response.TEntities.ToList().ForEach(data =>
                {
                    var model = new UpdateFormModel();
                    model.FormId = data.FormId;
                    model.Decision = data.Decision;
                    model.Comments = data.Comments;
                    model.ApproverEmail = data.ApproverEmail;
                    model.PendingwithEmail = data.PendingwithEmail;
                    model.RequestStatus = data.RequestStatus;
                    model.SentDate = data.CreatedDate;
                    models.Add(model);
                });
                return Ok(models);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATEApiController),
                    nameof(UpdateVATAPI), ex.Message, ex.ToString());

                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DownloadIGATEDocument(string formId)
        {
            var iGateResponse = await _IVATRequestUpdateRepo.GetAllEntities(x => x.FormId.Trim().ToLower() == formId.Trim().ToLower());
            if (iGateResponse.TEntities.Any())
            {
                var documentResponse = await _IGateUploadDocumentRepository.GetAllEntities(x => x.DocumentId.Trim().ToLower() == iGateResponse.TEntities.First().DocumentId.Trim().ToLower());
                if (documentResponse.TEntities.Any())
                {
                    return Ok(documentResponse.TEntities.First().DocumentPath);
                }
            }
            return BadRequest();

        }

        #region PrivateFields

        /// <summary>
        /// client_id and client_scerete and grant_type following are the value which are valid for IGATE API 
        /// </summary>
        /// <returns></returns>
        private AuthenticationModel GetAuthModel()
        {
            return new AuthenticationModel()
            {
                //client_id = "3a1bc4a8-e01a-4b57-bb2b-3ead05057c2e",
                //client_secret = "fbe24031-ecff-411b-b123-b37148de5543",
                //grant_type = "client_credentials"
                client_id = _IConfiguration.GetSection("IGATE:client_id").Value,
                client_secret = _IConfiguration.GetSection("IGATE:client_sceretKey").Value,
                grant_type = _IConfiguration.GetSection("IGATE:grant_type").Value
            };
        }


        private string ImageToByteArray(IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileArray = ms.ToArray();
                    return Convert.ToBase64String(fileArray);
                }
            }

            return null;


        }

        private Double calcBase64SizeInKBytes(String base64String)
        {
            Double result = -1.0;
            if (!string.IsNullOrEmpty(base64String))
            {
                int padding = 0;
                if (base64String.EndsWith("=="))
                {
                    padding = 2;
                }
                else
                {
                    if (base64String.EndsWith("=")) padding = 1;
                }
                result = Convert.ToDouble((Math.Ceiling(Convert.ToDecimal(base64String.Length / 4) * 3)) - padding);
            }
            return result / 1000;
        }
        #endregion
    }
}
