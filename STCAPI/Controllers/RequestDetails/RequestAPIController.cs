using CommonHelper;
using MailHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.RequestDetail;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STCAPI.Controllers.RequestDetails
{
    /// <summary>
    /// Request Api Controller 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class RequestAPIController : ControllerBase
    {
        private readonly IGenericRepository<RequestDetailModel, int> _IRequestRepository;
        private readonly IHostingEnvironment _IhostingEnviroment;
        private readonly INotificationService _INotificationService;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service controller
        /// </summary>
        /// <param name="_requestRepository"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="notificationService"></param>
        /// <param name="errorLogRepository"></param>
        public RequestAPIController(IGenericRepository<RequestDetailModel, int> _requestRepository,
            IHostingEnvironment hostingEnvironment,
            INotificationService notificationService, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _IRequestRepository = _requestRepository;
            _IhostingEnviroment = hostingEnvironment;
            _INotificationService = notificationService;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create new request 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewRequest([FromForm] RequestDetailModel model)
        {
            try
            {
                var documentPaths = await new BlobHelper().UploadDocument(model.AttachmentDetails, _IhostingEnviroment);
                model.Attachments = string.Join("♥", documentPaths);
                model.CreatedDate = DateTime.Now;
                model.IsActive = true;

                var response = await _IRequestRepository.CreateEntity(new List<RequestDetailModel>() { model }.ToArray());
                if (response.ResponseStatus == Core.Entities.Common.ResponseStatus.Error)
                {

                    return BadRequest("Something wents wrong Please contact admin Team");
                }
                var attachmentModel = new EmailAttachmentDetails()
                {
                    Attachments = model.AttachmentDetails,
                    Category = model.Category,
                    Priority = model.Priority,
                    Description = model.Description,
                    ToEmailIds = model.Emails

                };

                var emailResponse = await _INotificationService.SendMailWithAttachment(attachmentModel);
                return Ok("New request has been created.");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RequestAPIController),
                     nameof(CreateNewRequest), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }

        /// <summary>
        ///  Get New Request compelete Details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNewRequestDetails()
        {
            try
            {
                var responseData = await _IRequestRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
                if (responseData.ResponseStatus != Core.Entities.Common.ResponseStatus.Error)
                {
                    return Ok(responseData.TEntities);
                }
                return BadRequest("Something wents wrong Please contact admin Team");
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(RequestAPIController),
                        nameof(GetNewRequestDetails), ex.Message, ex.ToString());

                return BadRequest("Something wents wrong Please contact admin Team");
            }
        }
    }
}
