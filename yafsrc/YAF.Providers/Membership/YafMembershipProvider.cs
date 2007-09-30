using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration.Provider;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Globalization;

namespace YAF.Providers.Membership
{
    // YafMembershipProvider
    class YafMembershipProvider : MembershipProvider
    {
        string _appName, _passwordStrengthRegularExpression;
        int _minimumRequiredPasswordLength, _minRequiredNonAlphanumericCharacters;
        int _maxInvalidPasswordAttempts, _passwordAttemptWindow;
        bool _enablePaswordReset, _enablePasswordRetrieval, _requiresQuestionAndAnswer;
        bool _requiresUniqueEmail;
        Security.MembershipPasswordFormat _passwordFormat;

        #region PrivateMethods

        private const int PASSWORDSIZE = 14;

        private static string GenerateSalt()
        {
            byte[] buf = new byte[16];
            RNGCryptoServiceProvider rngCryptoSP = new RNGCryptoServiceProvider();
            rngCryptoSP.GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        private static string GeneratePassword(int minPassLength, int minNonAlphas)
        {
            return System.Web.Security.Membership.GeneratePassword(minPassLength < PASSWORDSIZE ? PASSWORDSIZE : minPassLength, minNonAlphas);
        }

        internal static string EncryptString(string unencryptedString, object hastMethod, string salt)
        {
            // Encrypt password accord to settings
            int hashMethod = (int)hastMethod;
            byte[] bIn = Encoding.Unicode.GetBytes(unencryptedString);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            switch (hashMethod)
            {
                case 0:
                    return unencryptedString; // Clear
                case 1:
                    return FormsAuthentication.HashPasswordForStoringInConfigFile(unencryptedString, "MD5"); // MD5 Unsalted (YAF Default)
                case 2:
                    return Convert.ToBase64String(HashAlgorithm.Create(System.Web.Security.Membership.HashAlgorithmType).ComputeHash(bAll)); // SHA1 Hash
                case 3:
                    return FormsAuthentication.HashPasswordForStoringInConfigFile(unencryptedString, "SHA2"); // SHA256
                default:
                    return Convert.ToBase64String(new YafMembershipProvider().EncryptPassword(bAll));
            }
        }

        private static string DecryptString(string pass, int passwordFormat)
        {
            switch (passwordFormat)
            {
                case 0: // MembershipPasswordFormat.Clear:
                    return pass;
                case 1: // MembershipPasswordFormat.Hashed:
                    throw new ProviderException(SR.GetString(SR.Provider_can_not_decode_hashed_password));
                case 4:
                    throw new ProviderException(SR.GetString(SR.Provider_can_not_decode_hashed_password));
                default:
                    byte[] bIn = Convert.FromBase64String(pass);
                    byte[] bRet = (new YafMembershipProvider()).DecryptPassword(bIn);
                    if (bRet == null)
                        return null;
                    return Encoding.Unicode.GetString(bRet, 16, bRet.Length - 16);
            }
        }

        private static bool IsPasswordCompliant(string password, int minLength, int minNonAlphaNumerics, string strengthRegEx)
        {
            // Check password meets minimum length criteria.
            if (!(password.Length >= minLength))
                return false;

            // Count Non alphanumerics
            int symbolCount = 1;
            foreach (char checkChar in password.ToCharArray())
            {
                if (!(char.IsLetterOrDigit(checkChar)))
                    symbolCount++;
            }
            // Check password meets minimum alphanumeric criteria
            if (!(symbolCount >= minNonAlphaNumerics))
                return false;



            // Check password strength meets Password Strength Regex Requirements
            if (!(Regex.IsMatch(password, strengthRegEx)))
                return false;

            // Check string meets requirements as set in config
            return true;
        }

        private bool IsPasswordCompliant(string passsword)
        {
            return YafMembershipProvider.IsPasswordCompliant(passsword, this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters, this.PasswordStrengthRegularExpression);
        }
        #endregion

        #region Override Public Properties

        public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePaswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minimumRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        private Security.MembershipPasswordFormat PasswordFormatInternal
        {
            get { return _passwordFormat; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        #endregion

        #region Overriden Public Methods
        public override void Initialize(string name, NameValueCollection config)
        {
            // Retrieve information for provider from web config
            // config ints

            // Minimum Required Password Length from Provider configuration
            if (config["minRequiredPasswordLength"] != null)
                _minimumRequiredPasswordLength = int.Parse(config["minRequiredPasswordLength"]);

            // Minimum Required Non Alpha-numeric Characters from Provider configuration
            if (config["minRequiredNonalphanumericCharacters"] != null)
                _minRequiredNonAlphanumericCharacters = int.Parse(config["_minRequiredNonalphanumericCharacters"]);

            // Maximum number of allowed password attempts
            if (config["maxInvalidPasswordAttempts"] != null)
                _maxInvalidPasswordAttempts = int.Parse(config["maxInvalidPasswordAttempts"]);

            // Password Attempt Window when maximum attempts have been reached
            if (config["passwordAttemptWindow"] != null)
                _passwordAttemptWindow = int.Parse(config["passwordAttemptWindow"]);

            // Application Name
            if (config["appName"] != null)
                _appName = config["appName"];
            if (config["passwordStrengthRegularExpression"] != null)
                _passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];


            // Password reset enabled from Provider Configuration
            if (config["enablePaswordReset"] != null)
                _enablePaswordReset = bool.Parse(config["enablePaswordReset"]);
            if (config["enablePasswordRetrieval"] != null)
                _enablePasswordRetrieval = bool.Parse(config["enablePasswordRetrieval"]);
            if (config["requiresQuestionAndAnswer"] != null)
                _requiresQuestionAndAnswer = bool.Parse(config["requiresQuestionAndAnswer"]);
            if (config["requiresUniqueEmail"] != null)
                _requiresUniqueEmail = bool.Parse(config["requiresUniqueEmail"]);


            string strTemp = config["passwordFormat"];
            if (strTemp == null)
                strTemp = "Hashed";

            switch (strTemp)
            {
                case "Clear":
                    _passwordFormat = Security.MembershipPasswordFormat.Clear;
                    break;
                case "Encrypted":
                    _passwordFormat = Security.MembershipPasswordFormat.Encrypted;
                    break;
                case "MD5Hashed":
                    _passwordFormat = Security.MembershipPasswordFormat.MD5Hashed;
                    break;
                case "SHA1Hashed":
                    _passwordFormat = Security.MembershipPasswordFormat.SHA1Hashed;
                    break;
                case "SHA2Hashed":
                    _passwordFormat = Security.MembershipPasswordFormat.SHA2Hashed;
                    break;
                default:
                    throw new ProviderException(SR.GetString(SR.Provider_bad_password_format));
            }

        }


        /// <summary>
        /// Change Users password
        /// </summary>
        /// <param name="username">Username to change password for</param>
        /// <param name="oldpassword">Password</param>
        /// <param name="newPassword">New question</param>
        /// <returns> Boolean depending on whether the change was successful</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            // Clean input

            // Check password meets requirements as set by Configuration settings
            if (!(this.IsPasswordCompliant(newPassword)))
                return false;

            if (!(new UserPasswordInfo(this.ApplicationName, username, false).IsCorrectPassword(oldPassword)))
                return false;

            // Call SQL Password  Change
            DB.ChangePassword(this.ApplicationName, username, newPassword);

            // Return True
            return true;
        }


        /// <summary>
        /// Change Users password Question and Answer
        /// </summary>
        /// <param name="username">Username to change Q&A for</param>
        /// <param name="password">Password</param>
        /// <param name="newPasswordQuestion">New question</param>
        /// <param name="newPasswordAnswer">New answer</param>
        /// <returns> Boolean depending on whether the change was successful</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {

            // Check arguments for null values
            if ((username == null) || (password == null) || (newPasswordQuestion == null) || (newPasswordAnswer == null))
                throw new ArgumentException("Username, Password, Password Question or Password Answer cannot be null");

            UserPasswordInfo currentPasswordInfo = new UserPasswordInfo(this.ApplicationName, username, false);

            newPasswordAnswer = YafMembershipProvider.EncryptString(newPasswordAnswer, currentPasswordInfo.PasswordFormat, currentPasswordInfo.PasswordSalt);

            if (currentPasswordInfo.IsCorrectPassword(password))
            {
                try
                {
                    DB.ChangePasswordQuestionAndAnswer(this.ApplicationName, username, newPasswordQuestion, newPasswordAnswer);
                }
                catch (Exception e)
                {
                    return false; // Exception raised return false
                }
                return true;
            }
            else
            {
                return false; // Invalid password return false
            }
        }

