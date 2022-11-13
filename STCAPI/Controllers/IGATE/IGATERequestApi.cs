using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.IGATE;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.ViewModel.ResponseModel;
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
        private readonly IGenericRepository<VATRequestUpdate, int> _IVATRequestUpdateRepo;

        /// <summary>
        /// Inject required service to constructor class.
        /// </summary>
        /// <param name="iGateRequestRepository"></param>
        /// <param name="iErrorLogRepository"></param>
        public IGATERequestApi(IGenericRepository<IGATERequestDetails, int> iGateRequestRepository,
            IGenericRepository<ErrorLogModel, int> iErrorLogRepository,
            IGenericRepository<VATRequestUpdate, int> iVATRequestUpdateRepo)
        {
            _IGateRequestRepository = iGateRequestRepository;
            _IErrorLogRepository = iErrorLogRepository;
            _IVATRequestUpdateRepo = iVATRequestUpdateRepo;
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
        ///  Get I GATE Request Details
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
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATERequestApi),
                 nameof(PostIGATERequestDetail), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }
        /// <summary>
        ///  Get I GATE Request Response details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetIGATERequestResponse()
        {
            try
            {
                var responseModels = new List<IGATEResponseModel>();
                var response = (await _IGateRequestRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted));
                var model = new RequestModel();

                var requestModels = await _IVATRequestUpdateRepo.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var filteredRequestDetails = requestModels.TEntities.OrderByDescending(x => x.CreatedDate);

                if (response != null && response.TEntities.Any())
                {
                    foreach (var data in response.TEntities)
                    {
                        var responseModel = new IGATEResponseModel();
                        responseModel.FormId = data.FormId;
                        model = JsonConvert.DeserializeObject<RequestModel>(data.RequestText);
                        responseModel.Month = model.bpmRequest.request.details[0].value;
                        responseModel.Year = model.bpmRequest.request.details[1].value;
                        responseModel.VATOnSale = model.bpmRequest.request.details[2].value;
                        responseModel.VATOnPurchase = model.bpmRequest.request.details[3].value;
                        responseModel.VATReturnDetails = model.bpmRequest.request.details[4].value;
                        responseModel.Comments = model.bpmRequest.request.details[5].value;
                        responseModel.OtherVAT = model.bpmRequest.request.details[6].value;

                        if (filteredRequestDetails.Any(x => x.FormId == data.FormId))
                        {

                            responseModel.Decision = filteredRequestDetails.First(x => x.FormId == data.FormId).Decision;
                            responseModel.CurrentStatus = filteredRequestDetails.First(x => x.FormId == data.FormId).RequestStatus;
                            responseModel.Email = filteredRequestDetails.First(x => x.FormId == data.FormId).ApproverEmail;
                            responseModel.CreatedDate = filteredRequestDetails.First(x => x.FormId == data.FormId).CreatedDate;

                            responseModels.Add(responseModel);
                        }
                    }
                }

                var uniqueRecords = from p in responseModels
                                    group p by new { p.FormId }
                                     into mygroup
                                    select mygroup.FirstOrDefault();


                return Ok(uniqueRecords);

                //return Ok(new ResponseModel<IGATEResponseModel, int>()
                //{
                //    TEntities = uniqueRecords.ToList(),
                //    Entity = null,
                //    Message = "Sucess",
                //    ResponseStatus = ResponseStatus.Success
                //});
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATERequestApi),
                  nameof(GetIGATERequestResponse), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }

        /// <summary>
        ///  Check previous I GATE request for month and Year
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CheckFormIdExists(string monthName, string yearName)
        {
            try
            {
                //var response = await _IVATRequestUpdateRepo.GetAllEntities(x => x.IsActive && !x.IsDeleted
                //            && x.FormId.Trim().ToLower() == formId.Trim().ToLower());

                //var responseModel = response.TEntities.OrderByDescending(x => x.CreatedDate).FirstOrDefault();


                //return Ok(new ResponseModel<VATRequestUpdate, int>()
                //{
                //    TEntities = null,
                //    Entity = responseModel,
                //    Message = "Sucess",
                //    ResponseStatus = ResponseStatus.Success
                //});

                var response = (await _IGateRequestRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted));
                var model = new RequestModel();
                var responseModels = new List<IGATEResponseModel>();
                if (response != null && response.TEntities.Any())
                {
                    foreach (var data in response.TEntities)
                    {
                        var responseModel = new IGATEResponseModel();
                        responseModel.FormId = data.FormId;
                        model = JsonConvert.DeserializeObject<RequestModel>(data.RequestText);
                        responseModel.Month = model.bpmRequest.request.details[0].value;
                        responseModel.Year = model.bpmRequest.request.details[1].value;
                        responseModel.VATOnSale = model.bpmRequest.request.details[2].value;
                        responseModel.VATOnPurchase = model.bpmRequest.request.details[3].value;
                        responseModel.VATReturnDetails = model.bpmRequest.request.details[4].value;
                        responseModel.Comments = model.bpmRequest.request.details[5].value;
                        responseModel.OtherVAT = model.bpmRequest.request.details[6].value;
                        responseModels.Add(responseModel);
                    }
                }

                var isExists = false;

                responseModels.ForEach(data =>
                {
                    if (data.Month.Trim().ToLower() == monthName.Trim().ToLower() && data.Year.ToLower().Trim() == yearName.Trim().ToLower())
                    {
                        isExists = true;
                    }
                });

                return Ok(isExists);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(IGATERequestApi),
                     nameof(CheckFormIdExists), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }


        }
    }
}
