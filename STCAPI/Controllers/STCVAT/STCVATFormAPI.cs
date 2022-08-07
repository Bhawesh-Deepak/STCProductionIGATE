using AutoMapper;
using CommonHelper;
using MailHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Common;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.STCVAT;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STCAPI.Controllers.STCVAT
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class STCVATFormAPI : ControllerBase
    {
        private readonly IMapper _IMapper;
        private readonly ISTACVATFormRepository _ISTCVATFormRepository;
        private readonly IHostingEnvironment _IhostingEnviroment;
        private readonly INotificationService _InotificationService;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;
        /// <summary>
        /// Inject required service controller constructor
        /// </summary>
        /// <param name="_mapper"></param>
        /// <param name="STCVARFormRepository"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="notificationService"></param>
        public STCVATFormAPI(IMapper _mapper, ISTACVATFormRepository STCVARFormRepository,
            IHostingEnvironment hostingEnvironment,
            INotificationService notificationService, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IMapper = _mapper;
            _ISTCVATFormRepository = STCVARFormRepository;
            _IhostingEnviroment = hostingEnvironment;
            _InotificationService = notificationService;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Form Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateForm(STCVATFormModel model)
        {
            try
            {
                List<STCVATForm> models = new List<STCVATForm>();

                foreach (var data in model.HeaderKeyIds)
                {
                    var formModel = new STCVATForm()
                    {
                        CreatedBy = model.UserName,
                        HeaderLineKey = data,
                        ImagePath = model.ImagePath,
                        ReconcileApprove = model.ReconcileApprove,
                        SupplierInvoiceNumber = model.SupplierInvoiceNumber,
                        TaxClassificationCode = model.TaxClassificationCode,
                        TaxCode = model.TaxCode
                    };
                    models.Add(formModel);
                }

                var response = await _ISTCVATFormRepository.CreateEntity(models.ToArray());
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATFormAPI),
                    nameof(CreateForm), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin !");
            }

        }

        /// <summary>
        /// Create Form with Attachment 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateFormWithAttachment([FromForm] STCVATFormModel model)
        {
            try
            {
                var emailResponse = await SendEmailNotification(model);
                model.IsEmaiSend = emailResponse;
                var response = await InsertAdjustmentFormData(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATFormAPI),
                        nameof(CreateFormWithAttachment), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin !");
            }

        }

        /// <summary>
        /// Get Detail with Attachments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDetailWithAttachment()
        {
            try
            {
                string url = HttpContext.Request.Host.Value;

                var response = await _ISTCVATFormRepository.GetAllEntities(x => x.IsActive == true && x.IsDeleted == false);
                foreach (var data in response.TEntities)
                {
                    List<string> ImagesPaths = new List<string>();
                    foreach (var item in data.ImagePath.Split("♥"))
                    {
                        ImagesPaths.Add($"http://{url}/{item}");
                    }
                    data.ImagesUrl = ImagesPaths;
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATFormAPI),
                            nameof(GetDetailWithAttachment), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin !");
            }

        }

        #region PrivateMethod
        /// <summary>
        /// Send Email Notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<bool> SendEmailNotification(STCVATFormModel model)
        {
            try
            {
                var emailResponse = await _InotificationService
            .SendMailNotification(model.EmailTo, model.EmailTemplate, model.EmailSubject);
                return emailResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// InsertAdjustmentFormData
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ResponseModel<STCVATForm, int>> InsertAdjustmentFormData(STCVATFormModel model)
        {
            try
            {
                List<STCVATForm> models = new List<STCVATForm>();

                var documentPaths = await new BlobHelper().UploadDocument(model.Images, _IhostingEnviroment);

                foreach (var data in model.HeaderKeyIds)
                {
                    var formModel = new STCVATForm()
                    {
                        CreatedBy = model.UserName,
                        HeaderLineKey = data,
                        ImagePath = string.Join("♥", documentPaths),
                        ReconcileApprove = model.ReconcileApprove,
                        SupplierInvoiceNumber = model.SupplierInvoiceNumber,
                        TaxClassificationCode = model.TaxClassificationCode,
                        TaxCode = model.TaxCode,
                        EmailTo = string.Join(";", model.EmailTo),
                        Comments = model.Comments,
                        IsEmailSend = model.IsEmaiSend
                    };
                    models.Add(formModel);
                }
                return await _ISTCVATFormRepository.CreateEntity(models.ToArray());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion
    }
}
