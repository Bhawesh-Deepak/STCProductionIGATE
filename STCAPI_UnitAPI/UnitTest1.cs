using Microsoft.Extensions.Configuration;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.Infrastructure.Implementation.GenericImplementation;
using Xunit;

namespace STCAPI_UnitAPI
{
    public class ConfigurationMasterTest
    {
        private IGenericRepository<StageMaster, int> _IStageMasterRepository;
        private readonly IConfiguration configuration;


        [Fact]
        public void GetStageDetailTest()
        {
            _IStageMasterRepository = new DetailImplementation<StageMaster, int>(configuration);
        }
    }
}