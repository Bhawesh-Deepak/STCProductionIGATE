using CommonHelper;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.VATReport;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.VATReport
{
    /// <summary>
    /// VAT Report Mapping Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class VATReportMappingController : ControllerBase
    {
        private readonly IGenericRepository<VATReportMapping, int> _IVATReportMappingRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        private readonly IDapperRepository<object> _IVATReportMasterRepository;


        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="vatReportMappingRepo"></param>
        /// <param name="errorLogRepo"></param>
        /// <param name="dapperRepository"></param>
        public VATReportMappingController(IGenericRepository<VATReportMapping, int> vatReportMappingRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepo, IDapperRepository<object> dapperRepository)
        {
            _IVATReportMappingRepository = vatReportMappingRepo;
            _IErrorLogRepository = errorLogRepo;
            _IVATReportMasterRepository = dapperRepository;
        }


        /// <summary>
        /// GET Complete VAT Report Mapping Details:
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetVATReportMapping()
        {
            try
            {
                var response = await _IVATReportMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                        nameof(GetVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// CreateVATReportMapping Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        public async Task<IActionResult> CreateVATReportMapping(VATReportMapping model)
        {
            try
            {
                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                var response = await _IVATReportMappingRepository.CreateEntity(new List<VATReportMapping>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                     nameof(CreateVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// GETVATReportMapping ByID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetVATReportMappingById(int Id)
        {
            try
            {

                var response = await _IVATReportMappingRepository.GetAllEntities(x => x.Id == Id);
                if (response.TEntities.Any())
                {
                    return Ok(response);
                }
                return BadRequest("Invalid Request Id");

            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                        nameof(GetVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }


        /// <summary>
        /// DeleteVATReportMaping
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteVATReportMapping(int Id)
        {
            try
            {

                var response = await _IVATReportMappingRepository.GetAllEntities(x => x.Id == Id);
                if (response.TEntities.Any())
                {
                    response.TEntities.ToList().ForEach(data =>
                    {
                        data.IsActive = false;
                        data.IsDeleted = true;
                        data.UpdatedBy = data.CreatedBy;
                        data.UpdatedDate = DateTime.Now;
                    });

                    var deleteResponse = await _IVATReportMappingRepository.DeleteEntity(response.TEntities.ToArray());
                    return Ok(deleteResponse);
                }
                return BadRequest("Invalid Request Id");

            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                        nameof(GetVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// UpdateVAT Report Mapping
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateVATReportMapping(VATReportMapping model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var updateResponse = await _IVATReportMappingRepository.UpdateEntity(model);
                    return Ok(updateResponse);
                }
                return BadRequest("Invalid data");


            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                        nameof(GetVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }


        /// <summary>
        /// Get TAX Vat Report Mapping detail will get complete details..
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetTaxVATReportMapping()
        {
            try
            {
                var response = _IVATReportMasterRepository.GetAll<VATReportMappingModel>
                    (SqlQueryHelper.GetVATTaxRateMapping, null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(VATReportMappingController),
                        nameof(GetVATReportMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }
    }
}
