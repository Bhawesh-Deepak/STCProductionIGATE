using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.UserManagement
{
    /// <summary>
    /// Periods Api's Details
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PeriodsAPI : ControllerBase
    {
        /// <summary>
        /// Period Details Api's
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]

        public async Task<IActionResult> GetPeriodsDetails()
        {
            var response =  Enumerable.Range(1, 12).Select(i => new {  periods = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i)+"-"+System.DateTime.Now.Year }).ToList();
            return Ok(await Task.Run(()=> response));
        }

    }
}
