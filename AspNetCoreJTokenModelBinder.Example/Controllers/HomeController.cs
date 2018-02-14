using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreDynamicModelBinder.Example.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace AspNetCoreJTokenModelBinder.Example.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
 

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ValidateAntiForgeryToken]
        public IActionResult FormReciever(JToken content)
        {
            return Content(content.ToString(), "text/html");
        }
        public IActionResult ApplicationJsonReciever(JToken content)
        {
            return Content(content.ToString(), "text/html");
        }

    }



}
