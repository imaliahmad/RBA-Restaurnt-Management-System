using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserInterface.Controllers
{
    public class ErrorController : Controller
    {
        [Route("ErrorLog/{statusCode}")]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeErrorHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            //404 --> NotFound, 500 --> Internal Server Error, 401 --> Unauthorized
            switch (statusCode)
            {
                case 404:
                case 405:
                    {
                        ViewBag.IsSuccess = false;
                        ViewBag.ErrorCode = statusCode;
                        ViewBag.Message = "Sorry, the resource you requested could not be found.";
                        ViewBag.Description = $"Module: UI, {"\n"} ErrorPath: {statusCodeResult.OriginalPath ?? ""}\n, QueryString: {statusCodeResult.OriginalQueryString ?? ""}";

                        return View("_NotFound");
                    }
                case 401:
                    {
                        ViewBag.IsSuccess = false;
                        ViewBag.ErrorCode = statusCode;
                        ViewBag.Message = "Sorry, you are not authorized to access this page. Please contact to your admin.";
                        ViewBag.Description = $"Module: UI, {"\n"} ErrorPath: {statusCodeResult.OriginalPath ?? ""}\n, QueryString: {statusCodeResult.OriginalQueryString ?? ""}";

                        return View("UnAuthorized");
                    }
            }

            return View();
        }


        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var statusCodeResult = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            

            ViewBag.IsSuccess = false;
            ViewBag.ErrorCode = 500;
            ViewBag.Message = statusCodeResult.Error.InnerException != null ? statusCodeResult.Error.InnerException.Message : statusCodeResult.Error.Message;
            ViewBag.Description = $"Module: UI, {"\n"} StackTrace: {statusCodeResult.Error.StackTrace ?? ""}\n";
           
            
            return View();
        }
    }
}
