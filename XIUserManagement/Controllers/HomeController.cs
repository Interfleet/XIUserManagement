using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using XIUserManagement.Models;

namespace XIUserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LoginViewModel _users;

        public HomeController(ILogger<HomeController> logger, LoginViewModel users)
        {
            _logger = logger;
            _users = users;
        }

        public IActionResult Index()
        {
            _users.UserName = HttpContext.Session.GetString(UserMessageConstants.searchValueOption1);
            if (_users.UserName != null)
                return View("Index", _users);
            else
                return View("../Login/Index", _users);
            
        }

        public IActionResult Privacy()
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
