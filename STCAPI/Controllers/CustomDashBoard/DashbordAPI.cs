using MailHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STCAPI.Core.ViewModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.CustomDashBoard
{
    //{
    //    [Route("api/[controller]/[action]")]
    //    [EnableCors("AllowAnyOrigin")]
    //    [ApiController]
    //    public class DashbordAPI : ControllerBase
    //    {
    //        private readonly INotificationService _InotificationService;

    //        public DashbordAPI(INotificationService inotificationService)
    //        {
    //            _InotificationService = inotificationService;
    //        }

    //        //[HttpPost]
    //        //public async Task<IActionResult> SendEmails([FromForm] CustomDeashbordVm model) 
    //        //{
    //        //    var response = await _InotificationService.SendMailWithAttachment(model.EmailIds, model.Category, model.Property, model.Attachment);
    //        //    return Ok(response);
    //        //}
    //    }
}
