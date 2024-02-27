using Interfleet.XIUserManagement.Constants;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Interfleet.XIUserManagement.Models
{
    public class Search
    {
        public const string sortOrderUserNameParam = UserMessageConstants.sortOrderUserNameParam;
        public const string sortOrderCompanyParam = UserMessageConstants.sortOrderCompanyParam;
        public Search() {
            SearchBy = UserMessageConstants.userName;
            SearchValue=string.Empty;
        }
        public Search(string searchValue, string searchBy)
        {
            SearchValue = string.Empty;
            SearchBy = searchBy;
        }
        public string SelectedOption { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string? SearchValue { get; set; }
        public string? SearchBy { get; set; }
        public string ErrorMessage = "";
    }
}
