using Interfleet.XaltAuthenticationAPI.Services;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Interfleet.XIUserManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;



namespace Interfleet.XIUserManagement.Controllers
{
    public class UserController : Controller
    {

        private readonly AuthenticationService _authenticationService;
        private readonly UserService _userService;
        private readonly IMemoryCache _memoryCache;
        Search search = new();
        List<Users> userList = new();
        List<Users> paginatedUserList = new();
        LoginViewModel _loginViewModel;
        const int pageSize = 10;
        const string cacheKey = UserMessageConstants.cacheKey;

        public UserController(AuthenticationService authenticationService, UserService userService, LoginViewModel loginViewModel, IMemoryCache memoryCache)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _loginViewModel = loginViewModel;
            _memoryCache = memoryCache;
        }
        public IActionResult Index(string searchBy, string searchValue, int pg = 1, int pageSize = 10)
        {
            try
            {
                Users user = new();
                Users userInfo = new();
                _loginViewModel.UserName = HttpContext.Session.GetString("username");
                userList = _userService.CacheUserData(cacheKey);

                if (pg < 1) pg = 1;
                if (searchValue != "" && searchValue != null && searchBy != "" && searchBy != null && searchBy.ToLower() == UserMessageConstants.searchValueOption1)
                {
                    userList = userList.Where(u => u.UserName.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                else if (searchValue != "" && searchValue != null && searchBy != "" && searchBy != null && searchBy.ToLower() == UserMessageConstants.searchValueOption2)
                {
                    userList = userList.Where(u => u.Company != null && u.Company.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                paginatedUserList = userList.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                Pager SearchPager = new(userList.Count, pg, pageSize) { Action = "Index", Controller = "User", SearchValue = searchValue, SearchBy = searchBy };
                ViewBag.SearchPager = SearchPager;
                ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(paginatedUserList, ViewBag.Search);

                return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(UserMessageConstants.userIndex, paginatedUserList);
        }
       
        public IActionResult View(int userId)
        {
            Users user = new();
            try
            {
                if (userId == 0)
                {
                    user.ErrorMessage = UserMessageConstants.userNotFoundMessage;
                    return RedirectToAction(UserMessageConstants.userIndex);
                }
                TempData["UserId"] = userId;
                user = _userService.GetUserByUserId(userId);
                userList = _userService.CacheUserData(cacheKey);
                return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminView,user) : View(UserMessageConstants.userView,user);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
        public IActionResult Clear(string searchValue, string searchBy, int pg = 1)
        {
            searchValue = string.Empty;
            searchBy = string.Empty;
            userList = _userService.CacheUserData(cacheKey);


            paginatedUserList = userList.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
            Pager SearchPager = new(userList.Count, pg, pageSize) { Action = "Index", Controller = "User", SearchValue = searchValue, SearchBy = searchBy };
            ViewBag.SearchPager = SearchPager;
            ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
            var userSearchModel = new Tuple<List<Users>, Search>(paginatedUserList, ViewBag.Search);

            return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
        }

        public IActionResult UnlockUser()
        {
            int userId = Convert.ToInt32(TempData["UserId"]);
            var user = _userService.GetUserByUserId(userId);
            user.SuccessMessage = UserMessageConstants.accountUnlockSuccessMessage;
            _userService.UnlockUser(user);
            _userService.UpdateUser(user);
            return View("Edit", user);
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
                    user.ErrorMessage = UserMessageConstants.modelInvalidDataMessage;
                    return View(user);
                }
                if (_authenticationService.VerifyUser(user.UserName))
                {
                    bool result = _userService.SaveUser(user);
                    if (!result)
                    {
                        user.ErrorMessage = UserMessageConstants.dataNotSavedMessage;
                        return View();
                    }
                }

                user.SuccessMessage = UserMessageConstants.dataSavedMessage;
                userList = _userService.CacheUserData(cacheKey);

                if (pg < 1) pg = 1;
                paginatedUserList = userList.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                Pager SearchPager = new(userList.Count, pg, pageSize) { Action = "Index", Controller = "User", SearchValue = "", SearchBy = "" };
                ViewBag.SearchPager = SearchPager;
                ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(paginatedUserList, ViewBag.Search);

                return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
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
                    user.ErrorMessage = UserMessageConstants.userNotFoundMessage;
                    return RedirectToAction(UserMessageConstants.userIndex);
                }
                TempData["UserId"] = userId;
                user = _userService.GetUserByUserId(userId);
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
                bool result = _userService.UpdateUser(user);
                if (!result)
                {
                    user.ErrorMessage = UserMessageConstants.dataNotUpdatedMessage;
                    return View();
                }
                user.SuccessMessage = UserMessageConstants.dataUpdatedMessage;
                userList = _userService.CacheUserData(cacheKey);

                if (pg < 1) pg = 1;
                paginatedUserList = userList.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                Pager SearchPager = new (userList.Count, pg, pageSize) { Action = "Index", Controller = "User", SearchValue = "", SearchBy = "" };
                ViewBag.SearchPager = SearchPager;
                ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(paginatedUserList, ViewBag.Search);

                return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
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
                    user.ErrorMessage = UserMessageConstants.userNotFoundMessage;
                    return RedirectToAction(UserMessageConstants.userIndex);
                }
                user = _userService.GetUserByUserId(userId);
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
                bool result = _userService.DeleteUser(user);
                if (!result)
                {
                    user.ErrorMessage = UserMessageConstants.dataNotDeletedMessage;
                    return View();
                }
                user.SuccessMessage = UserMessageConstants.dataDeletedMessage;
                userList = _userService.CacheUserData(cacheKey);

                if (pg < 1) pg = 1;
                paginatedUserList = userList.Skip((pg - 1) * pageSize).Take(pageSize).ToList();
                Pager SearchPager = new (userList.Count, pg, pageSize) { Action = "Index", Controller = "User", SearchValue = "", SearchBy = "" };
                ViewBag.SearchPager = SearchPager;
                ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(paginatedUserList, ViewBag.Search);

                return _userService.IsAdmin(_loginViewModel, userList) ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
    }
}
