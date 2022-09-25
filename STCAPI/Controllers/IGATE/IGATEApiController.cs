using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.IGATE;
using STCAPI.Core.Entities.Logger;
using STCAPI.ErrorLogService;
using STCAPI.Model;
using System;
using System.Collections.Generic;
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


        /// <summary>
        /// Inject required service to controller contructor
        /// </summary>
        /// <param name="vatRequestUpdateRepo"></param>
        public IGATEApiController(IGenericRepository<VATRequestUpdate, int> vatRequestUpdateRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository, IConfiguration configuration, IGenericRepository<IGATERequestDetails, int> iGateRequestRepository)
        {
            _IVATRequestUpdateRepo = vatRequestUpdateRepo;
            _IErrorLogRepository = errorLogRepository;
            _IConfiguration = configuration;
            _IGateRequestRepository = iGateRequestRepository;
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
                    model.SentDate= data.CreatedDate;
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
                client_id= _IConfiguration.GetSection("IGATE:client_id").Value,
                client_secret= _IConfiguration.GetSection("IGATE:client_sceretKey").Value,
                grant_type= _IConfiguration.GetSection("IGATE:grant_type").Value
            };
        }

        #endregion
    }
}
