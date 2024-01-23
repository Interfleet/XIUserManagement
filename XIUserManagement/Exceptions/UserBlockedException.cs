using System;


namespace Interfleet.XIUserManagement.Exceptions
{
	public class UserBlockedException : Exception
	{
		public UserBlockedException()
		{
		}
		public UserBlockedException(string message)
			: base(message)
		{
		}
		public UserBlockedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
