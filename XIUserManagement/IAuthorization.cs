using Interfleet.XaltAuthenticationAPI.Models;
using Interfleet.XIUserManagement.Models;



namespace Interfleet.XaltAuthenticationAPI
{
	public interface IAuthorization
	{
		bool Authorize(Token userToken, string role);

		IEnumerable<Role> GetRolesForUser(string userName);
	}
}
