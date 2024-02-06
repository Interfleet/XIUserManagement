using Interfleet.XIUserManagement.Constants;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace Interfleet.XIUserManagement.Models
{
    public class Users
    {
        private const int SaltLen = 128;
        public string ErrorMessage = "";
        public string SuccessMessage = "";
        public string deleteUserMessage = UserMessageConstants.deleteUserMessage;
        public string unlockUserMessage = UserMessageConstants.accountUnlockMessage;
        public Users()
        {
        }
        public void ChangePassword(string newPassword)
        {
            if (newPassword == null) throw new ArgumentNullException(nameof(newPassword));

            var salt = GenerateSalt();
            var hash = HashPassword(newPassword, salt);

            PasswordHash = hash;
            PasswordSalt = salt;


        }
        public Users(Guid id, string userName, string password,
                    string company, string comments, bool isAdmin, int userid, int invalidLoginAttempts, bool userAccountDisabled)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");

            Id = id;
            UserId = userid;
            UserName = userName;
            Password = password;
            Company = company;
            Comments = comments;
            IsAdmin = isAdmin;
            InvalidLoginAttempts = invalidLoginAttempts;
            UserAccountDisabled = userAccountDisabled;
        }
        [Pure]
        public byte[] GenerateSalt()
        {
            var random = RandomNumberGenerator.Create();
            var salt = new byte[SaltLen];
            random.GetNonZeroBytes(salt);
            return salt;
        }
        [Pure]
        public byte[] HashPassword(string password, byte[] passwordSalt)
        {
            var pwd = Encoding.UTF8.GetBytes(password);
            var pwdPlusSalt = new byte[pwd.Length + SaltLen];

            Array.Copy(pwd, pwdPlusSalt, pwd.Length);
            Array.Copy(passwordSalt, 0, pwdPlusSalt, pwd.Length, SaltLen);

            return SHA512.Create().ComputeHash(pwdPlusSalt);
            //SHA512 hasher = new();
            //return hasher.ComputeHash(pwdPlusSalt);
        }
        public bool VerifyPasswordWithLockout(string password, byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            if (!VerifyPassword(password, passwordSalt))
            {
                if (InvalidLoginAttempts >= 4)
                {
                    UserAccountDisabled = true;

                }
                else
                    InvalidLoginAttempts += 1;

                return false;
            }

            InvalidLoginAttempts = 0;
            return true;
        }
        /// <summary>
        /// 	Verifies the password
        /// <param name = "password">Password to verify</param>
        public bool VerifyPassword(string password)
        {

            if (password == null)
                throw new ArgumentNullException("password");

            return true;
        }

        /// <param name = "password">UTF8-encoded Password to validate.</param>
        /// <returns>Whether the password was correctly validated, with respect to
        /// 	the password/username combo currently in use by the user.</returns>
        [Pure]
        internal bool VerifyPassword(string password, byte[] passwordSalt)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var hash = HashPassword(password, passwordSalt);

            for (var i = 0; i < hash.Length; i++)
                if (!hash[i].Equals(PasswordHash[i]))
                    return false;

            return true;
        }

        [Pure]
        public bool VerifyHashedPassword(string hashOfPassword)
        {
            var bytes = Convert.FromBase64String(hashOfPassword);
            return bytes.SequenceEqual(PasswordHash);
        }
        protected internal byte[] PasswordHash { get; set; }

        protected internal byte[] PasswordSalt { get; set; }

        protected internal int UserId { get; set; }

        protected internal Guid Id { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(UserMessageConstants.passwordRegularExpression, ErrorMessage = UserMessageConstants.passwordValidatorMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = UserMessageConstants.passwordComparatorMessage)]
        public string ConfirmPassword { get; set; }


        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? Company { get; set; }


        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string? Comments { get; set; }
        public bool IsAdmin { get; set; }
        public int InvalidLoginAttempts { get; set; }
        public bool UserAccountDisabled { get; set; }

    }
}
