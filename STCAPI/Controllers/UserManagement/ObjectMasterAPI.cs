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
    /// Object Master
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ObjectMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<ObjectMaster, int> _IObjectMasterRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// inject required service to controller constructor
        /// </summary>
        /// <param name="ObjectMasterRepo"></param>
        /// <param name="errorLogRepository"></param>
        public ObjectMasterAPI(IGenericRepository<ObjectMaster, int> ObjectMasterRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IObjectMasterRepository = ObjectMasterRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// CreateObjectMaster
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateObjectMaster(ObjectMaster model)
        {
            try
            {
                var response = await _IObjectMasterRepository.CreateEntity(new List<ObjectMaster>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMasterAPI),
                       nameof(CreateObjectMaster), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// GetObjectMasterDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetObjectMasterDetails()
        {
            try
            {
                var response = await _IObjectMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMasterAPI),
                            nameof(GetObjectMasterDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// UpdateObjectMasterDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateObjectMasterDetails(ObjectMaster model)
        {
            try
            {
                var deleteModel = await _IObjectMasterRepository.GetAllEntities(x => x.Id == model.Id);
                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });
                var deleteResponse = await _IObjectMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _IObjectMasterRepository.CreateEntity(new List<ObjectMaster>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMasterAPI),
                    nameof(UpdateObjectMasterDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// DeleteObjectMaster
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteObjectMaster(int id)
        {
            try
            {
                var deleteModel = await _IObjectMasterRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });
                var deleteResponse = await _IObjectMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMasterAPI),
                        nameof(DeleteObjectMaster), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }
    }
}
