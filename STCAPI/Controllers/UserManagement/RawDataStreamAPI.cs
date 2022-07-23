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
    /// RawData Stream Api
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RawDataStreamAPI : ControllerBase
    {
        private readonly IGenericRepository<RawDataStream, int> _IRawDataStreamRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject service to constructor controller
        /// </summary>
        /// <param name="rawDataStreamRepo"></param>
        /// <param name="errorLogRepository"></param>
        public RawDataStreamAPI(IGenericRepository<RawDataStream, int> rawDataStreamRepo,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IRawDataStreamRepository = rawDataStreamRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Raw Data Stream
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateRawDataStream(RawDataStream model)
        {
            try
            {
                var response = await _IRawDataStreamRepository.CreateEntity(new List<RawDataStream>() { model }.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataStreamAPI),
                        nameof(CreateRawDataStream), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }
        /// <summary>
        /// Get Raw Data Stream Details
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetRawDataStream()
        {
            try
            {
                var response = await _IRawDataStreamRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataStreamAPI),
                        nameof(GetRawDataStream), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Delete Raw Data Stream
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteRawDataStream(int id)
        {
            try
            {
                var deleteModel = await _IRawDataStreamRepository.GetAllEntities(x => x.Id == id);
                deleteModel.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var response = await _IRawDataStreamRepository.DeleteEntity(deleteModel.TEntities.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataStreamAPI),
                        nameof(DeleteRawDataStream), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Update Raw Data Stream Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateRawStreamData(RawDataStream model)
        {
            try
            {
                var deleteModels = await _IRawDataStreamRepository.GetAllEntities(x => x.Id == model.Id);

                deleteModels.TEntities.ToList().ForEach(data =>
                {
                    data.IsActive = false;
                    data.IsDeleted = true;
                });

                var deleteResponse = await _IRawDataStreamRepository.DeleteEntity(deleteModels.TEntities.ToArray());

                model.Id = 0;
                var createResponse = await _IRawDataStreamRepository.CreateEntity(new List<RawDataStream> { model }.ToArray());

                return Ok(createResponse);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RawDataStreamAPI),
                        nameof(UpdateRawStreamData), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

    }
}
