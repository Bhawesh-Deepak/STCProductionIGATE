using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.STCVAT;
using System;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.STCVATRepository
{
    public interface ISTCPOstValidationRepository
    {
        Task<ResponseModel<STCPostValidation, int>> GetAllEntities(Func<STCPostValidation, bool> where);
        Task<ResponseModel<STCPostValidation, int>> CreateEntity(STCPostValidation[] model);
    }
}
