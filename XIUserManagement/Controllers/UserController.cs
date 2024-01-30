using Interfleet.XaltAuthenticationAPI.Services;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Interfleet.XIUserManagement.Services;
using Microsoft.AspNetCore.Mvc;



namespace Interfleet.XIUserManagement.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly AuthenticationService _authenticationService;
        private readonly UserService _userService;
        List<Users> lstUserInfo = new List<Users>();
        List<Users> lstUserRecInfo = new List<Users>();
        LoginViewModel _loginViewModel;
        const int pageSize = 10;

        public UserController(IUserRepository userRepository, AuthenticationService authenticationService, UserService userService, LoginViewModel loginViewModel)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _userService = userService;
            _loginViewModel = loginViewModel;
        }
        [HttpGet]
        public IActionResult Index(string searchValue, string searchBy, int pg = 1)
        {
            try
            {
                Users user = new();
                Users userInfo = new();
                _loginViewModel.UserName = HttpContext.Session.GetString("username");
                lstUserInfo = _userRepository.GetUsers();
                var adminUser = lstUserInfo.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();

                //search functionality
                if (searchBy != null && searchValue != null)
                {
                    lstUserInfo = _userService.Search(lstUserInfo, user, searchBy, searchValue);
                }

                //pagination
                var pager = new Pager(lstUserInfo.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, lstUserInfo, lstUserRecInfo, pager, pageSize);
                ViewBag.pager = pager;

                return adminUser ? View("Admin_Index", lstUserRecInfo) : View("User_Index", lstUserRecInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View("User_Index", lstUserRecInfo);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create", new Users());
        }

        [HttpPost]
        public IActionResult Create(Users user, int pg = 1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    user.ErrorMessage = "Model data is invalid!";
                    return View(user);
                }
                if (_authenticationService.VerifyUser(user.UserName))
                {
                    bool result = _userRepository.Save(user);
                    if (!result)
                    {
                        user.ErrorMessage = "Unable to save user details!";
                        return View();
                    }
                }

                user.SuccessMessage = "User details saved successfully!";
                lstUserInfo = _userRepository.GetUsers();
                //pagination
                var pager = new Pager(lstUserInfo.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, lstUserInfo, lstUserRecInfo, pager, pageSize);
                ViewBag.pager = pager;
                return lstUserInfo.Where(u => u.UserName == _loginViewModel.UserName).Select(i => i.IsAdmin).FirstOrDefault() ? View("Admin_Index", lstUserRecInfo) : View("User_Index", lstUserRecInfo);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Edit(int userId)
        {
            Users user = new();
            try
            {
                if (userId == 0)
                {
                    user.ErrorMessage = "User with id{0} not found";
                    return RedirectToAction("User_Index");
                }
                user = _userRepository.GetUserById(userId);
                return View("Edit", user);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult Edit(Users user, int pg = 1)
        {
            try
            {
                user.UserId = Convert.ToInt32(Request.Form["userId"]);
                bool result = _userRepository.Update(user);
                if (!result)
                {
                    user.ErrorMessage = "Unable to update user details!";
                    return View();
                }
                user.SuccessMessage = "User details updated successfully!";
                lstUserInfo = _userRepository.GetUsers();
                //pagination
                var pager = new Pager(lstUserInfo.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, lstUserInfo, lstUserRecInfo, pager, pageSize);
                ViewBag.pager = pager;
                return lstUserInfo.Where(u => u.UserName == _loginViewModel.UserName).Select(i => i.IsAdmin).FirstOrDefault() ? View("Admin_Index", lstUserRecInfo) : View("User_Index", lstUserRecInfo);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Delete(int userId)
        {
            Users user = new();
            try
            {
                if (userId == 0)
                {
                    user.ErrorMessage = "User with id{0} not found";
                    return RedirectToAction("User_Index");
                }
                user = _userRepository.GetUserById(userId);
                return View("Delete", user);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult Delete(Users user, int pg = 1)
        {
            try
            {
                user.UserId = Convert.ToInt32(Request.Form["userId"]);
                bool result = _userRepository.Delete(user);
                if (!result)
                {
                    user.ErrorMessage = "Unable to delete user details!";
                    return View();
                }
                user.SuccessMessage = "User details deleted successfully!";
                lstUserInfo = _userRepository.GetUsers();
                //pagination
                var pager = new Pager(lstUserInfo.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, lstUserInfo, lstUserRecInfo, pager, pageSize);
                ViewBag.pager = pager;
                return lstUserInfo.Where(u => u.UserName == _loginViewModel.UserName).Select(i => i.IsAdmin).FirstOrDefault() ? View("Admin_Index", lstUserRecInfo) : View("User_Index", lstUserRecInfo);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
    }
}
