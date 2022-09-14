using STCAPI.Core.Entities.Common;
using System;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.GenericRepository
{
    public interface IGenericRepository<TEntity, T> where TEntity : class
    {
        /// <summary>
        /// Get All entities by the help of where condition we provided.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<ResponseModel<TEntity, T>> GetAllEntities(Func<TEntity, bool> where);

        /// <summary>
        /// Create multiple entity or single entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel<TEntity, T>> CreateEntity(TEntity[] model);

        /// <summary>
        /// Update single entity or multiple entity 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResponseModel<TEntity, T>> UpdateEntity(TEntity model);

        /// <summary>
        /// Delete multiple entity or single entity It is soft delete hence It will become IsDeleted = true and IsActive= false
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task<ResponseModel<TEntity, T>> DeleteEntity(params TEntity[] items);
        Task<ResponseModel<TEntity, T>> CheckIsExists(Func<TEntity, bool> where);
    }
}
