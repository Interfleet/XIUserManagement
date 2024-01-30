using Interfleet.XaltAuthenticationAPI.Services;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Interfleet.XIUserManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;



namespace Interfleet.XIUserManagement.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepository;
        private readonly AuthenticationService _authenticationService;
        private readonly UserService _userService;
        private readonly IMemoryCache _memoryCache;
        List<Users> userList = new List<Users>();
        Search search = new Search();
        List<Users> lstUserRecInfo = new List<Users>();
        LoginViewModel _loginViewModel;
        const int pageSize = 10;
        const string cacheKey = "userList";

        public UserController(IUserRepository userRepository, AuthenticationService authenticationService, UserService userService, LoginViewModel loginViewModel, IMemoryCache memoryCache)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
            _userService = userService;
            _loginViewModel = loginViewModel;
            _memoryCache = memoryCache;
        }
        [HttpGet]
        public IActionResult Index(int pg = 1)
        {
            try
            {
                Users user = new();
                Users userInfo = new();
                _loginViewModel.UserName = HttpContext.Session.GetString("username");

                
                //checks if cache entries exists
                if (!_memoryCache.TryGetValue(cacheKey, out List<Users> userList))
                {
                    //calling the server
                    userList = _userRepository.GetUsers();

                    //setting up cache options
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(8)
                    };
                    //setting cache entries
                    _memoryCache.Set(cacheKey, userList, cacheExpiryOptions);
                }



                var adminUser = userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();

                //pagination
                var pager = new Pager(userList.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, userList, lstUserRecInfo, pager, pageSize);
                ViewBag.pager = pager;
                var userSearchModel = new Tuple<List<Users>, Search>(lstUserRecInfo, search);
                return adminUser ? View("Admin_Index", userSearchModel) : View("User_Index", userSearchModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View("User_Index", lstUserRecInfo);
        }
        [HttpGet]
        public IActionResult Search(string searchValue, string searchBy,int pg=1)
        {
            var cacheKey = "userList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<Users> userList))
            {
                //calling the server
                userList = _userRepository.GetUsers();

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(8)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, userList, cacheExpiryOptions);
            }
            userList = _userService.Search(userList, search, searchBy, searchValue);

            var adminUser = userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();
            
            //pagination
            var pager = new Pager(userList.Count, pg, pageSize);
            lstUserRecInfo = _userService.Pagination(pg, userList, lstUserRecInfo, pager, pageSize);
            var userSearchModel = new Tuple<List<Users>, Search>(lstUserRecInfo, search);
            ViewBag.pager = pager;
            return adminUser ? View("Admin_Index", userSearchModel) : View("User_Index", userSearchModel);
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
                userList = _userRepository.GetUsers();
                var adminUser = userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();

                //pagination
                var pager = new Pager(userList.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, userList, lstUserRecInfo, pager, pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(lstUserRecInfo, search);
                ViewBag.pager = pager;
                return adminUser ? View("Admin_Index", userSearchModel) : View("User_Index", userSearchModel);
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

                if (!_memoryCache.TryGetValue(cacheKey, out List<Users> userList))
                {
                    //calling the server
                    userList = _userRepository.GetUsers();

                    //setting up cache options
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(8)
                    };
                    //setting cache entries
                    _memoryCache.Set(cacheKey, userList, cacheExpiryOptions);
                }
                var adminUser = userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();

                //pagination
                var pager = new Pager(userList.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, userList, lstUserRecInfo, pager, pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(lstUserRecInfo, search);
                ViewBag.pager = pager;
                return adminUser ? View("Admin_Index", userSearchModel) : View("User_Index", userSearchModel);
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

                if (!_memoryCache.TryGetValue(cacheKey, out List<Users> userList))
                {
                    //calling the server
                    userList = _userRepository.GetUsers();

                    //setting up cache options
                    var cacheExpiryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(10),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromMinutes(8)
                    };
                    //setting cache entries
                    _memoryCache.Set(cacheKey, userList, cacheExpiryOptions);
                }
                var adminUser = userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();

                //pagination
                var pager = new Pager(userList.Count, pg, pageSize);
                lstUserRecInfo = _userService.Pagination(pg, userList, lstUserRecInfo, pager, pageSize);
                var userSearchModel = new Tuple<List<Users>, Search>(lstUserRecInfo, search);
                ViewBag.pager = pager;
                return adminUser ? View("Admin_Index", userSearchModel) : View("User_Index", userSearchModel);
            }
            catch (Exception ex)
            {
                user.ErrorMessage = ex.Message;
                return View(user);
            }
        }
    }
}
