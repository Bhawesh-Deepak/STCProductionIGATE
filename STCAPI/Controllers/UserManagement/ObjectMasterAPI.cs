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
    public class ObjectMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<ObjectMaster, int> _IObjectMasterRepository;
        public ObjectMasterAPI(IGenericRepository<ObjectMaster, int> ObjectMasterRepo)
        {
            _IObjectMasterRepository = ObjectMasterRepo;
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateObjectMaster(ObjectMaster model)
        {
            var response = await _IObjectMasterRepository.CreateEntity(new List<ObjectMaster>() { model }.ToArray());
            return Ok(response);
        }
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetObjectMasterDetails()
        {
            var response = await _IObjectMasterRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Ok(response);
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateObjectMasterDetails(ObjectMaster model)
        {
            var deleteModel = await _IObjectMasterRepository.GetAllEntities(x => x.Id == model.Id);
            deleteModel.TEntities.ToList().ForEach(x =>
            {
                x.IsActive = false;
                x.IsDeleted = true;
            });
            var deleteResponse = await _IObjectMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            model.Id = 0;
            var createResponse = await _IObjectMasterRepository.CreateEntity(new List<ObjectMaster>() { model }.ToArray());
            return Ok(createResponse);
        }
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteObjectMaster(int id)
        {
            var deleteModel = await _IObjectMasterRepository.GetAllEntities(x => x.Id == id);
            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });
            var deleteResponse = await _IObjectMasterRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            return Ok(deleteResponse);
        }
    }
}
