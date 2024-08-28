
namespace Interfleet.XIUserManagement.Constants
{
    public static class QueryConstants
    {
        public const string GetAllUsersQuery = @"Select UserId,userName,Password,Company,Comments,PasswordHash,PasswordSalt,Id,IsAdmin,InvalidLoginAttempts,UserAccountDisabled from tblUsers order by UserName";
        public const string GetUserByUserNameQuery = @"SELECT userId,username,company,comments,passwordHash,passwordSalt,Id,IsAdmin,InvalidLoginAttempts,UserAccountDisabled FROM tblUsers where userName=@userName order by UserName";
        public const string GetUserByUserIdQuery = @"SELECT userId,username,company,comments,passwordHash,passwordSalt,Id,IsAdmin,InvalidLoginAttempts,UserAccountDisabled FROM tblUsers where UserId=@userid order by UserName";
        public const string SaveUserQuery = @"Insert into tblUsers(UserName,Id,Company,Comments,PasswordHash,PasswordSalt,IsAdmin,InvalidLoginAttempts,UserAccountDisabled) values(@userName, @Id,@company, @comments,@passwordHash,@passwordSalt,@IsAdmin,@InvalidLoginAttempts,@UserAccountDisabled)";
        public const string UpdateUserQuery = @"Update tblUsers set UserName = @userName,Company=@company,Comments=@comments,IsAdmin=@isadmin,InvalidLoginAttempts=@invalidLoginAttempts,UserAccountDisabled=@userAccountDisabled where UserId=@userId";
        public const string ResetPasswordQuery = @"Update tblUsers set PasswordHash = @passwordHash,PasswordSalt=@passwordSalt where UserId=@userId";
        public const string DeleteUserQuery = @"Delete from tblUsers where UserId=@userId";

        public static readonly string[] RequiredRoles = { "User", "Admin" };
    }
}
