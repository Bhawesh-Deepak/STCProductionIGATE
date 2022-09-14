using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.ErrorLogService;
using STCAPI.Helpers;
using STCAPI.ReqRespVm.AdminPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.AdminPortal
{
    /// <summary>
    /// Stream Master Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StreamMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<StreamMaster, int> _IStreamMasterRepository;
        private readonly IGenericRepository<MainStreamMaster, int> _IMainStreamRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="streamMasterRepo"></param>
        /// <param name="mainStreamRepo"></param>
        /// <param name="errorLogRepository"></param>
        public StreamMasterAPI(IGenericRepository<StreamMaster, int> streamMasterRepo,
            IGenericRepository<MainStreamMaster, int> mainStreamRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IMainStreamRepository = mainStreamRepo;
            _IStreamMasterRepository = streamMasterRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create StreamMaster Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStream(StreamMaster model)
        {
            try
            {
                var createResponse = await _IStreamMasterRepository.CreateEntity(new List<StreamMaster>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StreamMasterAPI),
                    nameof(CreateStream), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Get Stream Master Details Api
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> GetStreamDetails()
        {
            try
            {
                //var mainStreamData = await _IMainStreamRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var streamData = await _IStreamMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                //List<StreamDetailVm> responseData = CommonServiceHelper.GetStreamDetails(mainStreamData, streamData);

                return Ok(streamData);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StreamMasterAPI),
                     nameof(GetStreamDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }


        }

        /// <summary>
        /// Delete Stream Master Api
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteStream(int id)
        {
            try
            {
                var deleteModel = await _IStreamMasterRepository.GetAllEntities(x => x.Id == id);

                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });

                var response = await _IStreamMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());

                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StreamMasterAPI),
                    nameof(DeleteStream), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Stream master Api
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateStream(StreamMaster model)
        {
            try
            {
                var deleteModel = await _IStreamMasterRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _IStreamMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());

                model.Id = 0;

                var createResponse = await _IStreamMasterRepository.CreateEntity(new List<StreamMaster>() { model }.ToArray());

                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StreamMasterAPI),
                    nameof(UpdateStream), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
