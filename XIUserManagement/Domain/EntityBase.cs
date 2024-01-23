using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interfleet.XIUserManagement.Domain
{
	public class EntityBase<TId> : IEquatable<EntityBase<TId>>
	{
		public virtual TId Id { get; protected set; }

		public virtual bool Equals(EntityBase<TId> other)
		{
			if (other == null) return false;

			var otherIsTransient = Equals(other.Id, default(TId));
			var thisIsTransient = Equals(Id, default(TId));

			if (otherIsTransient && thisIsTransient)
				return ReferenceEquals(other, this);

			return other.Id.Equals(Id);
		}

		public override bool Equals(object obj)
		{
			var other = obj as EntityBase<TId>;
			return other != null && Equals(other);
		}

		private int? _oldHashCode;

		public override int GetHashCode()
		{
			// Once we have a hash code we'll never change it
			if (_oldHashCode.HasValue)
				return _oldHashCode.Value;

			var thisIsTransient = Equals(Id, default(TId));

			// When this instance is transient, we use the base GetHashCode()
			// and remember it, so an instance can NEVER change its hash code.
			if (thisIsTransient)
			{
				_oldHashCode = base.GetHashCode();
				return _oldHashCode.Value;
			}

			return Id.GetHashCode();
		}
	}
}
