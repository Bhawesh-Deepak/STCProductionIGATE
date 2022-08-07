using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.AdminPortal
{
    /// <summary>
    /// Stage Master Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StageMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<StageMaster, int> _IStageMasterRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="stageMasterRepo"></param>
        /// <param name="errorLogRepository"></param>
        public StageMasterAPI(IGenericRepository<StageMaster, int> stageMasterRepo, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IStageMasterRepository = stageMasterRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Stage Master Api's
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStage(StageMaster model)
        {
            try
            {
                var response = await _IStageMasterRepository.CreateEntity(new List<StageMaster>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StageMasterAPI),
                        nameof(CreateStage), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Stage Master details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetStageDetails()
        {
            try
            {
                var response = await _IStageMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StageMasterAPI),
                    nameof(GetStageDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Master Stage details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateStageDetails(StageMaster model)
        {
            try
            {
                var deleteModel = await _IStageMasterRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });

                var deleteResponse = await _IStageMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());

                model.Id = 0;

                var createResponse = await _IStageMasterRepository.CreateEntity(new List<StageMaster>() { model }.ToArray());

                return Ok(createResponse);

            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StageMasterAPI),
                     nameof(UpdateStageDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete Stage Master 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteStage(int id)
        {
            try
            {
                var deleteModel = await _IStageMasterRepository.GetAllEntities(x => x.Id == id);

                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _IStageMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());

                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(StageMasterAPI),
                         nameof(DeleteStage), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
