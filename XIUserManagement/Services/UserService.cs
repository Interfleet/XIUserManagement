using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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
        public List<Users> SortUserData(string sortOrder,List<Users> userList)
        {
            switch (sortOrder)
            {
                case UserMessageConstants.sortOrderUserNameDesc:
                    userList = userList.OrderByDescending(u => u.UserName).ToList();
                    break;
                case UserMessageConstants.sortOrderCompanyAsc:
                    userList = userList.OrderBy(u => u.Company).ToList();
                    break;
                case UserMessageConstants.sortOrderCompanyDesc:
                    userList = userList.OrderByDescending(u => u.Company).ToList();
                    break;
                default:
                    userList = userList.OrderBy(u => u.UserName).ToList();
                    break;
            }
            return userList;
        }
        public bool IsAdmin(LoginViewModel _loginViewModel, List<Users> userList)
        {
            return _loginViewModel.UserName != null && userList.Where(u => u.UserName.ToUpper() == _loginViewModel.UserName.ToUpper()).Select(i => i.IsAdmin).FirstOrDefault();
        }
        public void UnlockUser(Users user)
        {
            user.InvalidLoginAttempts = 0;
            user.UserAccountDisabled = false;
        }
        public Users GetUserByUserId(int userId)
        {
            Users user = _userRepository.GetUserById(userId);
            return user;
        }
        public Users? GetUserByUserName(string userName)
        {
            Users user = _userRepository.FindUserByName(userName);
            return user;
        }
        public bool SaveUser(Users user)
        {
            bool userSaved = _userRepository.Save(user);
            return userSaved;
        }
        public bool UpdateUser(Users user)
        {
            bool userUpdated = _userRepository.Update(user);
            return userUpdated;
        }
        public bool ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            bool userUpdated = _userRepository.ResetPassword(resetPasswordModel);
            return userUpdated;
        }
        public bool DeleteUser(Users user)
        {
            bool userDeleted = _userRepository.Delete(user);
            return userDeleted;
        }
    }
}
