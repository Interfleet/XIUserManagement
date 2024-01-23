using Interfleet.XIUserManagement.Models;


namespace Interfleet.XaltAuthenticationAPI.Models
{
 	[Serializable]
	public class Token : IToken, IEquatable<Token>
	{
		private readonly Guid _inner;

		[Obsolete("for serialization")]
		protected Token()
		{
		}

		public Token(Guid inner)
		{
			_inner = inner;
		}

		public string Value
		{
			get { return _inner.ToString(); }
		}

		public static IToken EmptyToken
		{
			get { return new Token(Guid.Empty); }
		}

		public bool Equals(Token other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other._inner.Equals(_inner);
		}

	}
}
