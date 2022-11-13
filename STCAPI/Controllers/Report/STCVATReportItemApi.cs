using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Controllers.IGATE;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Report;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.Report
{
    /// <summary>
    ///  STC VAT Report details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class STCVATReportItemApi : ControllerBase
    {
        private readonly IGenericRepository<STCVATReportItem, int> _ISTCVATReportItemRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        ///  Inject required service to the constructors
        /// </summary>
        /// <param name="iSTCVATReportItemRepository"></param>
        /// <param name="iErrorLogRepository"></param>
        public STCVATReportItemApi(IGenericRepository<STCVATReportItem, int> iSTCVATReportItemRepository, IGenericRepository<ErrorLogModel, int> iErrorLogRepository)
        {
            _ISTCVATReportItemRepository = iSTCVATReportItemRepository;
            _IErrorLogRepository = iErrorLogRepository;
        }

        /// <summary>
        /// Create VAT Reports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateVATReport(STCVATReportItem model)
        {
            try
            {
                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;

                var response = await _ISTCVATReportItemRepository.CreateEntity(new List<STCVATReportItem>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATReportItemApi),
                          nameof(CreateVATReport), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }
        }

        /// <summary>
        ///  Get VAT Reports Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetVATReports()
        {
            try
            {
                var response = await _ISTCVATReportItemRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATReportItemApi),
                          nameof(GetVATReports), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }
        }

        /// <summary>
        ///  Get VAT Report details By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetVATReportDetailById(int id)
        {
            try
            {
                var response = await _ISTCVATReportItemRepository.GetAllEntities(c => c.Id == id);
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATReportItemApi),
                          nameof(GetVATReportDetailById), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }

        /// <summary>
        ///  Update VAT Report Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateVATReport(STCVATReportItem model)
        {
            try
            {
                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                

                var response = await _ISTCVATReportItemRepository.UpdateEntity(model);
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATReportItemApi),
                          nameof(UpdateVATReport), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }
        }


        /// <summary>
        ///  Deactivate VAT Report Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteVATReportDetailById(int id)
        {
            try
            {
                var response = await _ISTCVATReportItemRepository.GetAllEntities(c => c.Id == id);
                response.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                    data.UpdatedDate = DateTime.Now;
                });
                var deleteResponse = await _ISTCVATReportItemRepository.DeleteEntity(response.TEntities.ToArray());

                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATReportItemApi),
                          nameof(GetVATReportDetailById), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong.");
            }

        }
    }
}
