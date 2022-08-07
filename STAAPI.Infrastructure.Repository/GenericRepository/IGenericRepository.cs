using STCAPI.Core.Entities.Common;
using System;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.GenericRepository
{
    public interface IGenericRepository<TEntity, T> where TEntity : class
    {
        Task<ResponseModel<TEntity, T>> GetAllEntities(Func<TEntity, bool> where);
        Task<ResponseModel<TEntity, T>> CreateEntity(TEntity[] model);
        Task<ResponseModel<TEntity, T>> UpdateEntity(TEntity model);
        Task<ResponseModel<TEntity, T>> DeleteEntity(params TEntity[] items);
        Task<ResponseModel<TEntity, T>> CheckIsExists(Func<TEntity, bool> where);
    }
}
