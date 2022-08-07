using Microsoft.Extensions.Configuration;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Context;
using STCAPI.Core.Entities.STCVAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Infrastructure.Implementation.STCVATFormImplemetation
{
    public class STCVATFormDetail : ISTACVATFormRepository
    {
        private STCContext context;
        public STCVATFormDetail(IConfiguration configuration)
        {
            context = new STCContext(configuration);
        }
        public Task<ResponseModel<STCVATForm, int>> CheckIsExists(Func<STCVATForm, bool> where)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<STCVATForm, int>> CreateEntity(STCVATForm[] model)
        {
            try
            {
                foreach (var data in model)
                {
                    await context.STCVATForms.AddAsync(data);

                    #region ObseleteCode Remove as per guide Line
                    //if (context.STCVATForms.Where(x => x.HeaderLineKey.Trim().ToLower() == data.HeaderLineKey.ToLower().Trim()).Any())
                    //{
                    //    var updateModel = context.STCVATForms.Where(x => x.HeaderLineKey == data.HeaderLineKey).FirstOrDefault();
                    //    updateModel.TaxClassificationCode = data.TaxClassificationCode;
                    //    updateModel.TaxCode = data.TaxCode;
                    //    updateModel.SupplierInvoiceNumber = data.SupplierInvoiceNumber;
                    //    updateModel.ReconcileApprove = data.ReconcileApprove;
                    //    updateModel.ImagePath = data.ImagePath;
                    //    updateModel.UpdatedDate = DateTime.Now;
                    //    updateModel.UpdatedBy = data.CreatedBy;

                    //    context.STCVATForms.Update(updateModel);

                    //}
                    //else
                    //{
                    //    await context.STCVATForms.AddAsync(data);

                    //}
                    #endregion
                }
                await context.SaveChangesAsync();

                return new ResponseModel<STCVATForm, int>(null, null, "Created", ResponseStatus.Created);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException, ex);
            }

        }

        public Task<ResponseModel<STCVATForm, int>> DeleteEntity(params STCVATForm[] items)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<STCVATForm, int>> GetAllEntities(Func<STCVATForm, bool> where)
        {
            try
            {
                var models = new List<STCVATForm>();
                models = where != null ? context.STCVATForms.Where(where).ToList() :
                    context.STCVATForms.ToList();
                return await Task.Run(() => new ResponseModel<STCVATForm, int>(null, models, "success", ResponseStatus.Success));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ex.InnerException, ex);
            }
        }

        public Task<ResponseModel<STCVATForm, int>> UpdateEntity(STCVATForm model)
        {
            throw new NotImplementedException();
        }
    }
}
