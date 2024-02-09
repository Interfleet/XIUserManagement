using Interfleet.XIUserManagement.Constants;
using System.ComponentModel.DataAnnotations;

namespace Interfleet.XIUserManagement.Models
{
    public class ResetPasswordModel
    {
        public string SuccessMessage = "";
        public string ErrorMessage = "";
        [Required]
        public int? UserId { get; set; }
        [Required, DataType(DataType.Password)]
        [RegularExpression(UserMessageConstants.passwordRegularExpression, ErrorMessage = UserMessageConstants.passwordValidatorMessage)]
        public string? NewPassword { get; set; }
        
        [Required,DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = UserMessageConstants.passwordComparatorMessage)]
        public string? ConfirmPassword { get; set; }
        protected internal byte[]? PasswordHash { get; set; }

        protected internal byte[]? PasswordSalt { get; set; }
    }
}