        /// <summary>
        /// Create user and add to provider
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email Address</param>
        /// <param name="passwordQuestion">Password Question</param>
        /// <param name="passwordAnswer">Password Answer - used for password retrievals.</param>
        /// <param name="isApproved">Is the User approved?</param>
        /// <param name="providerUserKey">Provider User Key to identify the User</param>
        /// <param name="status">Out - MembershipCreateStatus object containing status of the Create User process</param>
        /// <returns> Boolean depending on whether the deletion was successful</returns>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            // Check password meets requirements as set out in the web.config
            if (!(this.IsPasswordCompliant(password)))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string salt = YafMembershipProvider.GenerateSalt();

            string pass = YafMembershipProvider.EncryptString(password, (int)this.PasswordFormat, salt);

            // Encode Password Answer
            string encodedPasswordAnswer = YafMembershipProvider.EncryptString(passwordAnswer.ToLower(CultureInfo.InvariantCulture), this.PasswordFormat, salt);

            // Process database user creation request
            DB.CreateUser(this.ApplicationName, username, pass,salt,(int) this.PasswordFormat, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey);

            status = MembershipCreateStatus.Success;

            return this.GetUser(username, false);
        }

        /// <summary>
        /// Delete User and User's information from provider
        /// </summary>
        /// <param name="username">Username to delete</param>
        /// <param name="deleteAllRelatedData">Delete all related daata</param>
        /// <returns> Boolean depending on whether the deletion was successful</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            // Check username argument is not null
            if (username == null)
                throw new ArgumentNullException("Username is null");

