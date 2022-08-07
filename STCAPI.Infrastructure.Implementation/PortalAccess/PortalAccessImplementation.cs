using Microsoft.Extensions.Configuration;
using MySqlConnector;
using STAAPI.Infrastructure.Repository.PortalAccessRepository;
using STCAPI.Core.Entities.Context;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Infrastructure.Implementation.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STCAPI.Infrastructure.Implementation.PortalAccess
{
    public class PortalAccessImplementation : IPortalAccessRepository
    {
        private readonly string _connectionString;
        private STCContext context;
        public PortalAccessImplementation(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            context = new STCContext(configuration);
        }

        public async Task<List<PortalAccessVm>> GetPortalAccessDetail()
        {
            var models = new List<PortalAccessVm>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();

                var command = new MySqlCommand("SELECT * FROM Vw_PortalAccessDetail", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var model = new PortalAccessVm();
                        model.Id = reader.DefaultIfNull<int>("Id");
                        model.StageName = reader.DefaultIfNull<string>("StageName");
                        model.StageLongName = reader.DefaultIfNull<string>("StageLongName");
                        model.StageShortName = reader.DefaultIfNull<string>("StageShortName");
                        model.MainStreamName = reader.DefaultIfNull<string>("MainStreamName");
                        model.MainStreamLongName = reader.DefaultIfNull<string>("MainStreamLongName");
                        model.MainStreamShortName = reader.DefaultIfNull<string>("MainStreamShortName");
                        model.StreamName = reader.DefaultIfNull<string>("StreamName");
                        model.StreamLongName = reader.DefaultIfNull<string>("StreamLongName");
                        model.StreamShortName = reader.DefaultIfNull<string>("StreamShortName");
                        model.ObjectName = reader.DefaultIfNull<string>("ObjectName");
                        model.ObjectLongName = reader.DefaultIfNull<string>("ObjectLongName");
                        model.ObjectShortName = reader.DefaultIfNull<string>("ObjectShortName");

                        models.Add(model);

                    }
                }

                return await Task.Run(() => models);
            }
        }


    }
}
