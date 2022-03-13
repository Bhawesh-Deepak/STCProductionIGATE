using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace STCAPI.Controllers.CopyFolders
{
    /// <summary>
    /// Copy folder from one drive to another
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CopyFolderController : ControllerBase
    {
        /// <summary>
        /// Copy Folder from one drive to another
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="destFolder"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FolderCopy(string sourceFolder, string destFolder)
        {
            CopyFolder(sourceFolder, destFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
            return await Task.Run(() => Ok("Folder copying is in progress...."));
        }

        private static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                System.IO.File.Copy(file, dest);
            }
        }
    }
}
