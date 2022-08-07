using MailHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STAAPI.Infrastructure.Repository.STCVATRepository;
using STCAPI.Core.Entities.Logger;
using STCAPI.Core.Entities.STCVAT;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.ErrorLogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.STCVAT
{
    /// <summary>
    /// STCVATPostValidationApi
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAnyOrigin")]
    public class STCVATPostValidationAPI : ControllerBase
    {
        private readonly ISTCPOstValidationRepository _iSTCPOstValidationRepository;
        private readonly INotificationService _InotificationService;
        private readonly IGenericRepository<ErrorLogModel, int> _IErrorLogRepository;

        /// <summary>
        /// Inject required service to controller constructor
        /// </summary>
        /// <param name="sTCPOstValidationRepository"></param>
        /// <param name="notificationService"></param>
        /// <param name="errorLogRepository"></param>
        public STCVATPostValidationAPI(ISTCPOstValidationRepository sTCPOstValidationRepository,
            INotificationService notificationService, IGenericRepository<ErrorLogModel, int> errorLogRepository)
        {
            _iSTCPOstValidationRepository = sTCPOstValidationRepository;
            _InotificationService = notificationService;
            _IErrorLogRepository = errorLogRepository;
        }

        /// <summary>
        /// Create Form
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateForm(STCPostValidationModel model)
        {
            try
            {
                List<STCPostValidation> models = new List<STCPostValidation>();
                foreach (var data in model.HeaderKey)
                {
                    var formModel = new STCPostValidation()
                    {
                        CreatedBy = model.UserName,
                        HeaderLineKey = data,
                        PostValidation = model.PostValidation,
                        CreatedDate = DateTime.Now,
                        EmailId = model.EmailId,
                        Comment = model.Comment
                    };
                    models.Add(formModel);
                }

                //Code to send the email  when the post validation type is email
                var response = await _iSTCPOstValidationRepository.CreateEntity(models.ToArray());
                if (model.IsEmailSend)
                {
                    var emailIds = model.EmailId.Split(";").ToList();
                    _ = await _InotificationService.SendMailNotification(emailIds, model.EmailTemplate, model.EmailSubject);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                await ErrorLogServiceImplementation.LogError(_IErrorLogRepository, nameof(STCVATPostValidationAPI),
                    nameof(CreateForm), ex.Message, ex.ToString());
                return BadRequest("Something wents wrong, Please contact admin Team !");
            }

        }


    }
}
