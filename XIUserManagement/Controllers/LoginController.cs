using Microsoft.AspNetCore.Mvc;
using Interfleet.XIUserManagement.Models;
using Interfleet.XaltAuthenticationAPI.Services;
using Microsoft.AspNetCore.Authentication;


namespace Interfleet.XIUserManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly XaltAuthenticationAPI.Services.AuthenticationService _authenticationService;
        public LoginController(ILogger<LoginController> logger, XaltAuthenticationAPI.Services.AuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("Index", new LoginViewModel());
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel user)
        {
            try
            {
                if (user.UserName != null && user.Password != null)
                {
                    var result = _authenticationService.Authenticate(user);

                    if (result != null)
                    {
                        HttpContext.Session.SetString("username", user.UserName);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    user.ErrorMessage = "Invalid User!";
                    return View("Index", user);
                }
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View("Index",user);
            }
            return View("Index");
        }
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}
