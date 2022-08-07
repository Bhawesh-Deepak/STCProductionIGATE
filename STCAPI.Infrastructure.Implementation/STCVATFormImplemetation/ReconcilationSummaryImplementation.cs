using Microsoft.Extensions.Configuration;
using STAAPI.Infrastructure.Repository.STCVATRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Context;
using STCAPI.Core.Entities.Reconcilation;
using System;
using System.Threading.Tasks;

namespace STCAPI.Infrastructure.Implementation.STCVATFormImplemetation
{
    public class ReconcilationSummaryImplementation : IReconcilationSummaryRepository
    {
        private readonly STCContext context;

        public ReconcilationSummaryImplementation(IConfiguration Configuration)
        {
            context = new STCContext(Configuration);
        }

        public async Task<ResponseModel<RecincilationSummary, int>> CreateEntity(RecincilationSummary[] model)
        {
            try
            {
                foreach (var data in model)
                {
                    await context.RecincilationSummaries.AddAsync(data);
                }
                await context.SaveChangesAsync();

                return new ResponseModel<RecincilationSummary, int>(null, null, "Created", ResponseStatus.Created);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException, ex);
            }
        }

        public Task<ResponseModel<RecincilationSummary, int>> GetAllEntities(Func<RecincilationSummary, bool> where)
        {
            throw new NotImplementedException();
        }
    }
}
