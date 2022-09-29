using ExcelDataReader;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STAAPI.Infrastructure.Repository.PortalAccessRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.UserManagement;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// PortalMenuMasterAPI
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class PortalMenuMasterAPI : ControllerBase
    {
        private readonly IGenericRepository<PortalMenuMaster, int> _IPortalMenuRepository;
        private readonly IGenericRepository<PortalAccess, int> _IPortalAccessRepository;
        private readonly IPortalAccessRepository _IPortalMenuAccessRepository;
        private readonly IGenericRepository<ObjectMapping, int> _IObjectMappingRepository;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;


        /// <summary>
        /// Constructor for Portal Master API to inject the services
        /// </summary>
        /// <param name="portalMenuReposiory"></param>
        /// <param name="portalAcessRepo"></param>
        public PortalMenuMasterAPI(IGenericRepository<PortalMenuMaster, int> portalMenuReposiory,
            IGenericRepository<PortalAccess, int> portalAcessRepo,
            IGenericRepository<ObjectMapping, int> objectMappingRepo,
            IPortalAccessRepository portalMenuAccessRepository, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IPortalMenuRepository = portalMenuReposiory;
            _IPortalAccessRepository = portalAcessRepo;
            _IPortalMenuAccessRepository = portalMenuAccessRepository;
            _IObjectMappingRepository = objectMappingRepo;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Add the User Menu detail
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreatePortalMenu([FromForm] PortalMenuMaster formFile)
        {
            try
            {
                var models = await GetPortalMenuList(formFile.PortalFile);
                var response = await _IPortalMenuRepository.CreateEntity(models.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(PortalMenuMasterAPI),
                            nameof(CreatePortalMenu), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        /// Get User Access Details
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserAccess(string userName)
        {
            try
            {
                var objectMappingDetails = await _IObjectMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var userAccessPortalModels = await _IPortalAccessRepository.
                    GetAllEntities(x => x.IsActive && !x.IsDeleted && x.UserName == userName);

                objectMappingDetails.TEntities.ToList().ForEach(data =>
                {
                    userAccessPortalModels.TEntities.ToList().ForEach(item =>
                    {
                        if (data.Id == item.PortalId)
                        {
                            data.IsMapped = item.IsMapped;
                        }
                    });

                });

                return Ok(objectMappingDetails);
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(PortalMenuMasterAPI),
                                 nameof(GetUserAccess), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }



        /// <summary>
        /// Get User Access Details
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetAccessRight(string userName, bool isMapped)
        {
            try
            {
                var objectMappingDetails = await _IObjectMappingRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);

                var userAccessPortalModels = await _IPortalAccessRepository.
                    GetAllEntities(x => x.IsActive && !x.IsDeleted && x.UserName.Trim().ToLower() == userName.Trim().ToLower() && x.IsMapped);
    
                var response = (from pa in userAccessPortalModels.TEntities
                                join om in objectMappingDetails.TEntities
                                on pa.PortalId equals om.Id
                                select new ObjectMapping
                                {
                                    Stage = om.Stage,
                                    MainStream = om.MainStream,
                                    Stream = om.Stream,
                                    Object = om.Object,
                                    Name = om.Name,
                                    ShortName = om.ShortName,
                                    LongName = om.LongName,
                                    Description = om.Description,
                                    ObjectNumber = om.ObjectNumber,
                                    ObjectReference = om.ObjectReference,
                                    IsMapped = pa.IsMapped

                                }).ToList();

                return Ok(new ResponseModel<ObjectMapping, int>()
                {
                    Entity = null,
                    Message = "success",
                    ResponseStatus = ResponseStatus.Success,
                    TEntities = response
                });
            }
            catch (Exception ex)
            {

                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(PortalMenuMasterAPI),
                                 nameof(GetUserAccess), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Create User Detail Access
        /// </summary>
        /// <param name="portalAccesses"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUserRight(List<PortalAccess> portalAccesses)
        {
            try
            {
                portalAccesses.ForEach(data =>
                {
                    data.CreatedBy = "Bhavesh Deepak";
                    data.CreatedDate = DateTime.Now;
                    data.IsActive = true;
                    data.IsDeleted = false;
                    data.UpdatedDate = DateTime.Now;
                });

                var deleteModel = await _IPortalAccessRepository.GetAllEntities(x => x.UserName.ToUpper().Trim() ==
                 portalAccesses.First().UserName.Trim().ToUpper());

                deleteModel.TEntities.ToList().ForEach(x =>
                {
                    x.IsActive = false;
                    x.IsDeleted = true;
                });

                var deleteResponse = await _IPortalAccessRepository.DeleteEntity(deleteModel.TEntities.ToArray());

                var response = await _IPortalAccessRepository.CreateEntity(portalAccesses.ToArray());

                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(PortalMenuMasterAPI),
                 nameof(CreateUserRight), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }
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
                throw new Exception(ex.Message, ex);
            }

            return await Task.Run(() => models);
        }
    }
}
