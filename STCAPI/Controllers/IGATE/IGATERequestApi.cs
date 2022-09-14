using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.IGATE;
using STCAPI.Core.Entities.Logger;
using STCAPI.ErrorLogService;
using STCAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace STCAPI.Controllers.IGATE
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IGATERequestApi : ControllerBase
    {
        private readonly IGenericRepository<IGATERequestDetails, int> _IGateRequestRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to constructor class.
        /// </summary>
        /// <param name="iGateRequestRepository"></param>
        /// <param name="iErrorLogRepository"></param>
        public IGATERequestApi(IGenericRepository<IGATERequestDetails, int> iGateRequestRepository,
            IGenericRepository<ErrorLogModel, int> iErrorLogRepository)
        {
            _IGateRequestRepository = iGateRequestRepository;
            _IErrorLogRepository = iErrorLogRepository;
        }

        /// <summary>
        /// Send the Request Detail with formID
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> PostIGATERequestDetail(RequestModel model, [FromHeader] string FormId)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(model);

                var dbModel = new IGATERequestDetails()
                {
                    CreatedBy = "Admin",
                    RequestText = jsonData,
                    FormId = FormId,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };

                var deleteModels = await _IGateRequestRepository.GetAllEntities(x => x.FormId == FormId);
                deleteModels.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;

                });

                var deleteResponse = await _IGateRequestRepository.DeleteEntity(deleteModels.TEntities.ToArray());


                var response = await _IGateRequestRepository.CreateEntity(new List<IGATERequestDetails>() { dbModel }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATERequestApi),
                           nameof(PostIGATERequestDetail), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FormId"></param>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
       
        public async Task<IActionResult> GetIGATERequestDetails([FromHeader] string FormId)
        {
            try
            {

                var response = await _IGateRequestRepository.GetAllEntities(x => x.FormId == FormId && x.IsActive && !x.IsDeleted);
                var model = new RequestModel();
                if (response != null && response.TEntities.Any())
                {
                    model = JsonConvert.DeserializeObject<RequestModel>(response.TEntities.First().RequestText);

                }
                return Ok(new ResponseModel<Detail, int>()
                {
                    TEntities = model.bpmRequest.request.details,
                    Entity = null,
                    Message = "Sucess",
                    ResponseStatus = ResponseStatus.Success
                });
            }
            catch (Exception ex) {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATERequestApi),
                 nameof(PostIGATERequestDetail), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }
    }
}
