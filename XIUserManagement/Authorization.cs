using Interfleet.XaltAuthenticationAPI.Models;
using Interfleet.XIUserManagement.Constants;
using Interfleet.XIUserManagement.Models;


namespace Interfleet.XaltAuthenticationAPI
{
    public class Authorization : IAuthorization
    {
        bool IAuthorization.Authorize(Token token, string role)
        {
            return role != UserMessageConstants.authorizeRoleValue;
        }

        public IEnumerable<Role> GetRolesForUser(string userName)
        {
            return new Role[0];
        }
    }
}
