using Interfleet.XIUserManagement.Domain;
using System;


namespace Interfleet.XaltAuthenticationAPI.Models
{
	public class Role : EntityBase<int>
	{
		[Obsolete("for NHibernate")]
		protected Role()
		{
		}

		public Role(string name)
		{
			Name = name;
		}

		public virtual string Name { get; protected set; }

		public void UpdateRole(string name)
		{
			Name = name;
		}
	}
}
