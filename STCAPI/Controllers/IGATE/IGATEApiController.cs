using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.IGATE;
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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IGATEApiController : ControllerBase
    {
        private readonly IGenericRepository<VATRequestUpdate, int> _IVATRequestUpdateRepo;

        /// <summary>
        /// Inject required service to controller contructor
        /// </summary>
        /// <param name="vatRequestUpdateRepo"></param>
        public IGATEApiController(IGenericRepository<VATRequestUpdate, int> vatRequestUpdateRepo)
        {
            _IVATRequestUpdateRepo = vatRequestUpdateRepo;
        }
        /// <summary>
        /// Authenticate Response From IGATE API
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetResponseTokenOnAuthenticate()
        {
            using HttpClient client = new HttpClient { BaseAddress = new Uri("https://10.21.132.47:9016/gateway/") };
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> VATRequestDetails(RequestModel model, [FromHeader] string token)
        {
            using HttpClient client = new HttpClient { BaseAddress = new Uri("https://10.21.132.47:9016/gateway/") };
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
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
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetVATRequest()
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

                models.Add(model);
            });
            return Ok(models);
        }

        #region PrivateFields

        private AuthenticationModel GetAuthModel()
        {
            return new AuthenticationModel()
            {
                client_id = "3a1bc4a8-e01a-4b57-bb2b-3ead05057c2e",
                client_secret = "fbe24031-ecff-411b-b123-b37148de5543",
                grant_type = "client_credentials"
            };
        }

        #endregion
    }
}
