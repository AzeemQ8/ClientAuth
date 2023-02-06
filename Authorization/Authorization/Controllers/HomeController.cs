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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login(string errorPage, string successPage)
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var errpage = jwtAppSettingOptions["errorCallback"];
            var sucpage = jwtAppSettingOptions["successCallback"];
            HttpContext.Session.SetString("errorPage1", errorPage);
            HttpContext.Session.SetString("successPage1", successPage);
            var currentUrl = HttpContext.Request.Host + HttpContext.Request.Path;
            HttpContext.Session.SetString("Url", currentUrl);
            if (errorPage == errpage && successPage == sucpage)
            {
                return View(new LoginDTO());
            }
            return Redirect(currentUrl);
            
        }

        [HttpPost]
        public IActionResult LoginClick(string userName, string password)
        {
            var token = _home.LoginToken(userName, password);
            //var urls = Configuration.GetSection("URL")["ClientUrl"].ToString() + "?token=" + token.Result;
            var url = HttpContext.Session.GetString("errorPage1");
            var status = _home.Login(userName, password);
            if (status.Result == "true")
            {
                url = HttpContext.Session.GetString("successPage1");
            }
            return Redirect(url);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
