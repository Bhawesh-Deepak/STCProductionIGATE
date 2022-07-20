using CommonHelper;
using MailHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.STCVATRepository;
using STCAPI.Core.Entities.Reconcilation;
using STCAPI.Core.ViewModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.Reconcilation
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class ReconcilationAPI : ControllerBase
    {
        private readonly IReconcilationSummaryRepository _IReconcilationSummaryRepo;
        private readonly IHostingEnvironment _IHostingEnviroment;
        private readonly INotificationService _InotificationService;

        /// <summary>
        /// Inject required service to constructor
        /// </summary>
        /// <param name="iReconcilationSummaryRepo"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="notificationService"></param>
        public ReconcilationAPI(IReconcilationSummaryRepository iReconcilationSummaryRepo,
            IHostingEnvironment hostingEnvironment, INotificationService notificationService)
        {
            _IReconcilationSummaryRepo = iReconcilationSummaryRepo;
            _IHostingEnviroment = hostingEnvironment;
            _InotificationService = notificationService;
        }

        /// <summary>
        /// create reconcilation model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] ReconcilationModel model)
        {
            try
            {
                var documentPaths = await new BlobHelper().UploadDocument(model.Attachment, _IHostingEnviroment);
                var models = new List<RecincilationSummary>();
                foreach (var data in model.HeaderLineKey)
                {
                    var reconcilationModel = new RecincilationSummary();
                    reconcilationModel.HeaderLineKey = data;
                    reconcilationModel.ReconcilationStatus = model.ReconcilationStatus;
                    reconcilationModel.EmailTo = model.EmailTo;
                    reconcilationModel.AdjustmentValue = model.AdjustmentValue;
                    reconcilationModel.Attachment = string.Join("♥", documentPaths);
                    reconcilationModel.Comments = model.Comments;
                    reconcilationModel.IsEmailSend = model.IsEmailSend;
                    reconcilationModel.CreatedBy = model.CreatedBy;
                    reconcilationModel.CreatedDate = DateTime.Now;
                    reconcilationModel.ImagePath = model.ImagePath;
                    models.Add(reconcilationModel);
                }

                var response = await _IReconcilationSummaryRepo.CreateEntity(models.ToArray());
                if (response.ResponseStatus != Core.Entities.Common.ResponseStatus.Error && model.IsEmailSend)
                {
                    await SendEmailNotification(model);
                }
                return Ok(response);
            }
            catch (Exception ex) {
                string exceptionMessage = ex.Message;
                return BadRequest("Issue Occured, Please contact admin Team !");
            }
        }

        /// <summary>
        /// Send email notification with and without attachment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<bool> SendEmailNotification(ReconcilationModel model)
        {
            var emailResponse = await _InotificationService
                        .SendMailNotification(model.EmailTo.Split(";").ToList(), model.EmailTemplate, model.EmailSubject);
            return emailResponse;
        }
    }
}
