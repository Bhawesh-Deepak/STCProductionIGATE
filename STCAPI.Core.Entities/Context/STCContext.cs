using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STCAPI.Core.Entities.Configuration;
using STCAPI.Core.Entities.IGATE;
using STCAPI.Core.Entities.InvoiceDetails;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.Master;
using STCAPI.Core.Entities.MenuSubMenu;
using STCAPI.Core.Entities.Reconcilation;
using STCAPI.Core.Entities.Report;
using STCAPI.Core.Entities.ReportCreteria;
using STCAPI.Core.Entities.RequestDetail;
using STCAPI.Core.Entities.SqlQueryValidation;
using STCAPI.Core.Entities.STCVAT;
using STCAPI.Core.Entities.Subsidry;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Core.Entities.VATDetailUpload;
using STCAPI.Core.Entities.VATReport;
using STCAPI.DataLayer.AdminPortal;

namespace STCAPI.Core.Entities.Context
{
    public class STCContext : DbContext
    {
        private readonly string _connectionString;

        public STCContext() { }
        public STCContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DemoConfiguration());
            modelBuilder.ApplyConfiguration(new STCVATConfiguration());
            //modelBuilder.Query<PortalAccessVm>().ToView("AuthorArticleCount");
        }

        public virtual DbSet<STCVATForm> STCVATForms { get; set; }
        public virtual DbSet<STCPostValidation> STCPostValidations { get; set; }
        public virtual DbSet<StageModel> StageModels { get; set; }
        public virtual DbSet<MainLevel> MainLevels { get; set; }
        public virtual DbSet<StreamModel> StreamModels { get; set; }
        public virtual DbSet<RecincilationSummary> RecincilationSummaries { get; set; }
        public virtual DbSet<InputVATDataFile> InputVATDataFiles { get; set; }
        public virtual DbSet<VATTrailBalanceModel> VATTrailBalanceModels { get; set; }
        public virtual DbSet<STCVATOutputModel> STCVATOutputModels { get; set; }
        public virtual DbSet<VATReturnModel> VATReturnModels { get; set; }
        public virtual DbSet<UploadInvoiceDetail> UploadInvoiceDetails { get; set; }
        public virtual DbSet<RequestDetailModel> RequestDetailModels { get; set; }
        public virtual DbSet<PortalMenuMaster> PortalMenuMasters { get; set; }
        public virtual DbSet<UserManagement.PortalAccess> PortalAccesses { get; set; }
        public virtual DbSet<UploadInvoiceDetails> UploadInvoiceDetailses { get; set; }
        public virtual DbSet<NewReportModel> NewReportModels { get; set; }
        public virtual DbSet<MainStreamModel> MainStreamModels { get; set; }
        public virtual DbSet<RawDataStream> RawDataStreams { get; set; }
        public virtual DbSet<StageMaster> StageMasters { get; set; }
        public virtual DbSet<StreamMaster> StreamMasters { get; set; }
        public virtual DbSet<ConfigurationMaster> ConfigurationMasters { get; set; }
        public virtual DbSet<MainStreamMaster> MainStreamMasters { get; set; }
        public virtual DbSet<QlikDataAccess> QlikDataAccesses { get; set; }

        public virtual DbSet<SubsidryModel> SubsidryModels { get; set; }
        public virtual DbSet<SubsidryUserMapping> SubsidryUserMappings { get; set; }
        public virtual DbSet<SourceMaster> SourceMasters { get; set; }
        public virtual DbSet<SourceDataMapping> SourceDataMappings { get; set; }
        public virtual DbSet<RawDataLink> RawDataLinks { get; set; }
        public virtual DbSet<ObjectMaster> ObjectMasters { get; set; }
        public virtual DbSet<ObjectMapping> ObjectMappings { get; set; }
        public virtual DbSet<SubsidryInvoiceAttachment> SubsidryInvoiceAttachments { get; set; }
        public virtual DbSet<ReportCreteriaModel> ReportCreterias { get; set; }
        public virtual DbSet<MenuSubMenuModel> MenuSubMenuModels { get; set; }
        public virtual DbSet<MenuSubMenuAccessModel> MenuSubMenuAccessModels { get; set; }
        public virtual DbSet<SqlQueryValidationModel> SqlQueryValidations { get; set; }
        public virtual DbSet<VATRequestUpdate> VATRequestUpdates { get; set; }
        public virtual DbSet<ErrorLogModel> ErrorLogModels { get; set; }
        public virtual DbSet<VATReportMapping> VATReportMappings { get; set; }
        public virtual DbSet<PeriodMaster> PeriodMasters { get; set; }
    }
}
