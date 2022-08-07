using Microsoft.Extensions.Configuration;
using STAAPI.Infrastructure.Repository.STCVATRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Context;
using STCAPI.Core.Entities.STCVAT;
using System;
using System.Threading.Tasks;

namespace STCAPI.Infrastructure.Implementation.STCVATFormImplemetation
{
    public class STCPostValidationDetail : ISTCPOstValidationRepository
    {
        private STCContext context;
        public STCPostValidationDetail(IConfiguration Configuration)
        {
            context = new STCContext(Configuration);
        }
        public async Task<ResponseModel<STCPostValidation, int>> CreateEntity(STCPostValidation[] model)
        {
            try
            {
                foreach (var data in model)
                {
                    //if (context.STCPostValidations.Where(x => x.HeaderLineKey.Trim().ToLower() == data.HeaderLineKey.ToLower().Trim()).Any())
                    //{
                    //    var updateModel = context.STCPostValidations.Where(x => x.HeaderLineKey == data.HeaderLineKey).FirstOrDefault();
                    //    updateModel.PostValidation = data.PostValidation;
                    //    context.STCPostValidations.Update(updateModel);

                    //}
                    //else
                    //{


                    //}
                    await context.STCPostValidations.AddAsync(data);
                }
                await context.SaveChangesAsync();

                return new ResponseModel<STCPostValidation, int>(null, null, "Created", ResponseStatus.Created);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException, ex);
            }
        }

        public Task<ResponseModel<STCPostValidation, int>> GetAllEntities(Func<STCPostValidation, bool> where)
        {
            throw new NotImplementedException();
        }
    }
}
