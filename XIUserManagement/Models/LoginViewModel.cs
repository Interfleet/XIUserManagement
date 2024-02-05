using System.ComponentModel.DataAnnotations;

namespace Interfleet.XIUserManagement.Models
{
    public class LoginViewModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        internal Guid Id { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
