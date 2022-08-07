using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// Source Master Api 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<SourceMaster, int> _ISourceMasterRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="sourceMasterRepo"></param>
        /// <param name="errorLogRepository"></param>
        public SourceMasterAPI(IGenericRepository<SourceMaster, int> sourceMasterRepo, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _ISourceMasterRepository = sourceMasterRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Source Api master
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSourceMaster(SourceMaster model)
        {
            try
            {
                var response = await _ISourceMasterRepository.CreateEntity(new List<SourceMaster>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceMasterAPI),
                        nameof(CreateSourceMaster), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Source Master Detail Api's
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSourceMasterDetails()
        {
            try
            {
                var response = await _ISourceMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceMasterAPI),
                            nameof(GetSourceMasterDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Source Master Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSourceMasterDetails(SourceMaster model)
        {
            try
            {
                var deleteModel = await _ISourceMasterRepository.GetAllEntities(x => x.Id == model.Id);
                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });
                var deleteResponse = await _ISourceMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _ISourceMasterRepository.CreateEntity(new List<SourceMaster>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceMasterAPI),
                        nameof(UpdateSourceMasterDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete Source Master Details Api's
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteSourceMaster(int id)
        {
            try
            {
                var deleteModel = await _ISourceMasterRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });
                var deleteResponse = await _ISourceMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceMasterAPI),
                        nameof(DeleteSourceMaster), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
