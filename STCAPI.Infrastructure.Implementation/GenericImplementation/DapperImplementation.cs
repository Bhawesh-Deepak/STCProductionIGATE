using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using STAAPI.Infrastructure.Repository.GenericRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace STCAPI.Infrastructure.Implementation.GenericImplementation
{
    public class DapperImplementation<TParams> : IDapperRepository<TParams>
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DapperImplementation(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetSection("ConnectionStrings:DefaultConnection").Value;
        }
        public void Dispose()
        {

        }

        public TModel Execute<TModel>(string sp, TParams entity)
        {
            try
            {
                TModel result;

                using IDbConnection db = new MySqlConnection(_connectionString);

                if (db.State == ConnectionState.Closed)
                    db.Open();

                var parms = ConvertObjectToDBParameter<TParams>(entity);
                result = db.Query<TModel>(sp, parms, commandType: CommandType.Text, transaction: null).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public TModel Get<TModel>(string sp, TParams entity)
        {
            DynamicParameters parms = null;
            if (entity != null)
            {
                parms = ConvertObjectToDBParameter<TParams>(entity);
            }

            using IDbConnection db = new MySqlConnection(_connectionString);
            return db.Query<TModel>(sp, parms, commandType: CommandType.Text).FirstOrDefault();
        }

        public IEnumerable<TModel> GetAll<TModel>(string sp, TParams entity)
        {
            try
            {
                DynamicParameters parms = null;
                if (entity != null)
                {
                    parms = ConvertObjectToDBParameter<TParams>(entity);
                }
                using IDbConnection db = new MySqlConnection(_connectionString);
                return db.Query<TModel>(sp, parms, commandType: CommandType.Text).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public DbConnection GetDbconnection()
        {
            return new MySqlConnection(_config.GetSection("ConnectionStrings:DefaultConnection").Value);
        }

        public DynamicParameters ConvertObjectToDBParameter<T>(T entity)
        {
            Type t = entity.GetType();
            var paramData = new DynamicParameters();
            foreach (var entityProp in t.GetProperties())
            {
                paramData.Add($"@{entityProp.Name}", entityProp.GetValue(entity, null), GetDbType(entityProp.PropertyType));
            }
            return paramData;
        }
        public static DbType GetDbType(Type runtimeType)
        {
            var nonNullableType = Nullable.GetUnderlyingType(runtimeType);

            if (nonNullableType != null)
            {
                runtimeType = nonNullableType;
            }

            var templateValue = (Object)null;
            if (runtimeType.IsClass == false)
            {
                templateValue = Activator.CreateInstance(runtimeType);
            }

            var sqlParamter = new SqlParameter(parameterName: String.Empty, value: templateValue);

            return sqlParamter.DbType;
        }

    }
}
