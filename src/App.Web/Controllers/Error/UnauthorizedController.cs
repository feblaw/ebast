using Microsoft.AspNetCore.Mvc;

namespace App.Web.Controllers.Error
{
    public class UnauthorizedController : Controller
    {
        public IActionResult Index()
        {
            return View("/Views/Error/Unauthorized.cshtml");
        }
    }
}
