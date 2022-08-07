using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Subsidry;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Core.ViewModel.ResponseModel;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// Source Data Mapping
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceDataMappingAPI : ControllerBase
    {
        private readonly IGenericRepository<SourceDataMapping, int> _ISourceDataMappingRepository;
        private readonly IGenericRepository<MainStreamMaster, int> _IMainStreamMasterRepository;
        private readonly IGenericRepository<StreamMaster, int> _IStreamMasterRepository;
        private readonly IGenericRepository<SourceMaster, int> _ISourceMasterRepository;
        private readonly IGenericRepository<RawDataStream, int> _IRawDataStreamRepository;
        private readonly IGenericRepository<SubsidryModel, int> _ISubsidryFormRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        /// <summary>
        /// Inject requierd service to constructor of the controller
        /// </summary>
        /// <param name="SourceDataMappingRepo"></param>
        /// <param name="MainStreamMasterRepo"></param>
        /// <param name="StreamMasterRepo"></param>
        /// <param name="sourceMasterRepo"></param>
        /// <param name="RawDataStreamRepo"></param>
        /// <param name="SubsidryFormRepo"></param>
        /// <param name="errorLogRepository"></param>
        public SourceDataMappingAPI(IGenericRepository<SourceDataMapping, int> SourceDataMappingRepo,
            IGenericRepository<MainStreamMaster, int> MainStreamMasterRepo,
             IGenericRepository<StreamMaster, int> StreamMasterRepo,
             IGenericRepository<SourceMaster, int> sourceMasterRepo,
             IGenericRepository<RawDataStream, int> RawDataStreamRepo,
             IGenericRepository<SubsidryModel, int> SubsidryFormRepo,
             IGenericRepository<ErrorLogModel, int> errorLogRepository
             )
        {
            _ISourceDataMappingRepository = SourceDataMappingRepo;
            _IMainStreamMasterRepository = MainStreamMasterRepo;
            _IStreamMasterRepository = StreamMasterRepo;
            _ISourceMasterRepository = sourceMasterRepo;
            _IRawDataStreamRepository = RawDataStreamRepo;
            _ISubsidryFormRepository = SubsidryFormRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Source Data Mapping
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSourceDataMapping(SourceDataMapping model)
        {
            try
            {
                var response = await _ISourceDataMappingRepository.CreateEntity(new List<SourceDataMapping>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceDataMappingAPI),
                         nameof(CreateSourceDataMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Source Data Mapping Details Api's
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSourceDataMappingDetails()
        {
            try
            {
                var response = await _ISourceDataMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var MainStreamList = await _IMainStreamMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var StreamList = await _IStreamMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var SourceList = await _ISourceMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var RawDataList = await _IRawDataStreamRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var SubsidryFormList = await _ISubsidryFormRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var responseDataList = from Mapping in response.TEntities
                                       join mainstream in MainStreamList.TEntities on Mapping.MainStreemId equals mainstream.Id
                                       join stream in StreamList.TEntities on Mapping.StreamId equals stream.Id
                                       join source in SourceList.TEntities on Mapping.SourceId equals source.Id
                                       join rawdata in RawDataList.TEntities on Mapping.RawDataId equals rawdata.Id
                                       join subsidry in SubsidryFormList.TEntities on Mapping.CompanyId equals subsidry.Id
                                       select new SourceDataMappingVM
                                       {
                                           Id = Mapping.Id,
                                           MainStreamName = mainstream.Name,
                                           StreamName = stream.Name,
                                           SourceName = source.Name,
                                           RawDataName = rawdata.Name,
                                           CompanyName = subsidry.Name,

                                       };
                return Ok(responseDataList);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceDataMappingAPI),
                     nameof(GetSourceDataMappingDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Source Data Mapping Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSourceDataMappingDetails(SourceDataMapping model)
        {
            try
            {
                var deleteModel = await _ISourceDataMappingRepository.GetAllEntities(x => x.Id == model.Id);
                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });
                var deleteResponse = await _ISourceDataMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _ISourceDataMappingRepository.CreateEntity(new List<SourceDataMapping>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceDataMappingAPI),
                    nameof(UpdateSourceDataMappingDetails), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete source mapping details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteSourceDataMapping(int id)
        {
            try
            {
                var deleteModel = await _ISourceDataMappingRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });
                var deleteResponse = await _ISourceDataMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SourceDataMappingAPI),
                     nameof(DeleteSourceDataMapping), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
