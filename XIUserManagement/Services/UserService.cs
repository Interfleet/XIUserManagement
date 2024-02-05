using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;

namespace Interfleet.XIUserManagement.Services
{
    public class UserService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IUserRepository _userRepository;
        public UserService(IMemoryCache memoryCache, IUserRepository userRepository)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
        }

        //This method caches user data
        public List<Users> CacheUserData(string? cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out List<Users> lstUsers))
            {
                //calling the server
                lstUsers = _userRepository.GetUsers();

                //setting up cache options
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, lstUsers, cacheExpiryOptions);
            }
            return lstUsers;
        }
        public List<SelectListItem> GetPageSizes(int selectedPageSize = 5)
        {
            var pagesSizes = new List<SelectListItem>();

            if (selectedPageSize == 5)
                pagesSizes.Add(new SelectListItem("5", "5", true));
            else
                pagesSizes.Add(new SelectListItem("5", "5"));

            for (int lp = 10; lp <= 100; lp += 10)
            {
                if (lp == selectedPageSize)
                { pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true)); }
                else
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString()));
            }

            return pagesSizes;
        }
        public bool IsAdmin(LoginViewModel _loginViewModel, List<Users> userList)
        {
            return _loginViewModel.UserName != null && userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();
        }
    }
}
