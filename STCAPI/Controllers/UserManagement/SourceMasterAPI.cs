using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.UserManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SourceMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<SourceMaster, int> _ISourceMasterRepository;
        public SourceMasterAPI(IGenericRepository<SourceMaster, int> sourceMasterRepo)
        {
            _ISourceMasterRepository = sourceMasterRepo;
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSourceMaster(SourceMaster model)
        {
            var response = await _ISourceMasterRepository.CreateEntity(new List<SourceMaster>() { model }.ToArray());
            return Ok(response);
        }
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSourceMasterDetails()
        {
            var response = await _ISourceMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Ok(response);
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSourceMasterDetails(SourceMaster model)
        {
            var deleteModel = await _ISourceMasterRepository.GetAllEntities(x => x.Id == model.Id);
            deleteModel.TEntities.ToList().ForEach(x =>
            {
                x.IsActive = false;
                x.IsDeleted = true;
            });
            var deleteResponse = await _ISourceMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            model.Id = 0;
            var createResponse = await _ISourceMasterRepository.CreateEntity(new List<SourceMaster>() { model }.ToArray());
            return Ok(createResponse);
        }
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteSourceMaster(int id)
        {
            var deleteModel = await _ISourceMasterRepository.GetAllEntities(x => x.Id == id);
            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });
            var deleteResponse = await _ISourceMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            return Ok(deleteResponse);
        }
    }
}
