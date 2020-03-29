using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FileIO = System.IO.File;
using System;
using App.Domain.Models.Core;
using App.Helper;
using System.Linq;
using System.Collections.Generic;

namespace App.Web.Areas.Admin.Controllers.Api
{
    [Produces("application/json")]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly FileHelper _fileHelper;
        private const string _uploadDir = "uploads/temp";

        public FileController(IHostingEnvironment env,
            FileHelper fileHelper)
        {
            _env = env;
            _fileHelper = fileHelper;
        }

        [Obsolete]
        [HttpPost]
        public IActionResult Upload(IFormFile filedata)
        {
            var upload = Path.Combine(_env.WebRootPath, _uploadDir);
            if (!Directory.Exists(upload))
            {
                Directory.CreateDirectory(upload);
            }

            if (filedata.Length > 0)
            {
                var prefix = Guid.NewGuid().ToString("n") + "_";
                var filepath = Path.Combine(upload, prefix + filedata.FileName);
                var imageSrc = $"{_uploadDir}/{prefix}{filedata.FileName}";
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    filedata.CopyTo(filestream);
                }

                var result = new Attachment()
                {
                    Name = filedata.FileName,
                    FileType = _fileHelper.GetMimeTypeByName(filedata.FileName),
                    Type = Attachment.FILE_TYPE_UPLOAD,
                    Path = imageSrc,
                    Size = filedata.Length / 1024
                };


                return Json(new
                {
                    data = result
                });
            }

            return Json(new BadRequestResult());
        }

        [HttpPost]
        public IActionResult PlUpload()
        {
            return DoPlUpload(Request, _env.WebRootPath, _uploadDir);
        }

        public static IActionResult DoPlUpload(HttpRequest request, string webRootPath, string uploadDir, Func<Attachment, bool> update = null)
        {
            var uploadBase = Path.Combine(webRootPath, uploadDir);
            if (!Directory.Exists(uploadBase))
                Directory.CreateDirectory(uploadBase);

            //using plUploadQueue frontside, actual filename is put into form data
            // (also, with plUploadQueue this method would be called for each enqueued files anyway)
            // so we can't actually process multiple files here
            //var data = new List<Attachment>();

            foreach (var filedata in request.Form?.Files ?? Enumerable.Empty<IFormFile>())
            {
                if (filedata.Length > 0)
                {
                    var prefix = $"{Guid.NewGuid().ToString("n")}_";
                    var filename = filedata.FileName;
                    if (request.Form?.ContainsKey("name") == true) filename = request.Form["name"];
                    var filepath = Path.Combine(uploadBase, $"{prefix}{filename}");
                    var imageSrc = $"{uploadDir}/{prefix}{filename}";
                    using (var filestream = new FileStream(filepath, FileMode.Create))
                    {
                        filedata.CopyTo(filestream);
                    }

                    var result = new Attachment()
                    {
                        Name = filename,
                        FileType = FileHelper.DoGetMimeTypeByName(filename),
                        Type = Attachment.FILE_TYPE_UPLOAD,
                        Path = imageSrc,
                        Size = filedata.Length / 1024
                    };

                    //data.Add(result);

                    if (update == null || update(result))
                        return new JsonResult(new { data = result });
                    else break;
                }
            }

            //if (data.Any())
            //    return Json(new { data = data });

            return new JsonResult(new BadRequestResult());
        }

        [HttpPost]
        public IActionResult Image(IFormFile filedata)
        {
            var upload = Path.Combine(_env.WebRootPath, _uploadDir);
            if (!Directory.Exists(upload))
            {
                Directory.CreateDirectory(upload);
            }

            if (filedata.Length > 0)
            {
                var prefix = Guid.NewGuid().ToString("n");
                var filepath = Path.Combine(upload, prefix + filedata.FileName);
                var imageSrc = $"{_uploadDir}/{prefix}{filedata.FileName}";
                using (var filestream = new FileStream(filepath, FileMode.Create))
                {
                    filedata.CopyTo(filestream);
                }

                var result = new Attachment()
                {
                    Name = filedata.FileName,
                    FileType = _fileHelper.GetMimeTypeByName(filedata.FileName),
                    Type = Attachment.FILE_TYPE_UPLOAD,
                    Path = imageSrc,
                    Size = filedata.Length / 1024
                };


                return Json(new
                {
                    data = result
                });
            }

            return Json(new BadRequestResult());
        }

     
        [HttpGet]
        [HttpPost]
        public IActionResult Delete()
        {
            var upload = Path.Combine(_env.WebRootPath, _uploadDir);
            var listFile = Directory.GetFiles(upload);
            if (listFile.Length != 0 && Directory.Exists(upload))
            {
                foreach (var item in listFile)
                {
                    if ((DateTime.Now.Subtract(FileIO.GetCreationTime(item)).TotalDays > 1 ? true : false))
                    {
                        FileIO.Delete(item);
                    }
                }
            }
            return Json(new OkResult());
        }
    }
}