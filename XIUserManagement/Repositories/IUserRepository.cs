using Interfleet.XIUserManagement.Models;



namespace Interfleet.XIUserManagement.Repositories
{
    public interface IUserRepository
    {
        List<Users> GetUsers();
        Users FindUserByName(string userName);
        Users GetUserById(int userId);
        bool Save(Users user);
        bool Update(Users user);
        bool ResetPassword(ResetPasswordModel? resetPasswordModel);
        bool Delete(Users user);
    }
}
