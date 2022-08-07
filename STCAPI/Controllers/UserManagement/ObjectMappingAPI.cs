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
    /// ObjectMappingApi
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ObjectMappingAPI : ControllerBase
    {
        private readonly IGenericRepository<ObjectMapping, int> _IObjectMappingRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        public ObjectMappingAPI(IGenericRepository<ObjectMapping, int> ObjectMappingRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IObjectMappingRepository = ObjectMappingRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// create object mapping 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateObjectMapping(ObjectMapping model)
        {
            try
            {
                var response = await _IObjectMappingRepository.CreateEntity(new List<ObjectMapping>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMappingAPI),
                       nameof(CreateObjectMapping), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Get Object mapping details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetObjectMappingDetails()
        {
            try
            {
                var response = await _IObjectMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMappingAPI),
                            nameof(GetObjectMappingDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Update Object Mapping Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateObjectMappingDetails(ObjectMapping model)
        {
            try
            {
                var deleteModel = await _IObjectMappingRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });

                var deleteResponse = await _IObjectMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _IObjectMappingRepository.CreateEntity(new List<ObjectMapping>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMappingAPI),
                            nameof(UpdateObjectMappingDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Delete Object Mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteObjectMapping(int id)
        {
            try
            {
                var deleteModel = await _IObjectMappingRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });
                var deleteResponse = await _IObjectMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ObjectMappingAPI),
                        nameof(DeleteObjectMapping), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
