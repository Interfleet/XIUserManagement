using Interfleet.XIUserManagement.Context;
using Dapper;
using Interfleet.XIUserManagement.Models;


namespace Interfleet.XIUserManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        private readonly Users _userInfo;
        public UserRepository(DapperContext context, Users userInfo)
        {
            _context = context;
            _userInfo = userInfo;
        }

        public List<Users> GetUsers()
        {
            using var connection = _context.CreateConnection();
            var UserInfo = connection.Query<Users>(Constants.QueryConstants.GetAllUsersQuery).ToList();
            return UserInfo;
        }
        public Users? FindUserByName(string userName)
        {
            using var connection = _context.CreateConnection();
            var user = connection.QueryFirstOrDefault<Users?>(Constants.QueryConstants.GetUserByUserNameQuery, new { userName });
            return user ?? null;
        }
        public Users? GetUserById(int userId)
        {
            using var connection = _context.CreateConnection();
            var user = connection.QueryFirstOrDefault<Users?>(Constants.QueryConstants.GetUserByUserIdQuery, new { userId });
            return user ?? null;
        }
        public bool Save(Users? user)
        {
            using var connection = _context.CreateConnection();
            if (user != null)
            {
                user.PasswordSalt = _userInfo.GenerateSalt();
                user.PasswordHash = _userInfo.HashPassword(user.Password, user.PasswordSalt);
                user.Password = string.Empty;
                user.Id = Guid.NewGuid();
            }

            user = connection.QueryFirstOrDefault<Users?>(Constants.QueryConstants.SaveUserQuery, new { user.UserName, user.Company, user.Comments, user.Id, user.PasswordHash, user.PasswordSalt, user.IsAdmin, user.InvalidLoginAttempts, user.UserAccountDisabled });
            return true;
        }

        public bool Update(Users? user)
        {
            using var connection = _context.CreateConnection();
            user = connection.QueryFirstOrDefault<Users?>(Constants.QueryConstants.UpdateUserQuery, new { user.UserId, user.UserName, user.Company, user.Comments, user.InvalidLoginAttempts, user.UserAccountDisabled });
            return true;
        }

        public bool Delete(Users? user)
        {
            using var connection = _context.CreateConnection();
            user = connection.QueryFirstOrDefault<Users?>(Constants.QueryConstants.DeleteUserQuery, new { user.UserId });
            return true;
        }
    }
}

