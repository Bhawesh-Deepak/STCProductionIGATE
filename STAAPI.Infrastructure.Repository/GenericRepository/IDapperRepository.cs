using System;
using System.Collections.Generic;
using System.Data.Common;


namespace STAAPI.Infrastructure.Repository.GenericRepository
{
    public interface IDapperRepository<TParams> : IDisposable
    {
        DbConnection GetDbconnection();
        TModel Get<TModel>(string sp, TParams entity);
        IEnumerable<TModel> GetAll<TModel>(string sp, TParams entity);
        TModel Execute<TModel>(string sp, TParams entity);
    }
}
