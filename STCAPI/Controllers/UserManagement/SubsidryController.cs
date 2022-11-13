using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Subsidry;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// SubsidryApi 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubsidryController : ControllerBase
    {
        private readonly IGenericRepository<SubsidryModel, int> _ISubsidryModelRepository;
        private readonly IGenericRepository<SubsidryUserMapping, int> _ISubsidryUserMapping;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        public SubsidryController(IGenericRepository<SubsidryModel, int> subsidryModelRepository,
            IGenericRepository<SubsidryUserMapping, int> subsidryUserMappingRepository, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _ISubsidryModelRepository = subsidryModelRepository;
            _ISubsidryUserMapping = subsidryUserMappingRepository;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Subsusidry
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSubsidry(SubsidryModel model)
        {
            try
            {
                var response = await _ISubsidryModelRepository.CreateEntity(new List<SubsidryModel>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                     nameof(CreateSubsidry), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update SubsidryMapping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSubsidry(SubsidryModel model)
        {
            try
            {
                var deleteModel = await _ISubsidryModelRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _ISubsidryModelRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var response = await _ISubsidryModelRepository.CreateEntity(new List<SubsidryModel>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                     nameof(UpdateSubsidry), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }


        /// <summary>
        /// Get Subsidry Mapping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSubsidry()
        {
            try
            {
                var response = await _ISubsidryModelRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                 nameof(GetSubsidry), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
        /// <summary>
        /// Delete subsidry mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteSubsidry(int id)
        {
            try
            {
                var deleteModel = await _ISubsidryModelRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _ISubsidryModelRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                    nameof(DeleteSubsidry), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Create subsidry user Mapping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSubsidryUserMapping(SubsidryUserMapping model)
        {
            try
            {
                //var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.UserName == model.UserName);

                //deleteModel.TEntities.ToList().ForEach(data =>
                //{
                //    data.IsActive = false;
                //    data.IsDeleted = true;
                //});

                //var deleteReponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());

                model.Id = 0;
                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedBy = "admin";
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;

                var createRepsonse = await _ISubsidryUserMapping.CreateEntity(new List<SubsidryUserMapping>() { model }.ToArray());

                return Ok(createRepsonse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                    nameof(CreateSubsidryUserMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete Subsidry Mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteSubsidryMapping(int id)
        {
            try
            {
                var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.Id == id);

                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                    nameof(DeleteSubsidryMapping), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get SubsidryMapping Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSubsidryMappingDetails()
        {
            try
            {
                var response = await _ISubsidryUserMapping.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                        nameof(GetSubsidryMappingDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update SubsidryMapping Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSubsidryMapping(SubsidryUserMapping model)
        {
            try
            {
                var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var response = await _ISubsidryUserMapping.CreateEntity(new List<SubsidryUserMapping>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SubsidryController),
                        nameof(UpdateSubsidryMapping), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        ///  Get Company Details by Company User Mappings
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetCompanyDetailByUserName(string userId)
        {

            var response = await _ISubsidryUserMapping.GetAllEntities(x => x.UserId.Trim().ToLower() 
                        == userId.Trim().ToLower()
                 && x.IsActive && !x.IsDeleted
            );


            return Ok(response);
        }
    }
}
