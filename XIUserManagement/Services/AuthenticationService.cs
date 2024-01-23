using Interfleet.XaltAuthenticationAPI.Models;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Context;
using Interfleet.XIUserManagement.Exceptions;
using Interfleet.XIUserManagement.Models;
using Interfleet.XIUserManagement.Repositories;
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
        public AuthenticationService(IUserRepository userRepository, DapperContext context, IAuthorization authorization, Users userInfo)
        {
            _userRepository = userRepository;
            _context = context;
            _authorization = authorization;
            _userInfo = userInfo;
        }
        private void ClearData(Users user)
        {
            user.UserName = "";
            user.Password = "";
            user.Company = "";
            user.Comments = "";
        }
        //This method is used for verifying the password associated with the user
        public bool VerifyUser(string userName)
        {
            var user = _userRepository.FindUserByName(userName);

            if (user != null && userName == user.UserName)
            {
                ClearData(user);
                throw new ApplicationException("User already exists. Please enter some other username");
            }

            return true;

        }

        //This method is used for verifying the password associated with the user
        public bool VerifyPassword(string userName, string password, byte[] passwordSalt)
        {
            var user = _userRepository.FindUserByName(userName);

            if (user != null && user.Id.Equals(Guid.Empty))
                throw new ApplicationException("First create the user before trying to login with that user.");

            if (user == null)
                throw new UserNotFoundException("Can not login with a non existing user");

            return user.VerifyPasswordWithLockout(password, passwordSalt);

        }


        //This method authenticates service request and returns user token associated with the user
        public Token Authenticate(LoginViewModel userRequest)
        {
            using var connection = _context.CreateConnection();
            var user = _userRepository.FindUserByName(userRequest.UserName);

            if (user == null)
            {
                throw new UserNotFoundException(string.Format("User {0} does not exist", userRequest.UserName));
            }

            if (user != null && VerifyPassword(userRequest.UserName,userRequest.Password,user.PasswordSalt))
            {
                Authorize(new Token(userRequest.Id), Constants.RequiredRoles);
                return new Token(userRequest.Id);
            }

            throw new AuthorizationException(string.Format("Login failed for user {0}", userRequest.UserName));

        }


        //This method is for authorizing the user with the roles associted with it
        public bool Authorize(Token userToken, string[] requiredRoles)
        {
            if (requiredRoles != null && requiredRoles.Length > 0)
            {
                foreach (var role in requiredRoles.Where(role => !_authorization.Authorize(userToken, role)))
                {
                    throw new AuthorizationException(string.Format("Authorization failed. Required role {0}", role));
                }
            }
            return true;
        }




    }
}
