namespace Interfleet.XIUserManagement.Constants
{
    public class UserMessageConstants
    {
        public const string passwordComparatorMessage = "Password and confirm password do not match, please try again !";
        public const string passwordValidatorMessage = "Password must be a minimum of 8 characters and a combination of one uppercase,one lowercase,one special character and one digit";
        public const string passwordRegularExpression = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$";
        public const string modelInvalidDataMessage = "Model data is invalid!";
        public const string dataNotSavedMessage = "Unable to save user details!";
        public const string dataSavedMessage = "User details saved successfully!";
        public const string userNotFoundMessage = "User not found!";
        public const string dataNotUpdatedMessage = "Unable to update user details!";
        public const string dataUpdatedMessage = "User details uodated successfully!";
        public const string passwordResetMessage = "Your password has been reset successfully!";
        public const string dataNotDeletedMessage = "Unable to delete user details!";
        public const string dataDeletedMessage = "User details deleted successfully!";
        public const string userExistsMessage = "User already exists. Please enter some other username";
        public const string userAccountDisabledMessage = "Your account has been locked due to too many invalid login attempts, please contact administrator to unlock!!";
        public const string cacheKey = "userList";
        public const string userIndex = "User_Index";
        public const string adminIndex = "Admin_Index";
        public const string userView = "User_View";
        public const string adminView = "Admin_View";
        public const string index = "Index";
        public const string user = "user";
        public const string userId = "userId";
        public const string userName = "UserName";
        public const string create = "create";
        public const string edit = "edit";
        public const string delete = "delete";
        public const string userListEmpty = "User list is empty!";
        public const string searchValueEntry = "Please enter search value";
        public const string searchValueOption1 = "username";
        public const string searchValueOption2 = "company";
        public const string searchBy = "searchBy";
        public const string searchValue = "searchValue";
        public const string createUserMessage = "First create the user before trying to login with that user.";
        public const string errorInLoginMessage = "Can not login with a non existing user";
        public const string wrongPwdMessage = "You have entered an invalid password!!";
        public const string authorizationFailedMessage = "Authorization failed. Required role {0}";
        public const string invalidUserMessage = "Invalid User!";
        public const string authorizeRoleValue = "deny";
        public const string accountUnlockMessage = "User account has been locked,are you sure you want to unlock the account?";
        public const string accountUnlockSuccessMessage = "Account has been unlocked for ";
        public const string deleteUserMessage = "Are you sure you want to delete this record?";
    }
}
