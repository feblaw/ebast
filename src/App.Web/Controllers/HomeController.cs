using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace App.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if(User.IsInRole("Contractor"))
            {
                return RedirectToAction("Index", "Profile", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
        }
    }
}
