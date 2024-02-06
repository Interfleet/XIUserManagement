using Azure.Core;
using Interfleet.XaltAuthenticationAPI.Models;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Context;
using Interfleet.XIUserManagement.Exceptions;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
using Interfleet.XIUserManagement.Services;
using System;
using System.Linq;

namespace Interfleet.XaltAuthenticationAPI.Services
{
    public class AuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly DapperContext _context;
        private readonly IAuthorization _authorization;
        private readonly Users _userInfo;
        private readonly UserService _userService;
        public AuthenticationService(IUserRepository userRepository, DapperContext context, IAuthorization authorization, Users userInfo,UserService userService)
        {
            _userRepository = userRepository;
            _context = context;
            _authorization = authorization;
            _userInfo = userInfo;
            _userService = userService;
        }
        private static void ClearData(Users user)
        {
            user.UserName = "";
            user.Password = "";
            user.Company = "";
            user.Comments = "";
        }
        //This method is used to verify if user already exists
        public bool VerifyUser(string userName)
        {
            var user = _userService.GetUserByUserName(userName);

            if (user != null && userName == user.UserName)
            {
                ClearData(user);
                throw new ApplicationException(UserMessageConstants.userExistsMessage);
            }

            return true;

        }

        //This method is used for verifying the password associated with the user
        public bool VerifyPassword(string password, byte[] passwordSalt, Users user)
        {
            if (user != null && user.Id.Equals(Guid.Empty))
                throw new ApplicationException(UserMessageConstants.createUserMessage);

            if (user == null)
                throw new UserNotFoundException(UserMessageConstants.errorInLoginMessage);
            return user.VerifyPasswordWithLockout(password, passwordSalt);

        }


        //This method authenticates service request and returns user token associated with the user
        //Also checks for invalid login attempts and locks the user if invalidloginattempts >= 4
        public Token Authenticate(LoginViewModel userRequest)
        {
            using var connection = _context.CreateConnection();
            var user = (userRequest.UserName != null ? _userService.GetUserByUserName(userRequest.UserName) : null) ?? throw new UserNotFoundException(string.Format(UserMessageConstants.userNotFoundMessage, userRequest.UserName));
            
            if (user != null && userRequest.Password!=null && VerifyPassword(userRequest.Password, user.PasswordSalt, user))
            {
                Authorize(new Token(userRequest.Id), QueryConstants.RequiredRoles);
                return new Token(userRequest.Id);
            }
            if (user!=null && user.InvalidLoginAttempts > 0) { _userService.UpdateUser(user); }

            if (user != null && user.InvalidLoginAttempts >= 4)
            {
                user.UserAccountDisabled = true;
                _userService.UpdateUser(user);
                user.ErrorMessage = UserMessageConstants.userAccountDisabledMessage;
                throw new UserBlockedException(string.Format(UserMessageConstants.userAccountDisabledMessage, userRequest.UserName));
            }

            throw new AuthorizationException(string.Format(UserMessageConstants.wrongPwdMessage, userRequest.UserName));

        }


        //This method is for authorizing the user with the roles associted with it
        public bool Authorize(Token userToken, string[] requiredRoles)
        {
            if (requiredRoles != null && requiredRoles.Length > 0)
            {
                foreach (var role in requiredRoles.Where(role => !_authorization.Authorize(userToken, role)))
                {
                    throw new AuthorizationException(string.Format(UserMessageConstants.authorizationFailedMessage, role));
                }
            }
            return true;
        }




    }
}
