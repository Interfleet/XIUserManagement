using Interfleet.XaltAuthenticationAPI.Services;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;
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
        readonly Search search = new();
        List<Users> userList = new();
        readonly List<Users> paginatedUserList = new();
        readonly LoginViewModel _loginViewModel;
        const string cacheKey = UserMessageConstants.cacheKey;

        public UserController(AuthenticationService authenticationService, UserService userService, LoginViewModel loginViewModel, IMemoryCache memoryCache)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _loginViewModel = loginViewModel;
            _memoryCache = memoryCache;
        }
        private void SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users user, List<Users> userList, int userId, string searchValue, string? searchBy, int pg = 1, int pageSize = 10)
        {
            user = _userService.GetUserByUserId(userId);
            isAdmin = _userService.IsAdmin(_loginViewModel, userList);
            searchBy ??= (new Search().SearchBy ?? null);
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
            Pager SearchPager = new(userList.Count, pg, pageSize) { Action = UserMessageConstants.index, Controller = UserMessageConstants.user, SearchValue = searchValue, SearchBy = searchBy };
            ViewBag.SearchPager = SearchPager;
            ViewBag.PageSizes = _userService.GetPageSizes(pageSize);
            try
            {
                if (user != null && userId == 0)
                {
                    user.ErrorMessage = UserMessageConstants.userNotFoundMessage;
                }
                TempData[UserMessageConstants.userId] = userId;
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
            }
        }
        public IActionResult Index(string searchBy, string searchValue, string sortOrder, int pg = 1, int pageSize = 10)
        {
            try
            {
                Users userInfo = new();
                _loginViewModel.UserName = HttpContext.Session.GetString(UserMessageConstants.searchValueOption1);
                ViewData[UserMessageConstants.sortOrderUserNameParam] = string.IsNullOrEmpty(sortOrder) ? UserMessageConstants.sortOrderUserNameDesc : "";
                ViewData[UserMessageConstants.sortOrderCompanyParam] = sortOrder == UserMessageConstants.sortOrderCompanyAsc ? UserMessageConstants.sortOrderCompanyDesc : UserMessageConstants.sortOrderCompanyAsc;
                userList = _userService.GetUserData();
                userList = _userService.SortUserData(sortOrder, userList);
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users user, userList, 0, searchValue, searchBy, pg, pageSize);

                var userSearchModel = new Tuple<List<Users>, Search, Pager>(paginatedUserList, ViewBag.Search, ViewBag.SearchPager);
                return isAdmin ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View(UserMessageConstants.userIndex, paginatedUserList);
        }

        public IActionResult View(string searchValue, string searchBy, int userId, int pg = 1, int pageSize = 10)
        {
            Users userDetails = new();
            try
            {
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users user, userList, userId, searchValue, searchBy, pg, pageSize);
                var userSearchModel = new Tuple<Users, Search, Pager>(user, ViewBag.Search, ViewBag.SearchPager);
                return isAdmin ? View(UserMessageConstants.adminView, userSearchModel) : View(UserMessageConstants.userView, userSearchModel);
            }
            catch (Exception ex)
            {
                userDetails.ErrorMessage = ex.Message;
                return View(userDetails);
            }
        }
        public IActionResult Clear(int pg = 1, int pageSize = 10)
        {
            userList = _userService.GetUserData();
            SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users _, userList, 0, string.Empty, UserMessageConstants.userName, pg, pageSize);
            var userSearchModel = new Tuple<List<Users>, Search, Pager>(paginatedUserList, ViewBag.Search, ViewBag.SearchPager);
            return isAdmin ? View(UserMessageConstants.adminIndex, userSearchModel) : View(UserMessageConstants.userIndex, userSearchModel);
        }

        public IActionResult UnlockUser(string searchValue, string searchBy, int pg = 1, int pageSize = 10)
        {
            int userId = Convert.ToInt32(TempData[UserMessageConstants.userId]);
            var user = _userService.GetUserByUserId(userId);
            user.SuccessMessage = UserMessageConstants.accountUnlockSuccessMessage + user.UserName;
            _userService.UnlockUser(user);
            _userService.UpdateUser(user);
            SetSearchAndPagination(out bool _, out List<Users> _, out Users _, userList, 0, searchValue, searchBy, pg, pageSize);
            var userSearchModel = new Tuple<Users, Search, Pager>(user, ViewBag.Search, ViewBag.SearchPager);
            return View(UserMessageConstants.adminView, userSearchModel);
        }
        [HttpGet]
        public IActionResult Create(int pg = 1, int pageSize = 10)
        {
            return View(UserMessageConstants.create, new Users());
        }

        [HttpPost]
        public IActionResult Create(Users user, int pg = 1, int pageSize = 10)
        {
            var userSearchModel = new Tuple<Users, Search, Pager>(new Users(), ViewBag.Search, ViewBag.SearchPager);
            SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users _, userList, 0, search.SearchValue, search.SearchBy, pg, pageSize);
            var userSearchModelIndex = new Tuple<List<Users>, Search, Pager>(paginatedUserList, ViewBag.Search, ViewBag.SearchPager);

            if (!ModelState.IsValid)
            {
                string errors = string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                ModelState.AddModelError("", errors);
                return View(UserMessageConstants.create, user);
            }
            if (_authenticationService.VerifyUser(user.UserName))
            {
                bool result = _userService.SaveUser(user);
                if (!result)
                {
                    user.ErrorMessage = UserMessageConstants.dataNotSavedMessage;
                    return View(userSearchModel);
                }
            }
            else
            {
                user.ErrorMessage = UserMessageConstants.userExistsMessage;
                return View(UserMessageConstants.create, user);
            }
            user.SuccessMessage = UserMessageConstants.dataSavedMessage;
            _userService.ClearUserData(user);
            return View(UserMessageConstants.create, user);
        }

        [HttpGet]
        public IActionResult Edit(int userId, int pg = 1, int pageSize = 10)
        {
            Users userDetails = new();
            try
            {
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users user, userList, userId, "", "", pg, pageSize);
                return View(UserMessageConstants.edit, user);
            }
            catch (Exception ex)
            {
                userDetails.ErrorMessage = ex.Message;
                return View(userDetails);
            }
        }

        [HttpPost]
        public IActionResult Edit(Users user, int pg = 1, int pageSize = 10)
        {
            try
            {
                user.UserId = Convert.ToInt32(Request.Form[UserMessageConstants.userId]);
                bool result = _userService.UpdateUser(user);
                if (!result)
                {
                    user.ErrorMessage = UserMessageConstants.dataNotUpdatedMessage;
                    return View();
                }
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users _, userList, 0, "", "", pg, pageSize);
                user.SuccessMessage = UserMessageConstants.dataUpdatedMessage;
                var userSearchModel = new Tuple<List<Users>, Search, Pager>(paginatedUserList, ViewBag.Search, ViewBag.SearchPager);
                return View(UserMessageConstants.edit, user);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Delete(int userId, int pg = 1, int pageSize = 10)
        {
            Users userDetails = new();
            try
            {
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users user, userList, userId, "", "", pg, pageSize);
                return View(UserMessageConstants.delete, user);

            }
            catch (Exception ex)
            {
                userDetails.ErrorMessage = ex.Message;
                return View(userDetails);
            }
        }

        [HttpPost]
        public IActionResult Delete(Users user, int pg = 1, int pageSize = 10)
        {
            try
            {
                user.UserId = Convert.ToInt32(Request.Form[UserMessageConstants.userId]);
                bool result = _userService.DeleteUser(user);
                if (!result)
                {
                    user.ErrorMessage = UserMessageConstants.dataNotDeletedMessage;
                    return View();
                }
                SetSearchAndPagination(out bool isAdmin, out List<Users> paginatedUserList, out Users _, userList, 0, "", "", pg, pageSize);
                user.SuccessMessage = UserMessageConstants.dataDeletedMessage;
                var userSearchModel = new Tuple<List<Users>, Search, Pager>(paginatedUserList, ViewBag.Search, ViewBag.SearchPager);
                _userService.ClearUserData(user);
                return View(UserMessageConstants.delete, user);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
    }
}
