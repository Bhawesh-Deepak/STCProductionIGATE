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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RawDataLinkAPI : ControllerBase
    {
        private readonly IGenericRepository<RawDataLink, int> _IRawDataLinkRepository;
        private readonly IGenericRepository<MainStreamMaster, int> _IMainStreamMasterRepository;
        private readonly IGenericRepository<StreamMaster, int> _IStreamMasterRepository;
        private readonly IGenericRepository<SourceMaster, int> _ISourceMasterRepository;
        private readonly IGenericRepository<RawDataStream, int> _IRawDataStreamRepository;
        private readonly IGenericRepository<SubsidryModel, int> _ISubsidryFormRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        public RawDataLinkAPI(IGenericRepository<RawDataLink, int> RawDataLinkRepo,
            IGenericRepository<MainStreamMaster, int> MainStreamMasterRepo,
             IGenericRepository<StreamMaster, int> StreamMasterRepo,
             IGenericRepository<SourceMaster, int> sourceMasterRepo,
             IGenericRepository<RawDataStream, int> RawDataStreamRepo,
             IGenericRepository<SubsidryModel, int> SubsidryFormRepo, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IRawDataLinkRepository = RawDataLinkRepo;
            _IMainStreamMasterRepository = MainStreamMasterRepo;
            _IStreamMasterRepository = StreamMasterRepo;
            _ISourceMasterRepository = sourceMasterRepo;
            _IRawDataStreamRepository = RawDataStreamRepo;
            _ISubsidryFormRepository = SubsidryFormRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Raw Data Link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateRawDataLink(RawDataLink model)
        {
            try
            {
                var response = await _IRawDataLinkRepository.CreateEntity(new List<RawDataLink>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataLinkAPI),
                        nameof(CreateRawDataLink), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get Raw data link Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetRawDataLinkDetails()
        {
            try
            {
                var response = await _IRawDataLinkRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var MainStreamList = await _IMainStreamMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var StreamList = await _IStreamMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var SourceList = await _ISourceMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var RawDataList = await _IRawDataStreamRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var SubsidryFormList = await _ISubsidryFormRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                var responseDataList = from Mapping in response.TEntities
                                       join mainstream in MainStreamList.TEntities on Mapping.MainStreamId equals mainstream.Id
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
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataLinkAPI),
                             nameof(GetRawDataLinkDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Raw data link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateRawDataLinkDetails(RawDataLink model)
        {
            try
            {
                var deleteModel = await _IRawDataLinkRepository.GetAllEntities(x => x.Id == model.Id);
                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });
                var deleteResponse = await _IRawDataLinkRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                model.Id = 0;
                var createResponse = await _IRawDataLinkRepository.CreateEntity(new List<RawDataLink>() { model }.ToArray());
                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataLinkAPI),
                        nameof(UpdateRawDataLinkDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete Raw Data Link
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteRawDataLink(int id)
        {
            try
            {
                var deleteModel = await _IRawDataLinkRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });
                var deleteResponse = await _IRawDataLinkRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(deleteResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataLinkAPI),
                            nameof(DeleteRawDataLink), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
    }
}
