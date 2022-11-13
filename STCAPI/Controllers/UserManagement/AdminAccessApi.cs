using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// Admin Access and configuration details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminAccessApi : ControllerBase
    {
        private readonly IGenericRepository<AdminAccess, int> _IAdminAccessRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to constructor
        /// </summary>
        /// <param name="iAdminAccessRepository"></param>
        /// <param name="errorLogRepository"></param>
        public AdminAccessApi(IGenericRepository<AdminAccess, int> iAdminAccessRepository,
            IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IAdminAccessRepository = iAdminAccessRepository;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        ///  Get User Admin access Details based on UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetAdminAccess(string userId)
        {
            try
            {
                var response = await _IAdminAccessRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted
                    && x.IsAdminAccess && x.UserName.Trim().ToLower() == userId.Trim().ToLower());
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(AdminAccessApi),
                      nameof(GetAdminAccess), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Create multiple user admin access 
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePermission(List<AdminAccessVm> models)
        {
            try
            {
                await DeletePreviousAdminAccess(models);

                var dbModels = new List<AdminAccess>();

                models.ForEach(data =>
                {
                    var dbModel = new AdminAccess()
                    {
                        CreatedBy = data.UserName,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        UserName = data.UserName,
                        IsAdminAccess = data.IsAdminAccess,
                        IsActive = true,
                        IsDeleted = false,
                    };

                    dbModels.Add(dbModel);
                });

                var response = await _IAdminAccessRepository.CreateEntity(dbModels.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(AdminAccessApi),
                         nameof(CreatePermission), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get User Permission details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetUserPersmision()
        {
            try
            {
                var response = await _IAdminAccessRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(response);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(AdminAccessApi),
                      nameof(GetUserPersmision), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Delete previous admin access details
        /// </summary>
        /// <returns></returns>
        private async Task<bool> DeletePreviousAdminAccess(List<AdminAccessVm> models)
        {
            var dbResponseModels = await _IAdminAccessRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            models.ForEach(item =>
            {
                dbResponseModels.TEntities.ToList().ForEach(data =>
                {
                    if (item.UserName.Trim().ToLower() == data.UserName.Trim().ToLower())
                    {
                        data.IsActive = false;
                        data.IsDeleted = true;
                        data.UpdatedDate = DateTime.Now;
                        data.UpdatedBy = data.UserName;
                    }

                });

            });

            var response = await _IAdminAccessRepository.DeleteEntity(dbResponseModels.TEntities.ToArray());

            return response.ResponseStatus == Core.Entities.Common.ResponseStatus.Success;

        }

    }
}
