using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Controllers.Configuration;
using STCAPI.Core.Entities.Configuration;
using STCAPI.Core.Entities.LogDetail;
using STCAPI.Core.Entities.Logger;
using STCAPI.DataLayer.AdminPortal;
using STCAPI.Infrastructure.Implementation.GenericImplementation;
using System.Configuration;
using Xunit;

namespace STCAPI_UnitAPI
{
    public class ConfigurationMasterTest
    {
        private  IGenericRepository<StageMaster, int> _IStageMasterRepository;
        private  IGenericRepository<StreamMaster, int> _IStreamMasterRepository;
        private  IGenericRepository<MainStreamMaster, int> _IMainStreamRepository;
        private  IGenericRepository<LogDetail, int> _ILogeDetailRepository;
        private  IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        private  IGenericRepository<ConfigurationMaster, int> _IConfigurationMaster;
        private  IConfiguration configuration;


        public IConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
                    configuration = builder.Build();
                }

                return configuration;
            }
        }

        public void ConfigurationMasterTestInjection()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(Configuration);

            _IStreamMasterRepository = new DetailImplementation<StreamMaster, int>(configuration);
            _IStageMasterRepository = new DetailImplementation<StageMaster, int>(configuration);
            _IMainStreamRepository = new DetailImplementation<MainStreamMaster, int>(configuration);
            _ILogeDetailRepository = new DetailImplementation<LogDetail, int>(configuration);
            _IErrorLogRepository = new DetailImplementation<ErrorLogModel, int>(configuration);
            _IConfigurationMaster = new DetailImplementation<ConfigurationMaster, int>(configuration);
        }

        [Fact]
        public async void Task_Get_ValidStageDetail_Return_OkResult()
        {
            ConfigurationMasterTestInjection();

            var controller = new ConfigurationController(_IStageMasterRepository, _IStreamMasterRepository,
                _IMainStreamRepository, _ILogeDetailRepository, _IConfigurationMaster, _IErrorLogRepository);

            //Act
            var response = await controller.GetStageDetail();

            //Assert

            Assert.IsType<OkObjectResult>(response);

        }

        //[Fact]
        
    }
}