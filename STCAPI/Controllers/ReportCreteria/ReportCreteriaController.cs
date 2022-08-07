using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.ReportCreteria;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.ReportCreteria
{
    /// <summary>
    ///  Report Creteria Details Api's
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportCreteriaController : ControllerBase
    {
        private readonly IGenericRepository<ObjectMapping, int> _IConfigurationMasterRepository;
        private readonly IGenericRepository<ReportCreteriaModel, int> _IReportCreteriaRepository;
        private IHostingEnvironment _IHostingEnvironment;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required dependency in the controller contructor
        /// </summary>
        /// <param name="configurationMasterRepository"></param>
        /// <param name="reportCreteriaRepository"></param>
        /// <param name="environment"></param>
        public ReportCreteriaController(IGenericRepository<ObjectMapping, int> configurationMasterRepository,
            IGenericRepository<ReportCreteriaModel, int> reportCreteriaRepository,
            IHostingEnvironment environment, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IConfigurationMasterRepository = configurationMasterRepository;
            _IReportCreteriaRepository = reportCreteriaRepository;
            _IHostingEnvironment = environment;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Report Creteria Details
        /// </summary>
        /// <param name="modelData"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReportCreteria(ReportCreteriaVm modelData)
        {
            try
            {
                var model = new ReportCreteriaModel()
                {
                    ObjectMappingId = modelData.Id,
                    UserName = modelData.UserName,
                    CSVUrlPath = modelData.CSVUrlPath,
                    JsonRule = modelData.JsonRule,
                    Creteria = modelData.Creteria,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = modelData.UserName,
                    CreatedDate = DateTime.Now
                };

                var ExistDataModel = await _IReportCreteriaRepository.GetAllEntities(x => x.IsActive
                 && x.UserName.Trim().ToLower() == modelData.UserName.Trim().ToLower() && x.ObjectMappingId == modelData.Id);

                if (ExistDataModel != null && ExistDataModel.TEntities.Any())
                {
                    ExistDataModel.TEntities.ToList().ForEach(data =>
                    {
                        data.IsActive = false;
                        data.IsDeleted = true;
                        data.UpdatedDate = DateTime.Now;
                        data.UpdatedBy = modelData.UserName;
                    });
                    var deleteRepsonse = await _IReportCreteriaRepository.DeleteEntity(ExistDataModel.TEntities.ToArray());
                }

                var response = await _IReportCreteriaRepository.CreateEntity(new List<ReportCreteriaModel>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReportCreteriaController),
                   nameof(CreateReportCreteria), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Report Creteria.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetReportCreteria()
        {
            try
            {
                var models = new List<ReportCreteriaResponseVm>();

                var response = await _IReportCreteriaRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var reportDetails = await _IConfigurationMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                if (reportDetails != null && reportDetails.TEntities.Any())
                {
                    reportDetails.TEntities.ToList().ForEach(data =>
                    {
                        var modelVm = new ReportCreteriaResponseVm()
                        {
                            Id = data.Id,
                            ReportName = data.Name,
                            ReportNumber = data.ObjectNumber,
                            ShortName = data.ShortName,
                            LongName = data.LongName,
                            Description = data.Description
                        };

                        models.Add(modelVm);
                    });
                }

                if (response != null && response.TEntities.Any())
                {
                    models.ForEach(data =>
                    {
                        response.TEntities.ToList().ForEach(item =>
                        {
                            if (data.Id == item.ObjectMappingId)
                            {
                                data.Criteria = item.Creteria;
                                data.JsonRule = item.JsonRule;

                            }
                        });
                    });
                }

                return Ok(models);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReportCreteriaController),
                  nameof(GetReportCreteria), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// GetReportCSVData Details
        /// </summary>
        /// <param name="reportName"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("text/csv")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetReportCSVData(string reportName)
        {
            try
            {
                string path = _IHostingEnvironment.WebRootPath + "\\ReportCSVPath\\" + reportName;
                string data = System.IO.File.ReadAllText(path, Encoding.UTF8);

                return await Task.Run(() => Ok(data));
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(ReportCreteriaController),
                    nameof(GetReportCSVData), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
