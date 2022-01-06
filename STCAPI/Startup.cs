using MailHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STAAPI.Infrastructure.Repository.PortalAccessRepository;
using STAAPI.Infrastructure.Repository.STCVATRepository;
using STCAPI.Helper;
using STCAPI.Infrastructure.Implementation.GenericImplementation;
using STCAPI.Infrastructure.Implementation.PortalAccess;
using STCAPI.Infrastructure.Implementation.STCVATFormImplemetation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace STCAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "STCAPI", Version = "v1" });
                c.DescribeAllEnumsAsStrings();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddAuthenticationToken(Configuration);

            services.AddTransient(typeof(IGenericRepository<,>), typeof(DetailImplementation<,>));
            services.AddTransient<ISTACVATFormRepository, STCVATFormDetail>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<ISTCPOstValidationRepository, STCPostValidationDetail>();
            services.AddTransient<IReconcilationSummaryRepository, ReconcilationSummaryImplementation>();
            services.AddTransient<IPortalAccessRepository, PortalAccessImplementation>();


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddCors(option => {
                option.AddPolicy("AllowAnyOrigin", 
                   option => option.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                );
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "STCAPI v1"));
            
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.UseCors("AllowAnyOrigin");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
