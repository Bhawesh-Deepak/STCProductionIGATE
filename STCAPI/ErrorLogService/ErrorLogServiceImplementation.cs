using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STCAPI.ErrorLogService
{
    public class ErrorLogServiceImplementation
    {
        public static async Task<bool> LogError(IGenericRepository<ErrorLogModel, int> _IErrorLogRepository,
            string controllerName, string actionName, string exceptionMessage, string innerExceptionMessage)
        {

            await _IErrorLogRepository.CreateEntity(new List<ErrorLogModel>()
                    {
                        new ErrorLogModel()
                        {
                             ActionName= actionName,
                             ControllerName= controllerName,
                             ExceptionMessage= exceptionMessage,
                             InnerException= innerExceptionMessage,
                             CreatedBy="Admin",
                             CreatedDate= DateTime.Now,
                             Status= false,
                             IsActive=true,
                             IsDeleted= false
                        }
                }.ToArray());

            return true;
        }
    }
}
