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
    public class ObjectMappingAPI : ControllerBase
    {
        private readonly IGenericRepository<ObjectMapping, int> _IObjectMappingRepository;
        public ObjectMappingAPI(IGenericRepository<ObjectMapping, int> ObjectMappingRepo)
        {
            _IObjectMappingRepository = ObjectMappingRepo;
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateObjectMapping(ObjectMapping model)
        {
            var response = await _IObjectMappingRepository.CreateEntity(new List<ObjectMapping>() { model }.ToArray());
            return Ok(response);
        }
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetObjectMappingDetails()
        {
            var response = await _IObjectMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Ok(response);
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateObjectMappingDetails(ObjectMapping model)
        {
            var deleteModel = await _IObjectMappingRepository.GetAllEntities(x => x.Id == model.Id);

            deleteModel.TEntities.ToList().ForEach(x =>
            {
                x.IsActive = false;
                x.IsDeleted = true;
            });

            var deleteResponse = await _IObjectMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            model.Id = 0;
            var createResponse = await _IObjectMappingRepository.CreateEntity(new List<ObjectMapping>() { model }.ToArray());
            return Ok(createResponse);
        }
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteObjectMapping(int id)
        {
            var deleteModel = await _IObjectMappingRepository.GetAllEntities(x => x.Id == id);
            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });
            var deleteResponse = await _IObjectMappingRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            return Ok(deleteResponse);
        }
    }
}
