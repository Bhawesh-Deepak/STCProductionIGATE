using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class BlobHelper
    {
        public async Task<List<string>> UploadDocument(List<IFormFile> document, IHostingEnvironment _hostingEnvironment)
        {
            var guidId = Guid.NewGuid();
            List<string> _documentPaths = new List<string>();
            if (document != null && document.Any())
            {
                document.ForEach(item =>
                {
                    var upload = Path.Combine(_hostingEnvironment.WebRootPath, "Files//");
                    using (var fs = new FileStream(Path.Combine(upload, guidId.ToString() + "_" + item.FileName), FileMode.Create))
                    {
                        item.CopyTo(fs);
                    }

                    _documentPaths.Add("/Files/" + guidId.ToString() + "_" + item.FileName);
                });
            }
            return await Task.Run(() => _documentPaths);
        }


        public async Task<string> UploadDocument(IFormFile document, IHostingEnvironment _hostingEnvironment)
        {
            if (document != null && document.Length > 0)
            {
                var guidId = Guid.NewGuid();
                var upload = Path.Combine(_hostingEnvironment.WebRootPath, "Files//");
                using (var fs = new FileStream(Path.Combine(upload, guidId.ToString() + "_" + document.FileName), FileMode.Create))
                {
                    document.CopyTo(fs);
                }

                return await Task.Run(() => "/Files/" + guidId.ToString() + "_" + document.FileName);
            }
            return await Task.Run(() => string.Empty);

        }
    }
}
