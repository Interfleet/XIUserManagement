using Microsoft.AspNetCore.Mvc;
using Interfleet.XIUserManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Services;


namespace Interfleet.XIUserManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly XaltAuthenticationAPI.Services.AuthenticationService _authenticationService;
        private readonly UserService _userService;
        public LoginController(ILogger<LoginController> logger, XaltAuthenticationAPI.Services.AuthenticationService authenticationService, UserService userService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userService = userService;
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
                if (user.UserName != null && user.Password != null && (user.ErrorMessage == "" || user.ErrorMessage == null))
                {
                    var result = _authenticationService.Authenticate(user);

                    if (result != null)
                    {
                        HttpContext.Session.SetString(UserMessageConstants.searchValueOption1, user.UserName);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    user.ErrorMessage = UserMessageConstants.userAccountDisabledMessage;
                    return View("Index", user);
                }
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View("Index", user);
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
        [HttpGet]
        public IActionResult Reset()
        {
            try
            {
                return View("Reset");
            }
            catch (Exception ex)
            {

                return View(ex.Message);
            }
        }
        public IActionResult ResetPassword(string userName)
        {
            _ = new Users();
            ResetPasswordModel? resetPasswordModel = new();
            Users? user = _userService.GetUserByUserName(userName);
            try
            {
                if (user == null)
                {
                    ModelState.AddModelError("UserName",UserMessageConstants.userNotFoundMessage);
                    return View("Reset", user);
                }
                user = _userService.GetUserByUserName(userName);
                resetPasswordModel.UserId = user != null ? user.UserId : 0;
                return View("ResetPassword", resetPasswordModel);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
        public IActionResult Submit(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                if (resetPasswordModel != null && resetPasswordModel.UserId == 0)
                {
                    resetPasswordModel.ErrorMessage = UserMessageConstants.userNotFoundMessage;
                    return RedirectToAction("Login");
                }
                if (!ModelState.IsValid)
                {
                    return View("ResetPassword", resetPasswordModel);
                }
                if (resetPasswordModel != null)
                {
                    bool result = _userService.ResetPassword(resetPasswordModel);
                    resetPasswordModel.SuccessMessage = UserMessageConstants.passwordResetMessage;
                    return View("ResetPassword", resetPasswordModel);
                }

                return View("ResetPassword", resetPasswordModel);
            }
            catch (Exception ex)
            {
                resetPasswordModel.ErrorMessage = ex.Message;
                return View(resetPasswordModel);
            }
        }

    }
}
