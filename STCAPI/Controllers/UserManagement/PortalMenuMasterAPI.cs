using ExcelDataReader;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STAAPI.Infrastructure.Repository.PortalAccessRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.Core.ViewModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class PortalMenuMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<PortalMenuMaster, int> _IPortalMenuRepository;
        private readonly IGenericRepository<PortalAccess, int> _IPortalAccessRepository;
        private readonly IPortalAccessRepository _IPortalMenuAccessRepository;

        /// <summary>
        /// Constructor for Portal Master API to inject the services
        /// </summary>
        /// <param name="portalMenuReposiory"></param>
        /// <param name="portalAcessRepo"></param>
        public PortalMenuMasterAPI(IGenericRepository<PortalMenuMaster, int> portalMenuReposiory,
            IGenericRepository<PortalAccess, int> portalAcessRepo, IPortalAccessRepository portalMenuAccessRepository)
        {
            _IPortalMenuRepository = portalMenuReposiory;
            _IPortalAccessRepository = portalAcessRepo;
            _IPortalMenuAccessRepository = portalMenuAccessRepository;
        }

        /// <summary>
        /// Add the User Menu detail
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        
        [HttpPost]
        public async Task<IActionResult> CreatePortalMenu([FromForm] PortalMenuMaster formFile)
        {
            var models = await GetPortalMenuList(formFile.PortalFile);
            var response = await _IPortalMenuRepository.CreateEntity(models.ToArray());
            return Ok(response.ResponseStatus);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccess(string userName)
        {
            var portalAccessModels = await _IPortalMenuAccessRepository.GetPortalAccessDetail();

            var userAccessPortalModels = await _IPortalAccessRepository.
                GetAllEntities(x => x.IsActive && !x.IsDeleted && x.UserName == userName);

            portalAccessModels.ToList().ForEach(data =>
            {
                userAccessPortalModels.TEntities.ToList().ForEach(item =>
                {
                    if (data.Id == item.PortalId)
                    {
                        data.Flag = true;
                    }
                });

            });

            return Ok(portalAccessModels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserRight(List<PortalAccess> portalAccesses)
        {
            portalAccesses.ForEach(data =>
            {
                data.CreatedBy = "Bhavesh Deepak";
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                data.IsDeleted = false;
            });

            var deleteModel = await _IPortalAccessRepository.GetAllEntities(x => x.UserName.ToUpper().Trim() ==
             portalAccesses.First().UserName.Trim().ToUpper());

            deleteModel.TEntities.ToList().ForEach(x => { 
                x.IsActive = false;
                x.IsDeleted = true;
            });

            var deleteResponse= await _IPortalAccessRepository.DeleteEntity(deleteModel.TEntities.ToArray());

            var response = await _IPortalAccessRepository.CreateEntity(portalAccesses.ToArray());

            return Ok(response.Message);

        }


        /// <summary>
        /// Get the User Access right based on user name detail
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private async Task<List<PortalMenuMaster>> GetPortalMenuList(IFormFile inputFile)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            string message = string.Empty;
            Stream stream = inputFile.OpenReadStream();
            List<PortalMenuMaster> models = new List<PortalMenuMaster>();

            try
            {
                if (inputFile != null)
                {
                    if (inputFile.FileName.EndsWith(".xls"))
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (inputFile.FileName.EndsWith(".xlsx"))
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        message = "The file format is not supported.";

                    dsexcelRecords = reader.AsDataSet();
                    reader.Close();

                    if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                    {
                        DataTable inputVatInvoiceDetail = dsexcelRecords.Tables[0];

                        for (int i = 2; i < inputVatInvoiceDetail.Rows.Count; i++)
                        {
                            var model = new PortalMenuMaster();
                            model.Stage = Convert.ToString(inputVatInvoiceDetail.Rows[i][0]);
                            model.MainStream = Convert.ToString(inputVatInvoiceDetail.Rows[i][1]);
                            model.StreamLongName = Convert.ToString(inputVatInvoiceDetail.Rows[i][2]);
                            model.Stream = Convert.ToString(inputVatInvoiceDetail.Rows[i][3]);
                            model.ObjectName = Convert.ToString(inputVatInvoiceDetail.Rows[i][4]);
                            model.Name = Convert.ToString(inputVatInvoiceDetail.Rows[i][5]);
                            model.Url = Convert.ToString(inputVatInvoiceDetail.Rows[i][6]);
                            model.Flag = false;
                            model.CreatedBy = "Bhavesh";
                            model.IsActive = true;
                            model.IsDeleted = false;
                            model.CreatedDate = DateTime.Now;

                            models.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }

            return await Task.Run(() => models);
        }
    }
}
