using System;
using System.Runtime.Serialization;

namespace Interfleet.XIUserManagement.Exceptions
{

    /// <summary>
    /// Exception thrown when gotten a command for a user that doesn't exist.
    /// </summary>
    [Serializable]
    public sealed class UserNotFoundException : Exception
    {
        private readonly Guid _userId;

        public UserNotFoundException(Guid userId)
            : base(string.Format("User with id#{0} could not be found", userId))
        {
            _userId = userId;
        }

        public UserNotFoundException(Guid userId, string message)
            : base(message)
        {
            _userId = userId;
        }

        public UserNotFoundException(Guid userId, string message, Exception innerException)
            : base(message, innerException)
        {
            _userId = userId;
        }

        public UserNotFoundException(string message)
            : base(message)
        {
        }

        private UserNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _userId = (Guid)info.GetValue("_userId", typeof(Guid));
        }

        /// <summary>
        /// Gets the user id that was not found.
        /// </summary>
        public Guid UserId
        {
            get { return _userId; }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_userId", _userId);
        }
    }
}
