using Microsoft.AspNetCore.Mvc;
using Interfleet.XIUserManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Services;


namespace Interfleet.XIUserManagement.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly XaltAuthenticationAPI.Services.AuthenticationService _authenticationService;
        private readonly UserService _userService;
        public ChangePasswordController(ILogger<LoginController> logger, XaltAuthenticationAPI.Services.AuthenticationService authenticationService, UserService userService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ChangePassword(string userName)
        {
           _ = new Users();
            Users? user = _userService.GetUserByUserName(userName);
            ChangePasswordModel? changePasswordModel = user !=null ? new() : null;
            try
            {
                if (user == null)
                {
                    ModelState.AddModelError("UserName", UserMessageConstants.invalidPwdMessage);
                    return View("ChangePassword", changePasswordModel);
                }
                user = _userService.GetUserByUserName(userName);
                changePasswordModel.UserId = user != null ? user.UserId : 0;
                return View("ChangePassword", changePasswordModel);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
        public IActionResult Submit(ChangePasswordModel changePasswordModel)
        {
            try
            {
                Users? user = _userService.GetUserByUserId(changePasswordModel.UserId);
                if (changePasswordModel != null && changePasswordModel.UserId == 0)
                {
                    changePasswordModel.ErrorMessage = UserMessageConstants.invalidPwdMessage;
                    return RedirectToAction("Index");
                }
                if (!ModelState.IsValid)
                {
                    return View("ChangePassword", changePasswordModel);
                }
                if (changePasswordModel != null && _authenticationService.VerifyPassword(changePasswordModel.CurrentPassword, user.PasswordSalt, user))
                {
                    bool result = _userService.ChangePassword(changePasswordModel);
                    changePasswordModel.SuccessMessage = UserMessageConstants.passwordChangedMessage;
                    return View("ChangePassword", changePasswordModel);
                }
                else
                {
                    changePasswordModel.ErrorMessage = UserMessageConstants.invalidPwdMessage;
                    return View("ChangePassword", changePasswordModel);
                }

                return View("ChangePassword", changePasswordModel);
            }
            catch (Exception ex)
            {
                changePasswordModel.ErrorMessage = ex.Message;
                return View(changePasswordModel);
            }
        }
    }
}
