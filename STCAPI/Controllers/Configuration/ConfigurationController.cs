using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Configuration;
using STCAPI.Core.Entities.LogDetail;
using STCAPI.Core.Entities.Logger;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.Configuration
{
    /// <summary>
    ///  Configuration manager which manage the stage stream and main stream master
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IGenericRepository<StageMaster, int> _IStageMasterRepository;
        private readonly IGenericRepository<StreamMaster, int> _IStreamMasterRepository;
        private readonly IGenericRepository<MainStreamMaster, int> _IMainStreamRepository;
        private readonly IGenericRepository<LogDetail, int> _ILogeDetailRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        private readonly IGenericRepository<ConfigurationMaster, int> _IConfigurationMaster;

        /// <summary>
        /// Inject required service to the constructor
        /// </summary>
        /// <param name="iStageMasterRepository"></param>
        /// <param name="iStreamMasterRepository"></param>
        /// <param name="iMainStreamRepository"></param>
        /// <param name="logDetailRepository"></param>
        /// <param name="iConfigurationMaster"></param>
        public ConfigurationController(IGenericRepository<StageMaster, int> iStageMasterRepository,
            IGenericRepository<StreamMaster, int> iStreamMasterRepository,
            IGenericRepository<MainStreamMaster, int> iMainStreamRepository,
            IGenericRepository<LogDetail, int> logDetailRepository,
            IGenericRepository<ConfigurationMaster, int> iConfigurationMaster,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IStageMasterRepository = iStageMasterRepository;
            _IStreamMasterRepository = iStreamMasterRepository;
            _IMainStreamRepository = iMainStreamRepository;
            _IConfigurationMaster = iConfigurationMaster;
            _ILogeDetailRepository = logDetailRepository;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Get Statge Details
        /// 
        /// </summary>
        /// <remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetStageDetail()
        {
            try
            {
                var response = await _IStageMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                    nameof(GetStageDetail), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Get Main Stream Details
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
        public async Task<IActionResult> GetMainStreamDetail(int stageId)
        {
            try
            {
                var response = await _IMainStreamRepository.GetAllEntities(x => x.StageId == stageId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(GetMainStreamDetail), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get stream detail data
        /// </summary>
        /// <param name="mainStreamId"></param>
        /// 
        /// <remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetStreamDetail(int mainStreamId)
        {
            try
            {
                var response = await _IStreamMasterRepository.GetAllEntities(x => x.MainStreamId == mainStreamId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(GetStreamDetail), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Create configuration 
        /// </summary>
        /// <param name="model"></param>
        /// 
        ///<remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateConfiguration(ConfigurationMaster model)
        {
            try
            {
                var response = await _IConfigurationMaster.CreateEntity(new List<ConfigurationMaster>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(CreateConfiguration), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Configuration Detail data
        /// </summary>
        /// <param name="configurationTypeId"></param>
        /// 
        /// <remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetConfigurationDetails(string configurationTypeId)
        {
            try
            {
                var response = await _IConfigurationMaster.GetAllEntities(x => x.ConfigurationType.Trim().ToUpper()
                == configurationTypeId.Trim().ToUpper());
                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(GetConfigurationDetails), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Configuration detail information 
        /// </summary>
        /// <param name="model"></param>
        /// 
        ///<remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateConfiguration(ConfigurationMaster model)
        {
            try
            {
                var deleteModel = await _IConfigurationMaster.GetAllEntities(x => x.Id == model.Id);
                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsDeleted = true;
                    x.IsActive = false;
                });
                var deleteResponse = await _IConfigurationMaster.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _IConfigurationMaster.CreateEntity(new List<ConfigurationMaster>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(UpdateConfiguration), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete  configuration detail information
        /// </summary>
        /// <param name="id"></param>
        /// 
        ///<remarks> Using API to get the complete stage details
        /// 
        /// AllowAnnonymous -> Authentication and Authorization not required.
        /// 
        /// 200: on success exceution for API EndPoint will get the data with 200 status code
        /// 
        /// On Exception We will log the Exception So that we get the complete log Information
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteConfiguration(int id)
        {
            try
            {
                var deleteModels = await _IConfigurationMaster.GetAllEntities(x => x.Id == id);
                deleteModels.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });
                var deleteResponse = await _IConfigurationMaster.DeleteEntity(deleteModels.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ConfigurationController),
                nameof(DeleteConfiguration), exceptionMessage, ex.ToString());
                return BadRequest("Issue Occured, Please contact admin Team !");
            }
        }
    }
}
