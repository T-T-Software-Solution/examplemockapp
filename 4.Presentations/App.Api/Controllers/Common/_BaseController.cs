using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Api
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Default constructure for dependency injection
        /// </summary>
        public BaseController()
        {

        }

        protected string GetClientRootUrl()
        {
            var request = HttpContext.Request;

            return $"{request.Scheme}://{request.Host}";
        }

        protected string GetClientRequestUrl()
        {
            var request = HttpContext.Request;

            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }

        protected string GetClientRootUrlByOrigin()
        {
            var nameFilter = string.Empty;

            //Get Origin header for full root path of the client
            if (Request.Headers.TryGetValue("Origin", out StringValues headerValues))
                nameFilter = headerValues.FirstOrDefault();

            return nameFilter;
        }

        //protected async Task<IActionResult> ReturnFileDownload(string relativePath)
        //{
        //    var fullPath = FileUtil.GetAbsolutePath(relativePath);

        //    if (!FileUtil.DoesFileExist(fullPath))
        //        return Content("filename not present");

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(fullPath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;

        //    return File(memory, FileUtil.GetMineType(fullPath), Path.GetFileName(fullPath));
        //}

        protected async Task<string> GetAccessToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
