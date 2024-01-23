
namespace Interfleet.XIUserManagement.Constants
{
    public static class Constants
    {
        public const string GetAllUsersQuery = @"Select UserId,userName,Password,Company,Comments,PasswordHash,PasswordSalt,Id,IsAdmin from tblUsers";
        public const string GetUserByUserNameQuery = @"SELECT userId,username,company,comments,passwordHash,passwordSalt,Id,IsAdmin FROM tblUsers where userName=@userName";
        public const string GetUserByUserIdQuery = @"SELECT userId,username,company,comments,passwordHash,passwordSalt,Id,IsAdmin FROM tblUsers where UserId=@userid";
        public const string SaveUserQuery = @"Insert into tblUsers(UserName,Id,Company,Comments,PasswordHash,PasswordSalt,IsAdmin) values(@userName, @Id,@company, @comments,@passwordHash,@passwordSalt,@IsAdmin)";
        public const string UpdateUserQuery = @"Update tblUsers set UserName = @userName,Company=@company,Comments=@comments where UserId=@userId";
        public const string DeleteUserQuery = @"Delete from tblUsers where UserId=@userId";

        public static readonly string[] RequiredRoles = { "User", "Admin" };
    }
}
