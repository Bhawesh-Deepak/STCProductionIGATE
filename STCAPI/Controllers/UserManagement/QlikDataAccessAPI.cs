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
    /// Qlik Data Access Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QlikDataAccessAPI : ControllerBase
    {
        private readonly IGenericRepository<QlikDataAccess, int> _IQlickDataAccessRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="qlickDataAccessRepo"></param>
        /// <param name="errorLogRepository"></param>
        public QlikDataAccessAPI(IGenericRepository<QlikDataAccess, int> qlickDataAccessRepo, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IQlickDataAccessRepository = qlickDataAccessRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create DataAccess for Qlik
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDataAccess(QlikDataAccess model)
        {
            try
            {
                var filteredModel = await _IQlickDataAccessRepository.GetAllEntities
                    (
                         x => x.StreamName.Trim().ToUpper() == model.StreamName.Trim().ToUpper()
                         && x.UserName.Trim().ToUpper() == model.UserName.Trim().ToUpper()
                         && x.AppName.Trim().ToUpper() == model.AppName.Trim().ToUpper()
                         && x.AccessLevel.Trim().ToUpper() == model.AccessLevel.Trim().ToUpper()
                         && x.DataGranularity.Trim().ToUpper() == model.DataGranularity.Trim().ToUpper()
                         && x.ActionName.Trim().ToUpper() == model.ActionName.Trim().ToUpper()
                    );

                if (filteredModel.TEntities.Any())
                {
                    filteredModel.TEntities.ToList().ForEach(data =>
                    {
                        data.IsActive = false;
                        data.IsDeleted = true;
                    });

                    var deleteResponse = await _IQlickDataAccessRepository.DeleteEntity(filteredModel.TEntities.ToArray());
                }

                var response = await _IQlickDataAccessRepository.CreateEntity(new List<QlikDataAccess>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(QlikDataAccessAPI),
                     nameof(CreateDataAccess), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");

            }

        }
        /// <summary>
        /// Get Access
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetAccess()
        {
            try
            {
                var response = await _IQlickDataAccessRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(QlikDataAccessAPI),
                            nameof(GetAccess), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
