using System.ComponentModel.DataAnnotations;

namespace Interfleet.XIUserManagement.Models
{
    public class Search
    {
        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string? SearchValue { get; set; }
        public string ErrorMessage = "";
    }
}
