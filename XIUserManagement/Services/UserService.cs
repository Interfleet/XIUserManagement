using Interfleet.XIUserManagement.Models;

namespace Interfleet.XIUserManagement.Services
{
    public class UserService
    {
        public List<Users> Search(List<Users> lstUserInfo, Search search, string searchBy, string searchValue)
        {
            //search functionality
            if (lstUserInfo.Count == 0)
            {
                search.ErrorMessage = "No user available";
            }
            if (string.IsNullOrEmpty(searchValue))
            {
                search.ErrorMessage = "Please enter search value";
            }
            else
            {
                if (searchBy.ToLower() == "username")
                {
                    lstUserInfo = lstUserInfo.Where(u => u.UserName.ToLower().Contains(searchValue.ToLower())).ToList();
                }
                if (searchBy.ToLower() == "company")
                {
                    lstUserInfo = lstUserInfo.Where(u => u.Company.ToLower().Contains(searchValue.ToLower())).ToList();
                }
            }
            return lstUserInfo;
        }
        public List<Users> Pagination(int pg, List<Users> lstUserInfo, List<Users> lstUserRecInfo, Pager pager, int pageSize)
        {
            if (pg < 1)
                pg = 1;
            int userCount = lstUserInfo.Count;
            pager = new Pager(userCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            return lstUserInfo.Skip(recSkip).Take(pager.PageSize).ToList();
        }
    }
}
