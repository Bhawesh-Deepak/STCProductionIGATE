using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.STCVAT;
using System;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.GenericRepository
{
    public interface ISTACVATFormRepository
    {
        Task<ResponseModel<STCVATForm, int>> GetAllEntities(Func<STCVATForm, bool> where);
        Task<ResponseModel<STCVATForm, int>> CreateEntity(STCVATForm[] model);
        Task<ResponseModel<STCVATForm, int>> UpdateEntity(STCVATForm model);
        Task<ResponseModel<STCVATForm, int>> DeleteEntity(params STCVATForm[] items);
        Task<ResponseModel<STCVATForm, int>> CheckIsExists(Func<STCVATForm, bool> where);
    }
}
