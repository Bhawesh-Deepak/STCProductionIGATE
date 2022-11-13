using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Master;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// Periods Api's Details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PeriodsAPI : ControllerBase
    {
        private readonly IGenericRepository<PeriodMaster, int> _IPeriodMasterRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        public PeriodsAPI(IGenericRepository<PeriodMaster, int> iPeriodMasterRepository, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IPeriodMasterRepository = iPeriodMasterRepository;
            _IErrorLogRepository = errorLogRepository;
        }



        /// <summary>
        /// Period Details Api's
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> GetPeriodsDetails(int year)
        {
            var response = await _IPeriodMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted && x.Year == year);
            return Ok(response);
        }

        /// <summary>
        ///  Create custom period for subsidries
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePeriod(int year)
        {
            try
            {
                var periodModels = new List<PeriodMaster>();

                var responseModels = await _IPeriodMasterRepository.GetAllEntities(x => x.Year == year);

                if (responseModels.TEntities.Any())
                {
                    responseModels.TEntities.ToList().ForEach(data => {
                        data.IsActive = false;
                        data.IsDeleted = true;
                    });
                    var deleteResponse = await _IPeriodMasterRepository.DeleteEntity(responseModels.TEntities.ToArray());
                }

                for (int i = 1; i < 13; i++)
                {
                    periodModels.Add(new PeriodMaster()
                    {
                        PeriodDate = new DateTime(year, i, 1),
                        Period = new DateTime(year, i, 1).ToString("MMM") + "-" + year.ToString(),
                        CreatedBy = "admin",
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Year = year,
                    });
                }

                var response = await _IPeriodMasterRepository.CreateEntity(periodModels.ToArray());

                return Ok(response);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(PeriodsAPI),
                        nameof(CreatePeriod), ex.Message, ex.ToString());

                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }
    }
}
