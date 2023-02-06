using Authorization.Interfaces;
using Authorization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHome _home;
        public IConfiguration Configuration { get; }        
        public HomeController(ILogger<HomeController> logger, IHome home, IConfiguration configuration)
        {
            _logger = logger;
            _home = home;
            Configuration = configuration;            
        }

        public IActionResult Index(string errorPage, string successPage)
        {            
            if(successPage!=null)
            {
                HttpContext.Session.SetString("errorPage", errorPage);
                HttpContext.Session.SetString("successPage", successPage);
            }
                     
            return View();
        }
        public IActionResult Login()
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var errpage = jwtAppSettingOptions["errorCallback"];
            var sucpage = jwtAppSettingOptions["successCallback"];

            var authURL = Configuration.GetSection("URL")["AuthUrl"] + "?errorPage=" 
                + errpage + "&successPage=" + sucpage;
            //var authURL = Configuration.GetSection("URL")["AuthUrl"] + "?errorPage="
            //    + HttpContext.Session.GetString("errorPage")
            //    + "&successPage=" + HttpContext.Session.GetString("successPage");

            return Redirect(authURL);
        }
       
        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Failed()
        {
            return View();
        }

        public IActionResult LoginClient(string token)
        {
            var isValid = _home.LoginClient(token);
            if (isValid.Result == true)
            {
                return RedirectToAction("Success", "Home");               
            }
            else
            {
                return RedirectToAction("Failed", "Home");
            }            
        }
        [HttpPost]
        public IActionResult Index(string token)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
