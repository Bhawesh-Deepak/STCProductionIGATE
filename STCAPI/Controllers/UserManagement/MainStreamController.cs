using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.ErrorLogService;
using STCAPI.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// MainStream controller To Perform CRUD Operation Details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MainStreamController : ControllerBase
    {
        private readonly IGenericRepository<MainStreamMaster, int> _IMainStreamRepository;
        private readonly IGenericRepository<StageMaster, int> _IStageMasterRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="mainStreamRepo"></param>
        /// <param name="iStageMasterRepository"></param>
        /// <param name="errorLogRepository"></param>
        public MainStreamController(IGenericRepository<MainStreamMaster, int> mainStreamRepo,
            IGenericRepository<StageMaster, int> iStageMasterRepository,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IMainStreamRepository = mainStreamRepo;
            _IStageMasterRepository = iStageMasterRepository;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create MainStream 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> CreateMainStream(MainStreamMaster model)
        {
            try
            {
                model.IsDeleted = false;
                model.IsActive = true;
                MainStreamMaster[] dbModelArray = { model };
                var response = await _IMainStreamRepository.CreateEntity(dbModelArray);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(MainStreamController),
                        nameof(CreateMainStream), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Main stream details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> GetMainStreamDetails()
        {
            try
            {
                var mainStreamModel = await _IMainStreamRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                //var stageModel = await _IStageMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                //var response = CommonServiceHelper.GetMainStreamDetail(mainStreamModel, stageModel);

                return Ok(mainStreamModel);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(MainStreamController),
                        nameof(GetMainStreamDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Main Stream Details 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> UpdateMainStreamDetails(MainStreamMaster model)
        {
            try
            {
                var deleteModel = await _IMainStreamRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;

                });

                var deleteResponse = await _IMainStreamRepository.UpdateEntity(deleteModel.TEntities.First());

                model.Id = 0;

                MainStreamMaster[] dbModelArray = { model };

                var createResponse = await _IMainStreamRepository.CreateEntity(dbModelArray);

                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(MainStreamController),
                        nameof(UpdateMainStreamDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete Main Stream Details 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteMainStream(int id)
        {
            try
            {
                var response = await _IMainStreamRepository.GetAllEntities(x => x.Id == id);

                if (response.TEntities.Any())
                {
                    response.TEntities.ToList().ForEach(item =>
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                    });

                    var deleteResponse = await _IMainStreamRepository.UpdateEntity(response.TEntities.First());

                    return Ok(deleteResponse);
                }

                return BadRequest($"Invalid Report Id {id}");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(MainStreamController),
                        nameof(DeleteMainStream), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