            // Process database user deletion request
            try
            {
                DB.DeleteUser(this.ApplicationName, username, deleteAllRelatedData);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves all users into a MembershupUserCollection where Email Matches
        /// </summary>
        /// <param name="emailToMatch">Email use as filter criteria</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="userIsOnline">How many records to the page</param>
        /// <param name="totalRecords">Out - Number of records held</param>
        /// <returns>MembershipUser Collection</returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            if (pageIndex < 0)
                throw new ArgumentException(SR.GetString(SR.PageIndex_bad), "pageIndex");
            if (pageSize < 1)
                throw new ArgumentException(SR.GetString(SR.PageSize_bad), "pageSize");

            // Loop through all users
            foreach (DataRow dr in DB.FindUsersByEmail(this.ApplicationName, emailToMatch, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(new MembershipUser(this.Name, dr["Username"].ToString(), dr["MemberShipPK"].ToString(), dr["EmailAddress"].ToString(), dr["PasswordQuestion"].ToString(), dr["Comment"].ToString(), bool.Parse(dr["IsAppoved"].ToString()), bool.Parse(dr["IsLockedOut"].ToString()), DateTime.Parse(dr["CreationDate"].ToString()), DateTime.Parse(dr["LastLogin"].ToString()), DateTime.Parse(dr["LastActivity"].ToString()), DateTime.Parse(dr["LastPasswordChange"].ToString()), DateTime.Parse(dr["LastLockout"].ToString())));
            }
            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Retrieves all users into a MembershupUserCollection where Username matches
        /// </summary>
        /// <param name="usernameToMatch">Username use as filter criteria</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="userIsOnline">How many records to the page</param>
        /// <param name="totalRecords">Out - Number of records held</param>
        /// <returns>MembershipUser Collection</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            if (pageIndex < 0)
                throw new ArgumentException(SR.GetString(SR.PageIndex_bad), "pageIndex");
            if (pageSize < 1)
                throw new ArgumentException(SR.GetString(SR.PageSize_bad), "pageSize");

            // Loop through all users
            foreach (DataRow dr in DB.FindUsersByName(this.ApplicationName, usernameToMatch, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(new MembershipUser(this.Name, dr["Username"].ToString(), dr["MemberShipPK"].ToString(), dr["EmailAddress"].ToString(), dr["PasswordQuestion"].ToString(), dr["Comment"].ToString(), bool.Parse(dr["IsAppoved"].ToString()), bool.Parse(dr["IsLockedOut"].ToString()), DateTime.Parse(dr["CreationDate"].ToString()), DateTime.Parse(dr["LastLogin"].ToString()), DateTime.Parse(dr["LastActivity"].ToString()), DateTime.Parse(dr["LastPasswordChange"].ToString()), DateTime.Parse(dr["LastLockout"].ToString())));
            }
            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Retrieves all users into a MembershupUserCollection
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="userIsOnline">How many records to the page</param>
        /// <param name="totalRecords">Out - Number of records held</param>
        /// <returns>MembershipUser Collection</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            if (pageIndex < 0)
                throw new ArgumentException(SR.GetString(SR.PageIndex_bad), "pageIndex");
            if (pageSize < 1)
                throw new ArgumentException(SR.GetString(SR.PageSize_bad), "pageSize");

            // Loop through all users
            foreach (DataRow dr in DB.GetAllUsers(this.ApplicationName, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(new MembershipUser(this.Name, dr["Username"].ToString(), dr["MemberShipPK"].ToString(), dr["EmailAddress"].ToString(), dr["PasswordQuestion"].ToString(), dr["Comment"].ToString(), bool.Parse(dr["IsAppoved"].ToString()), bool.Parse(dr["IsLockedOut"].ToString()), DateTime.Parse(dr["CreationDate"].ToString()), DateTime.Parse(dr["LastLogin"].ToString()), DateTime.Parse(dr["LastActivity"].ToString()), DateTime.Parse(dr["LastPasswordChange"].ToString()), DateTime.Parse(dr["LastLockout"].ToString())));
            }
            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Retrieves the number of users currently online for this application
        /// </summary>
        /// <returns>Number of users online</returns>
        public override int GetNumberOfUsersOnline()
        {
            DataRow dr = DB.GetNumberOfUsersOnline(this.ApplicationName, System.Web.Security.Membership.UserIsOnlineTimeWindow);

            if (!(dr == null))
                return (int)dr["totalusersonline"];
            else
                return 0;
        }


        /// <summary>
        /// Retrieves the Users password (if EnablePasswordRetrieval is true)
        /// </summary>
        /// <param name="username">Username to retrieve password for</param>
        /// <param name="answer">Answer to the Users Membership Question</param>
        /// <param name="newPasswordQuestion">New question</param>
        /// <param name="newPasswordAnswer">New answer</param>
        /// <returns> Password unencrypted</returns>
        public override string GetPassword(string username, string answer)
        {
            if (!this.EnablePasswordRetrieval)
            {
                throw new NotSupportedException(SR.GetString(SR.Membership_PasswordRetrieval_not_supported));
            }

            // Check for null arguments
            if ((username == null) || (answer == null))
                throw new ArgumentException("Username or Password answer cannot be null");

            UserPasswordInfo currentPasswordInfo = new UserPasswordInfo(this.ApplicationName, username, false);
            if (currentPasswordInfo.IsCorrectAnswer(answer))
                return YafMembershipProvider.DecryptString(currentPasswordInfo.Password, currentPasswordInfo.PasswordFormat);
            else
                return null;
        }

        /// <summary>
        /// Retrieves a MembershipUser object from the criteria given
        /// </summary>
        /// <param name="username">Username to be foundr</param>
        /// <param name="userIsOnline">Is the User currently online</param>
        /// <returns>MembershipUser object</returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (username == null)
                throw new ArgumentNullException("username is null");

            DataRow dr = DB.GetUser(this.ApplicationName, username, userIsOnline);
            if (dr == null)
                return new MembershipUser(this.Name, dr["Username"].ToString(), dr["MemberShipPK"].ToString(), dr["EmailAddress"].ToString(), dr["PasswordQuestion"].ToString(), dr["Comment"].ToString(), bool.Parse(dr["IsAppoved"].ToString()), bool.Parse(dr["IsLockedOut"].ToString()), DateTime.Parse(dr["CreationDate"].ToString()), DateTime.Parse(dr["LastLogin"].ToString()), DateTime.Parse(dr["LastActivity"].ToString()), DateTime.Parse(dr["LastPasswordChange"].ToString()), DateTime.Parse(dr["LastLockout"].ToString()));
            else
                return null;
        }

        /// <summary>
        /// Retrieves a MembershipUser object from the criteria given
        /// </summary>
        /// <param name="providerUserKey">User to be found based on UserKey</param>
        /// <param name="userIsOnline">Is the User currently online</param>
        /// <returns>MembershipUser object</returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            if (!(providerUserKey is Guid))
            {
                throw new ArgumentException(SR.GetString(SR.Membership_InvalidProviderUserKey), "providerUserKey");
            }

            DataRow dr = DB.GetUser(this.ApplicationName, providerUserKey, userIsOnline);
            if (dr == null)
                return new MembershipUser(this.Name, dr["Username"].ToString(), dr["MemberShipPK"].ToString(), dr["EmailAddress"].ToString(), dr["PasswordQuestion"].ToString(), dr["Comment"].ToString(), bool.Parse(dr["IsAppoved"].ToString()), bool.Parse(dr["IsLockedOut"].ToString()), DateTime.Parse(dr["CreationDate"].ToString()), DateTime.Parse(dr["LastLogin"].ToString()), DateTime.Parse(dr["LastActivity"].ToString()), DateTime.Parse(dr["LastPasswordChange"].ToString()), DateTime.Parse(dr["LastLockout"].ToString()));
            else
                return null;
        }


        /// <summary>
        /// Retrieves a MembershipUser object from the criteria given
        /// </summary>
        /// <param name="providerUserKey">User to be found based on UserKey</param>
        /// <param name="userIsOnline">Is the User currently online</param>
        /// <returns>Username as string</returns>
        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
                throw new ArgumentNullException("Email is null");

            DataTable Users = DB.GetUserNameByEmail(this.ApplicationName, email);
            if (this.RequiresUniqueEmail && Users.Rows.Count > 1)
                throw new ProviderException("More than one username returned for email address");
            if (Users.Rows.Count == 0)
                return null;
            else
                return Users.Rows[0]["Username"].ToString();
        }

        /// <summary>
        /// Reset a users password - *
        /// </summary>
        /// <param name="username">User to be found based by Name</param>
        /// <param name="answer">Verifcation that it is them</param>
        /// <returns>Username as string</returns>
        public override string ResetPassword(string username, string answer)
        {
            string newPassword, newPasswordEnc, newPasswordSalt, newPasswordAnswer;

            /// Check Password reset is enabled
            if (!(this.EnablePasswordReset))
                throw new NotSupportedException("Reset passwords are not supported with this configuration");

            // Check arguments for null values
            if ((username == null) || (answer == null))
                throw new ArgumentException("Username or Password answer cannot be null");

            UserPasswordInfo currentPasswordInfo = new UserPasswordInfo(this.ApplicationName, username, false);

            if (currentPasswordInfo.IsCorrectAnswer(answer))
            {
                newPasswordSalt = YafMembershipProvider.GenerateSalt();
                newPasswordAnswer = YafMembershipProvider.EncryptString(answer, this.PasswordFormat, newPasswordSalt);
                newPassword = YafMembershipProvider.GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);
                newPasswordEnc = YafMembershipProvider.EncryptString(newPassword, this.PasswordFormat, newPasswordSalt);
                DB.ResetPassword(this.ApplicationName, username, newPasswordEnc, newPasswordSalt, (int)this.PasswordFormat, this.MaxInvalidPasswordAttempts, this.PasswordAttemptWindow);
                return newPassword; // Return unencrypted password
            }
            else
                return null;

        }

        /// <summary>
        /// Unlocks a users account
        /// </summary>
        /// <param name="username">User to be found based by Name</param>
        /// <returns>True/False is users account has been unlocked</returns>
        public override bool UnlockUser(string userName)
        {
            // Check for null argument
            if (userName == null)
                throw new ArgumentNullException("userName cannot be null");


            try { DB.UnlockUser(this.ApplicationName, userName); }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Updates a providers user information
        /// </summary>
        /// <param name="user">MembershipUser object</param>
        public override void UpdateUser(MembershipUser user)
        {
            if (user == null)
                throw new ArgumentNullException("User is null.");
            DB.UpdateUser(this.ApplicationName, user, this.RequiresUniqueEmail);
        }

        /// <summary>
        /// Validates a user by user name / password
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="userName">Password</param>
        /// /// <returns>True/False whether username/password match what is on database.</returns>
        public override bool ValidateUser(string username, string password)
        {
            return (new UserPasswordInfo(this.ApplicationName, username, false).IsCorrectPassword(password));
        }
        #endregion
    }
}
