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
        public const string userNotFoundMessage = "User with id{0} not found";
        public const string dataNotUpdatedMessage = "Unable to update user details!";
        public const string dataUpdatedMessage = "User details saved successfully!";
        public const string dataNotDeletedMessage = "Unable to delete user details!";
        public const string dataDeletedMessage = "User details deleted successfully!";
        public const string cacheKey = "userList";
        public const string userIndex = "User_Index";
        public const string adminIndex = "Admin_Index";
        public const string userListEmpty = "User list is empty!";
        public const string searchValueEntry = "Please enter search value";
        public const string searchValueOption1 = "username";
        public const string searchValueOption2 = "company";
        public const string searchBy = "searchBy";
        public const string searchValue = "searchValue";
        public const string createUserMessage = "First create the user before trying to login with that user.";
        public const string errorInLoginMessage = "Can not login with a non existing user";
        public const string wrongUnamePwdMessage = "Username or password may be wrong";
        public const string authorizationFailedMessage = "Authorization failed. Required role {0}";
        public const string invalidUserMessage = "Invalid User!";
        public const string authorizeRoleValue = "deny";
    }
}
