using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using App.Controllers;
using App.Web.Models;

namespace App.Controllers
{
    public class alienViewController : Controller
    {       
        private ILogger<alienViewController> _logger;
		private IConfiguration Configuration { get; set; }
		
        /// <summary>
        /// Default constructure for dependency injection
        /// </summary>
		/// <param name="configuration"></param>
        /// <param name="logger"></param>
        public alienViewController(ILogger<alienViewController> logger, IConfiguration configuration)
        {
            _logger = logger;
			Configuration = configuration;
        }

        public IActionResult alien()
        {
			//if (!MyHelper.checkAuth(Configuration, HttpContext)) return Unauthorized(); // Or UnauthorizedView
            return View();
        }

        public IActionResult alien_d()
        {
			//if (!MyHelper.checkAuth(Configuration, HttpContext)) return Unauthorized(); // Or UnauthorizedView
            return View();
        }

		//public IActionResult alien_report()
        //{
		//    if (!MyHelper.checkAuth(Configuration, HttpContext)) return Unauthorized(); // Or UnauthorizedView
        //    return View();
        //}

        //public IActionResult alien_inline()
        //{
		//    if (!MyHelper.checkAuth(Configuration, HttpContext)) return Unauthorized(); // Or UnauthorizedView
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


