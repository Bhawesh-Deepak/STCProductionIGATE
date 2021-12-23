using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Subsidry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubsidryController : ControllerBase
    {
        private readonly IGenericRepository<SubsidryModel, int> _ISubsidryModelRepository;
        private readonly IGenericRepository<SubsidryUserMapping, int> _ISubsidryUserMapping;

        public SubsidryController(IGenericRepository<SubsidryModel, int> subsidryModelRepository,
            IGenericRepository<SubsidryUserMapping, int> subsidryUserMappingRepository)
        {
            _ISubsidryModelRepository = subsidryModelRepository;
            _ISubsidryUserMapping = subsidryUserMappingRepository;
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSubsidry(SubsidryModel model)
        {
            var response = await _ISubsidryModelRepository.CreateEntity(new List<SubsidryModel>() { model }.ToArray());
            return Ok(response);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSubsidry(SubsidryModel model)
        {
            var deleteModel = await _ISubsidryModelRepository.GetAllEntities(x => x.Id == model.Id);

            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });

            var deleteResponse = await _ISubsidryModelRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            model.Id = 0;
            var response = await _ISubsidryModelRepository.CreateEntity(new List<SubsidryModel>() { model }.ToArray());
            return Ok(response);
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSubsidry()
        {
            var response = await _ISubsidryModelRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Ok(response);
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteSubsidry(int id)
        {
            var deleteModel = await _ISubsidryModelRepository.GetAllEntities(x => x.Id == id);
            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });

            var deleteResponse = await _ISubsidryModelRepository.DeleteEntity(deleteModel.TEntities.ToArray());
            return Ok(deleteResponse);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateSubsidryUserMapping(SubsidryUserMapping model)
        {
            var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.UserName == model.UserName);

            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });

            var deleteReponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());

            model.Id = 0;

            var createRepsonse = await _ISubsidryUserMapping.CreateEntity(new List<SubsidryUserMapping>() { model }.ToArray());

            return Ok(createRepsonse);
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteSubsidryMapping(int id)
        {
            var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.Id == id);

            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });

            var deleteResponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());
            return Ok(deleteResponse);
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetSubsidryMappingDetails()
        {
            var response = await _ISubsidryUserMapping.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            return Ok(response);
        }
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateSubsidryMapping(SubsidryUserMapping model)
        {
            var deleteModel = await _ISubsidryUserMapping.GetAllEntities(x => x.Id == model.Id);

            deleteModel.TEntities.ToList().ForEach(data =>
            {
                data.IsActive = false;
                data.IsDeleted = true;
            });

            var deleteResponse = await _ISubsidryUserMapping.DeleteEntity(deleteModel.TEntities.ToArray());
            model.Id = 0;
            var response = await _ISubsidryUserMapping.CreateEntity(new List<SubsidryUserMapping>() { model }.ToArray());
            return Ok(response);
        }
    }
}
