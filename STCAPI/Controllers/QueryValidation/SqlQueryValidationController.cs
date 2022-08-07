using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.SqlQueryValidation;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.QueryValidation
{
    /// <summary>
    /// SQlQueryValidationController
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SqlQueryValidationController : ControllerBase
    {
        private readonly IGenericRepository<SqlQueryValidationModel, int> _sqlQueryValidationRepo;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="sqlQueryRepo"></param>
        public SqlQueryValidationController(IGenericRepository<SqlQueryValidationModel, int> sqlQueryRepo, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _sqlQueryValidationRepo = sqlQueryRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateQuery(SqlQueryValidationModel model)
        {
            try
            {
                model.IsActive = true;
                model.IsDeleted = false;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                model.CreatedBy = model.CreatedBy ?? "Admin";

                //Get MaxMonth from SqlValidator Based on Stream name and Active; if MaxMonth is greater than the database maxMonth 
                //Then insert the record with Active else mark the record deactive and insert this to table. With Previous Data as active
                var validatorDetails = await _sqlQueryValidationRepo.GetAllEntities(x => x.Stream.ToLower().Trim()
                == model.Stream.ToLower().Trim() && x.IsActive && !x.IsDeleted);

                if (validatorDetails.TEntities.Any())
                {
                    var maxMonth = validatorDetails.TEntities.First().MaxMonth;
                    if (model.MaxMonth > maxMonth)
                    {
                        model.IsActive = true;
                        model.IsDeleted = false;

                        validatorDetails.TEntities.ToList().ForEach(data =>
                        {
                            data.IsActive = false;
                            data.IsDeleted = true;
                        });
                        await _sqlQueryValidationRepo.DeleteEntity(validatorDetails.TEntities.ToArray());
                    }
                    else
                    {
                        model.IsDeleted = true;
                        model.IsActive = false;
                    }
                }


                var respons = await _sqlQueryValidationRepo.CreateEntity(new List<SqlQueryValidationModel>() { model }.ToArray());
                return Ok(respons);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SqlQueryValidationController),
                      nameof(CreateQuery), ex.Message, ex.ToString());

                return BadRequest(new ResponseModel<SqlQueryValidationModel, int>());
            }

        }


        /// <summary>
        /// Get complete query for validation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetQuery()
        {
            try
            {
                var respons = await _sqlQueryValidationRepo.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                return Ok(respons);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(SqlQueryValidationController),
                     nameof(GetQuery), ex.Message, ex.ToString());

                return BadRequest("Issue Occured, Please contact admin Team !");
            }

        }
    }
}
