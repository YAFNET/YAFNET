/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Providers.Membership
{
    using System;
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Security;

    using YAF.Providers.Utils;
    using YAF.Types.Extensions;

    /// <summary>
    ///     The yaf membership provider.
    /// </summary>
    public class YafMembershipProvider : MembershipProvider
    {
        #region Constants

        /// <summary>
        ///     The _passwordsize.
        /// </summary>
        private const int _passwordsize = 14;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The conn str app key name.
        /// </summary>
        public static string ConnStrAppKeyName = "YafMembershipConnectionString";

        #endregion

        // Instance Variables

        #region Fields

        /// <summary>
        ///     The _app name.
        /// </summary>
        private string _appName;

        /// <summary>
        ///     The _conn str name.
        /// </summary>
        private string _connStrName;

        /// <summary>
        ///     The _enable password reset.
        /// </summary>
        private bool _enablePasswordReset;

        /// <summary>
        ///     The _enable password retrieval.
        /// </summary>
        private bool _enablePasswordRetrieval;

        /// <summary>
        ///     The _hash case.
        /// </summary>
        private string _hashCase;

        /// <summary>
        ///     The _hash hex.
        /// </summary>
        private bool _hashHex;

        /// <summary>
        ///     The _hash remove chars.
        /// </summary>
        private string _hashRemoveChars;

        /// <summary>
        ///     The _max invalid password attempts.
        /// </summary>
        private int _maxInvalidPasswordAttempts;

        /// <summary>
        ///     The _min required non alphanumeric characters.
        /// </summary>
        private int _minRequiredNonAlphanumericCharacters;

        /// <summary>
        ///     The _minimum required password length.
        /// </summary>
        private int _minimumRequiredPasswordLength;

        /// <summary>
        ///     The _ms compliant.
        /// </summary>
        private bool _msCompliant;

        /// <summary>
        ///     The _password attempt window.
        /// </summary>
        private int _passwordAttemptWindow;

        /// <summary>
        ///     The _password format.
        /// </summary>
        private MembershipPasswordFormat _passwordFormat;

        /// <summary>
        ///     The _password strength regular expression.
        /// </summary>
        private string _passwordStrengthRegularExpression;

        /// <summary>
        ///     The _requires question and answer.
        /// </summary>
        private bool _requiresQuestionAndAnswer;

        /// <summary>
        ///     The _requires unique email.
        /// </summary>
        private bool _requiresUniqueEmail;

        /// <summary>
        ///     The _use salt.
        /// </summary>
        private bool _useSalt;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets ApplicationName.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this._appName;
            }

            set
            {
                if (value != this._appName)
                {
                    this._appName = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets provider Description. The standard string used to identify provider as a build-in one.
        /// </summary>
        public override string Description
        {
            get
            {
                return "YAF Native Membership Provider";
            }
        }

        /// <summary>
        ///     Gets a value indicating whether EnablePasswordReset.
        /// </summary>
        public override bool EnablePasswordReset
        {
            get
            {
                return this._enablePasswordReset;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether EnablePasswordRetrieval.
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return this._enablePasswordRetrieval;
            }
        }

        /// <summary>
        ///     Gets MaxInvalidPasswordAttempts.
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this._maxInvalidPasswordAttempts;
            }
        }

        /// <summary>
        ///     Gets MinRequiredNonAlphanumericCharacters.
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return this._minRequiredNonAlphanumericCharacters;
            }
        }

        /// <summary>
        ///     Gets MinRequiredPasswordLength.
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return this._minimumRequiredPasswordLength;
            }
        }

        /// <summary>
        ///     Gets PasswordAttemptWindow.
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get
            {
                return this._passwordAttemptWindow;
            }
        }

        /// <summary>
        ///     Gets PasswordFormat.
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return this._passwordFormat;
            }
        }

        /// <summary>
        ///     Gets PasswordStrengthRegularExpression.
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return this._passwordStrengthRegularExpression;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether RequiresQuestionAndAnswer.
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return this._requiresQuestionAndAnswer;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether RequiresUniqueEmail.
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return this._requiresUniqueEmail;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets HashCase.
        /// </summary>
        internal string HashCase
        {
            get
            {
                return this._hashCase;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether HashHex.
        /// </summary>
        internal bool HashHex
        {
            get
            {
                return this._hashHex;
            }
        }

        /// <summary>
        ///     Gets HashRemoveChars.
        /// </summary>
        internal string HashRemoveChars
        {
            get
            {
                return this._hashRemoveChars;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether MSCompliant.
        /// </summary>
        internal bool MSCompliant
        {
            get
            {
                return this._msCompliant;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether UseSalt.
        /// </summary>
        internal bool UseSalt
        {
            get
            {
                return this._useSalt;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates a password buffer from salt and password ready for hashing/encrypting
        /// </summary>
        /// <param name="salt">
        ///     Salt to be applied to hashing algorithm
        /// </param>
        /// <param name="clearString">
        ///     Clear string to hash
        /// </param>
        /// <param name="standardComp">
        ///     Use Standard asp.net membership method of creating the buffer
        /// </param>
        /// <returns>
        ///     Salted Password as Byte Array
        /// </returns>
        public static byte[] GeneratePasswordBuffer(string salt, string clearString, bool standardComp)
        {
            byte[] unencodedBytes = Encoding.Unicode.GetBytes(clearString);
            byte[] saltBytes = Convert.FromBase64String(salt);
            var buffer = new byte[unencodedBytes.Length + saltBytes.Length];

            if (standardComp)
            {
                // Compliant with ASP.NET Membership method of hash/salt
                Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
                Buffer.BlockCopy(unencodedBytes, 0, buffer, saltBytes.Length, unencodedBytes.Length);
            }
            else
            {
                Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, buffer, unencodedBytes.Length - 1, saltBytes.Length);
            }

            return buffer;
        }

        /// <summary>
        ///     Hashes a clear string to the given hashtype
        /// </summary>
        /// <param name="clearString">
        ///     Clear string to hash
        /// </param>
        /// <param name="hashType">
        ///     hash Algorithm to be used
        /// </param>
        /// <param name="salt">
        ///     Salt to be applied to hashing algorithm
        /// </param>
        /// <param name="useSalt">
        ///     Should salt be applied to hashing algorithm
        /// </param>
        /// <param name="hashHex">
        ///     The hash Hex.
        /// </param>
        /// <param name="hashCase">
        ///     The hash Case.
        /// </param>
        /// <param name="hashRemoveChars">
        ///     The hash Remove Chars.
        /// </param>
        /// <param name="standardComp">
        ///     The standard Comp.
        /// </param>
        /// <returns>
        ///     Hashed String as Hex or Base64
        /// </returns>
        public static string Hash(
            string clearString,
            string hashType,
            string salt,
            bool useSalt,
            bool hashHex,
            string hashCase,
            string hashRemoveChars,
            bool standardComp)
        {
            byte[] buffer;
            if (useSalt)
            {
                buffer = GeneratePasswordBuffer(salt, clearString, standardComp);
            }
            else
            {
                byte[] unencodedBytes = Encoding.UTF8.GetBytes(clearString); // UTF8 used to maintain compatibility
                buffer = new byte[unencodedBytes.Length];
                Buffer.BlockCopy(unencodedBytes, 0, buffer, 0, unencodedBytes.Length);
            }

            byte[] hashedBytes = Hash(buffer, hashType); // Hash

            string hashedString;

            hashedString = hashHex ? BitBoolExtensions.ToHexString(hashedBytes) : Convert.ToBase64String(hashedBytes);

            // Adjust the case of the hash output
            switch (hashCase.ToLower())
            {
                case "upper":
                    hashedString = hashedString.ToUpper();
                    break;
                case "lower":
                    hashedString = hashedString.ToLower();
                    break;
            }

            if (hashRemoveChars.IsSet())
            {
                hashedString = hashRemoveChars.Aggregate(hashedString, (current, removeChar) => current.Replace(removeChar.ToString(), string.Empty));
            }

            return hashedString;
        }

        /// <summary>
        ///     Change Users password
        /// </summary>
        /// <param name="username">
        ///     Username to change password for
        /// </param>
        /// <param name="oldPassword">
        ///     The old Password.
        /// </param>
        /// <param name="newPassword">
        ///     New question
        /// </param>
        /// <returns>
        ///     Boolean depending on whether the change was successful
        /// </returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            string passwordSalt = string.Empty;
            string newEncPassword = string.Empty;

            // Clean input

            // Check password meets requirements as set by Configuration settings
            if (!this.IsPasswordCompliant(newPassword))
            {
                return false;
            }

            UserPasswordInfo currentPasswordInfo = UserPasswordInfo.CreateInstanceFromDB(
                this.ApplicationName,
                username,
                false,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            // validate the correct user information was found...
            if (currentPasswordInfo == null)
            {
                return false;
            }

            // validate the correct user password was entered...
            if (!currentPasswordInfo.IsCorrectPassword(oldPassword))
            {
                return false;
            }

            if (this.UseSalt)
            {
                // generate a salt if one doesn't exist...
                passwordSalt = currentPasswordInfo.PasswordSalt.IsNotSet()
                                   ? GenerateSalt()
                                   : currentPasswordInfo.PasswordSalt;
            }

            // encode new password
            newEncPassword = EncodeString(
                newPassword,
                (int)this.PasswordFormat,
                passwordSalt,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            // Call SQL Password to Change
            DB.Current.ChangePassword(
                this.ApplicationName,
                username,
                newEncPassword,
                passwordSalt,
                (int)this.PasswordFormat,
                currentPasswordInfo.PasswordAnswer);

            // Return True
            return true;
        }

        /// <summary>
        ///     The change password question and answer.
        /// </summary>
        /// <param name="username">
        ///     The username.
        /// </param>
        /// <param name="password">
        ///     The password.
        /// </param>
        /// <param name="newPasswordQuestion">
        ///     The new password question.
        /// </param>
        /// <param name="newPasswordAnswer">
        ///     The new password answer.
        /// </param>
        /// <returns>
        ///     The change password question and answer.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public override bool ChangePasswordQuestionAndAnswer(
            string username,
            string password,
            string newPasswordQuestion,
            string newPasswordAnswer)
        {
            // Check arguments for null values
            if ((username == null) || (password == null) || (newPasswordQuestion == null) || (newPasswordAnswer == null))
            {
                throw new ArgumentException("Username, Password, Password Question or Password Answer cannot be null");
            }

            UserPasswordInfo currentPasswordInfo = UserPasswordInfo.CreateInstanceFromDB(
                this.ApplicationName,
                username,
                false,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);
            newPasswordAnswer = EncodeString(
                newPasswordAnswer,
                currentPasswordInfo.PasswordFormat,
                currentPasswordInfo.PasswordSalt,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            if (currentPasswordInfo.IsCorrectPassword(password))
            {
                try
                {
                    DB.Current.ChangePasswordQuestionAndAnswer(
                        this.ApplicationName,
                        username,
                        newPasswordQuestion,
                        newPasswordAnswer);
                    return true;
                }
                catch
                {
                    // will return false...
                }
            }

            return false; // Invalid password return false
        }

        /// <summary>
        ///     Create user and add to provider
        /// </summary>
        /// <param name="username">
        ///     Username
        /// </param>
        /// <param name="password">
        ///     Password
        /// </param>
        /// <param name="email">
        ///     Email Address
        /// </param>
        /// <param name="passwordQuestion">
        ///     Password Question
        /// </param>
        /// <param name="passwordAnswer">
        ///     Password Answer - used for password retrievals.
        /// </param>
        /// <param name="isApproved">
        ///     Is the User approved?
        /// </param>
        /// <param name="providerUserKey">
        ///     Provider User Key to identify the User
        /// </param>
        /// <param name="status">
        ///     Out - MembershipCreateStatus object containing status of the Create User process
        /// </param>
        /// <returns>
        ///     Boolean depending on whether the deletion was successful
        /// </returns>
        public override MembershipUser CreateUser(
            string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {
            // ValidatePasswordEventArgs e = new ValidatePasswordEventArgs( username, password, true );
            // OnValidatingPassword( e );
            // if ( e.Cancel )
            // {
            // 	status = MembershipCreateStatus.InvalidPassword;
            // 	return null;
            // }
            string salt = string.Empty, pass = string.Empty;

            // Check password meets requirements as set out in the web.config
            if (!this.IsPasswordCompliant(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Check password Question and Answer requirements.
            if (this.RequiresQuestionAndAnswer)
            {
                if (passwordQuestion.IsNotSet())
                {
                    status = MembershipCreateStatus.InvalidQuestion;
                    return null;
                }

                if (passwordAnswer.IsNotSet())
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }
            }

            // Check provider User Key
            if (!(providerUserKey == null))
            {
                // IS not a duplicate key
                if (!(GetUser(providerUserKey, false) == null))
                {
                    status = MembershipCreateStatus.DuplicateProviderUserKey;
                    return null;
                }
            }

            // Check for unique email
            if (this.RequiresUniqueEmail)
            {
                if (this.GetUserNameByEmail(email).IsSet())
                {
                    status = MembershipCreateStatus.DuplicateEmail; // Email exists
                    return null;
                }
            }

            // Check for unique user name
            if (!(this.GetUser(username, false) == null))
            {
                status = MembershipCreateStatus.DuplicateUserName; // Username exists
                return null;
            }

            if (this.UseSalt)
            {
                salt = GenerateSalt();
            }

            pass = EncodeString(
                password,
                (int)this.PasswordFormat,
                salt,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            // Encode Password Answer
            string encodedPasswordAnswer = EncodeString(
                passwordAnswer,
                (int)this.PasswordFormat,
                salt,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            // Process database user creation request
            DB.Current.CreateUser(
                this.ApplicationName,
                username,
                pass,
                salt,
                (int)this.PasswordFormat,
                email,
                passwordQuestion,
                encodedPasswordAnswer,
                isApproved,
                providerUserKey);

            status = MembershipCreateStatus.Success;

            return this.GetUser(username, false);
        }

        /// <summary>
        ///     Delete User and User's information from provider
        /// </summary>
        /// <param name="username">
        ///     Username to delete
        /// </param>
        /// <param name="deleteAllRelatedData">
        ///     Delete all related daata
        /// </param>
        /// <returns>
        ///     Boolean depending on whether the deletion was successful
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            // Check username argument is not null
            if (username == null)
            {
                ExceptionReporter.ThrowArgumentNull("MEMBERSHIP", "USERNAMENULL");
            }

            // Process database user deletion request
            try
            {
                DB.Current.DeleteUser(this.ApplicationName, username, deleteAllRelatedData);

                return true;
            }
            catch
            {
                // will return false...  
            }

            return false;
        }

        /// <summary>
        ///     Retrieves all users into a MembershupUserCollection where Email Matches
        /// </summary>
        /// <param name="emailToMatch">
        ///     Email use as filter criteria
        /// </param>
        /// <param name="pageIndex">
        ///     Page Index
        /// </param>
        /// <param name="pageSize">
        ///     The page Size.
        /// </param>
        /// <param name="totalRecords">
        ///     Out - Number of records held
        /// </param>
        /// <returns>
        ///     <see cref="MembershipUser" /> Collection
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(
            string emailToMatch,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            var users = new MembershipUserCollection();

            if (pageIndex < 0)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGEINDEX");
            }

            if (pageSize < 1)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGESIZE");
            }

            // Loop through all users
            foreach (DataRow dr in DB.Current.FindUsersByEmail(this.ApplicationName, emailToMatch, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(this.UserFromDataRow(dr));
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        ///     Retrieves all users into a <see cref="MembershipUserCollection" /> where Username matches
        /// </summary>
        /// <param name="usernameToMatch">
        ///     Username use as filter criteria
        /// </param>
        /// <param name="pageIndex">
        ///     Page Index
        /// </param>
        /// <param name="pageSize">
        ///     The page Size.
        /// </param>
        /// <param name="totalRecords">
        ///     Out - Number of records held
        /// </param>
        /// <returns>
        ///     <see cref="MembershipUser" /> Collection
        /// </returns>
        public override MembershipUserCollection FindUsersByName(
            string usernameToMatch,
            int pageIndex,
            int pageSize,
            out int totalRecords)
        {
            var users = new MembershipUserCollection();

            if (pageIndex < 0)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGEINDEX");
            }

            if (pageSize < 1)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGESIZE");
            }

            // Loop through all users
            foreach (DataRow dr in DB.Current.FindUsersByName(this.ApplicationName, usernameToMatch, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(this.UserFromDataRow(dr));
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        ///     Retrieves all users into a <see cref="MembershipUserCollection" />
        /// </summary>
        /// <param name="pageIndex">
        ///     Page Index
        /// </param>
        /// <param name="pageSize">
        ///     The page Size.
        /// </param>
        /// <param name="totalRecords">
        ///     Out - Number of records held
        /// </param>
        /// <returns>
        ///     <see cref="MembershipUser" /> Collection
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();

            if (pageIndex < 0)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGEINDEX");
            }

            if (pageSize < 1)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "BADPAGESIZE");
            }

            // Loop through all users
            foreach (DataRow dr in DB.Current.GetAllUsers(this.ApplicationName, pageIndex, pageSize).Rows)
            {
                // Add new user to collection
                users.Add(this.UserFromDataRow(dr));
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        ///     Retrieves the number of users currently online for this application
        /// </summary>
        /// <returns>
        ///     Number of users online
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            return DB.Current.GetNumberOfUsersOnline(this.ApplicationName, Membership.UserIsOnlineTimeWindow);
        }

        /// <summary>
        ///     Retrieves the Users password (if <see cref="EnablePasswordRetrieval" /> is <see langword="true" />)
        /// </summary>
        /// <param name="username">
        ///     Username to retrieve password for
        /// </param>
        /// <param name="answer">
        ///     Answer to the Users Membership Question
        /// </param>
        /// <returns>
        ///     Password unencrypted
        /// </returns>
        public override string GetPassword(string username, string answer)
        {
            if (!this.EnablePasswordRetrieval)
            {
                ExceptionReporter.ThrowNotSupported("MEMBERSHIP", "PASSWORDRETRIEVALNOTSUPPORTED");
            }

            // Check for null arguments
            if ((username == null) || (answer == null))
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "USERNAMEPASSWORDNULL");
            }

            UserPasswordInfo currentPasswordInfo = UserPasswordInfo.CreateInstanceFromDB(
                this.ApplicationName,
                username,
                false,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            if (currentPasswordInfo != null && currentPasswordInfo.IsCorrectAnswer(answer))
            {
                return DecodeString(currentPasswordInfo.Password, currentPasswordInfo.PasswordFormat);
            }

            return null;
        }

        /// <summary>
        ///     Retrieves a <see cref="MembershipUser" /> object from the criteria given
        /// </summary>
        /// <param name="username">
        ///     Username to be foundr
        /// </param>
        /// <param name="userIsOnline">
        ///     Is the User currently online
        /// </param>
        /// <returns>
        ///     MembershipUser object
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (username == null)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "USERNAMENULL");
            }

            // if it's empty don't bother calling the DB.
            if (username.IsNotSet())
            {
                return null;
            }

            DataRow dr = DB.Current.GetUser(this.ApplicationName, null, username, userIsOnline);

            return dr != null ? this.UserFromDataRow(dr) : null;
        }

        /// <summary>
        ///     Retrieves a <see cref="MembershipUser" /> object from the criteria given
        /// </summary>
        /// <param name="providerUserKey">
        ///     User to be found based on UserKey
        /// </param>
        /// <param name="userIsOnline">
        ///     Is the User currently online
        /// </param>
        /// <returns>
        ///     MembershipUser object
        /// </returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                ExceptionReporter.ThrowArgumentNull("MEMBERSHIP", "USERKEYNULL");
            }

            DataRow dr = DB.Current.GetUser(this.ApplicationName, providerUserKey, null, userIsOnline);

            return dr != null ? this.UserFromDataRow(dr) : null;
        }

        /// <summary>
        ///     Retrieves a <see cref="MembershipUser" /> object from the criteria given
        /// </summary>
        /// <param name="email">
        ///     The email.
        /// </param>
        /// <returns>
        ///     Username as string
        /// </returns>
        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
            {
                ExceptionReporter.ThrowArgumentNull("MEMBERSHIP", "EMAILNULL");
            }

            DataTable users = DB.Current.GetUserNameByEmail(this.ApplicationName, email);

            if (this.RequiresUniqueEmail && users.Rows.Count > 1)
            {
                ExceptionReporter.ThrowProvider("MEMBERSHIP", "TOOMANYUSERNAMERETURNS");
            }

            return users.Rows.Count == 0 ? null : users.Rows[0]["Username"].ToString();
        }

        /// <summary>
        ///     Initialize Membership Provider
        /// </summary>
        /// <param name="name">
        ///     Membership Provider Name
        /// </param>
        /// <param name="config">
        ///     <see cref="NameValueCollection" /> of configuration items
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that the configuration section was properly passed
            if (config == null)
            {
                ExceptionReporter.ThrowArgument("ROLES", "CONFIGNOTFOUND");
            }

            // Retrieve information for provider from web config
            // config ints

            // Minimum Required Password Length from Provider configuration
            this._minimumRequiredPasswordLength = int.Parse(config["minRequiredPasswordLength"] ?? "6");

            // Minimum Required Non Alpha-numeric Characters from Provider configuration
            this._minRequiredNonAlphanumericCharacters = int.Parse(config["minRequiredNonalphanumericCharacters"] ?? "0");

            // Maximum number of allowed password attempts
            this._maxInvalidPasswordAttempts = int.Parse(config["maxInvalidPasswordAttempts"] ?? "5");

            // Password Attempt Window when maximum attempts have been reached
            this._passwordAttemptWindow = int.Parse(config["passwordAttemptWindow"] ?? "10");

            // Check whething Hashing methods should use Salt
            this._useSalt = (config["useSalt"] ?? "false").ToBool();

            // Check whether password hashing should output as Hex instead of Base64
            this._hashHex = (config["hashHex"] ?? "false").ToBool();

            // Check to see if password hex case should be altered
            this._hashCase = config["hashCase"].ToStringDBNull("None");

            // Check to see if password should have characters removed
            this._hashRemoveChars = config["hashRemoveChars"].ToStringDBNull();

            // Check to see if password/salt combination needs to asp.net standard membership compliant
            this._msCompliant = (config["msCompliant"] ?? "false").ToBool();

            // Application Name
            this._appName = config["applicationName"].ToStringDBNull("YetAnotherForum");

            // Connection String Name
            this._connStrName = config["connectionStringName"].ToStringDBNull();

            this._passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"].ToStringDBNull();

            // Password reset enabled from Provider Configuration
            this._enablePasswordReset = (config["enablePasswordReset"] ?? "true").ToBool();
            this._enablePasswordRetrieval = (config["enablePasswordRetrieval"] ?? "false").ToBool();
            this._requiresQuestionAndAnswer = (config["requiresQuestionAndAnswer"] ?? "true").ToBool();

            this._requiresUniqueEmail = (config["requiresUniqueEmail"] ?? "true").ToBool();

            string strPasswordFormat = config["passwordFormat"].ToStringDBNull("Hashed");

            switch (strPasswordFormat)
            {
                case "Clear":
                    this._passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                case "Encrypted":
                    this._passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Hashed":
                    this._passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                default:
                    ExceptionReporter.Throw("MEMBERSHIP", "BADPASSWORDFORMAT");
                    break;
            }

            ConnStringHelpers.TrySetConnectionAppString(this._connStrName, ConnStrAppKeyName);

            base.Initialize(name, config);
        }

        /// <summary>
        ///     Reset a users password - *
        /// </summary>
        /// <param name="username">
        ///     User to be found based by Name
        /// </param>
        /// <param name="answer">
        ///     Verification that it is them
        /// </param>
        /// <returns>
        ///     Username as string
        /// </returns>
        public override string ResetPassword(string username, string answer)
        {
            string newPassword = string.Empty,
                newPasswordEnc = string.Empty,
                newPasswordSalt = string.Empty,
                newPasswordAnswer = string.Empty;

            // Check Password reset is enabled
            if (!this.EnablePasswordReset)
            {
                ExceptionReporter.ThrowNotSupported("MEMBERSHIP", "RESETNOTSUPPORTED");
            }

            // Check arguments for null values
            if (username == null)
            {
                ExceptionReporter.ThrowArgument("MEMBERSHIP", "USERNAMEPASSWORDNULL");
            }

            // get an instance of the current password information class
            UserPasswordInfo currentPasswordInfo = UserPasswordInfo.CreateInstanceFromDB(
                this.ApplicationName,
                username,
                false,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            if (currentPasswordInfo != null)
            {
                if (this.UseSalt && currentPasswordInfo.PasswordSalt.IsNotSet())
                {
                    // get a new password salt...
                    newPasswordSalt = GenerateSalt();
                }
                else
                {
                    // use existing salt...
                    newPasswordSalt = currentPasswordInfo.PasswordSalt;
                }

                if (answer.IsSet())
                {
                    // verify answer is correct...
                    if (!currentPasswordInfo.IsCorrectAnswer(answer))
                    {
                        return null;
                    }
                }

                // create a new password
                newPassword = GeneratePassword(this.MinRequiredPasswordLength, this.MinRequiredNonAlphanumericCharacters);

                // encode it...
                newPasswordEnc = EncodeString(
                    newPassword,
                    (int)this.PasswordFormat,
                    newPasswordSalt,
                    this.UseSalt,
                    this.HashHex,
                    this.HashCase,
                    this.HashRemoveChars,
                    this.MSCompliant);

                // save to the database
                DB.Current.ResetPassword(
                    this.ApplicationName,
                    username,
                    newPasswordEnc,
                    newPasswordSalt,
                    (int)this.PasswordFormat,
                    this.MaxInvalidPasswordAttempts,
                    this.PasswordAttemptWindow);

                // Return unencrypted password
                return newPassword;
            }

            return null;
        }

        /// <summary>
        ///     Unlocks a users account
        /// </summary>
        /// <param name="userName">
        ///     The user Name.
        /// </param>
        /// <returns>
        ///     True/False is users account has been unlocked
        /// </returns>
        public override bool UnlockUser(string userName)
        {
            // Check for null argument
            if (userName == null)
            {
                ExceptionReporter.ThrowArgumentNull("MEMBERSHIP", "USERNAMENULL");
            }

            try
            {
                DB.Current.UnlockUser(this.ApplicationName, userName);
                return true;
            }
            catch
            {
                // will return false below
            }

            return false;
        }

        /// <summary>
        ///     Updates a providers user information
        /// </summary>
        /// <param name="user">
        ///     <see cref="MembershipUser" /> object
        /// </param>
        public override void UpdateUser(MembershipUser user)
        {
            // Check User object is not null
            if (user == null)
            {
                ExceptionReporter.ThrowArgumentNull("MEMBERSHIP", "MEMBERSHIPUSERNULL");
            }

            // Update User
            int updateStatus = DB.Current.UpdateUser(this.ApplicationName, user, this.RequiresUniqueEmail);

            // Check update was not successful
            if (updateStatus != 0)
            {
                // An error has occurred, determine which one.
                switch (updateStatus)
                {
                    case 1:
                        ExceptionReporter.Throw("MEMBERSHIP", "USERKEYNULL");
                        break;
                    case 2:
                        ExceptionReporter.Throw("MEMBERSHIP", "DUPLICATEEMAIL");
                        break;
                }
            }
        }

        /// <summary>
        ///     The custom method is implemented in YAF provider only to fix various issues related to it.
        /// </summary>
        /// <param name="user">
        ///     <see cref="MembershipUser" /> object
        /// </param>
        public void UpgradeMembership(int previousVersion, int newVersion)
        {
            DB.Current.UpgradeMembership(previousVersion, newVersion);
        }

        /// <summary>
        ///     Validates a user by user name / password
        /// </summary>
        /// <param name="username">
        ///     Username
        /// </param>
        /// <param name="password">
        ///     Password
        /// </param>
        /// ///
        /// <returns>
        ///     True/False whether username/password match what is on database.
        /// </returns>
        public override bool ValidateUser(string username, string password)
        {
            UserPasswordInfo currentUser = UserPasswordInfo.CreateInstanceFromDB(
                this.ApplicationName,
                username,
                false,
                this.UseSalt,
                this.HashHex,
                this.HashCase,
                this.HashRemoveChars,
                this.MSCompliant);

            if (currentUser != null && currentUser.IsApproved)
            {
                return currentUser.IsCorrectPassword(password);
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Encrypt string to hash method.
        /// </summary>
        /// <param name="clearString">
        ///     UnEncrypted Clear String
        /// </param>
        /// <param name="encFormat">
        ///     The enc Format.
        /// </param>
        /// <param name="salt">
        ///     Salt to be used in Hash method
        /// </param>
        /// <param name="useSalt">
        ///     Salt to be used in Hash method
        /// </param>
        /// <param name="hashHex">
        ///     The hash Hex.
        /// </param>
        /// <param name="hashCase">
        ///     The hash Case.
        /// </param>
        /// <param name="hashRemoveChars">
        ///     The hash Remove Chars.
        /// </param>
        /// <param name="msCompliant">
        ///     The ms Compliant.
        /// </param>
        /// <returns>
        ///     Encrypted string
        /// </returns>
        internal static string EncodeString(
            string clearString,
            int encFormat,
            string salt,
            bool useSalt,
            bool hashHex,
            string hashCase,
            string hashRemoveChars,
            bool msCompliant)
        {
            string encodedPass = string.Empty;

            var passwordFormat = (MembershipPasswordFormat)Enum.ToObject(typeof(MembershipPasswordFormat), encFormat);

            // Multiple Checks to ensure UseSalt is valid.
            if (clearString.IsNotSet())
            {
                // Check to ensure string is not null or empty.
                return String.Empty;
            }

            if (useSalt && salt.IsNotSet())
            {
                // If Salt value is null disable Salt procedure
                useSalt = false;
            }

            if (useSalt && passwordFormat == MembershipPasswordFormat.Encrypted)
            {
                useSalt = false; // Cannot use Salt with encryption
            }

            // Check Encoding format / method
            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Clear:

                    // plain text
                    encodedPass = clearString;
                    break;
                case MembershipPasswordFormat.Hashed:
                    encodedPass = Hash(clearString, HashType(), salt, useSalt, hashHex, hashCase, hashRemoveChars, msCompliant);
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPass = Encrypt(clearString, salt, msCompliant);
                    break;
                default:
                    encodedPass = Hash(clearString, HashType(), salt, useSalt, hashHex, hashCase, hashRemoveChars, msCompliant);
                    break;
            }

            return encodedPass;
        }

        /// <summary>
        ///     Decrypt string using passwordFormat.
        /// </summary>
        /// <param name="pass">
        ///     Password to be decrypted
        /// </param>
        /// <param name="passwordFormat">
        ///     Method of encryption
        /// </param>
        /// <returns>
        ///     Unencrypted string
        /// </returns>
        private static string DecodeString(string pass, int passwordFormat)
        {
            switch ((MembershipPasswordFormat)Enum.ToObject(typeof(MembershipPasswordFormat), passwordFormat))
            {
                case MembershipPasswordFormat.Clear: // MembershipPasswordFormat.Clear:
                    return pass;
                case MembershipPasswordFormat.Hashed: // MembershipPasswordFormat.Hashed:
                    ExceptionReporter.Throw("MEMBERSHIP", "DECODEHASH");
                    break;
                case MembershipPasswordFormat.Encrypted:
                    byte[] bIn = Convert.FromBase64String(pass);
                    byte[] bRet = (new YafMembershipProvider()).DecryptPassword(bIn);
                    if (bRet == null)
                    {
                        return null;
                    }

                    return Encoding.Unicode.GetString(bRet, 16, bRet.Length - 16);
                default:
                    ExceptionReporter.Throw("MEMBERSHIP", "DECODEHASH");
                    break;
            }

            return String.Empty; // Removes "Not all paths return a value" warning.
        }

        /// <summary>
        ///     The encrypt.
        /// </summary>
        /// <param name="clearString">
        ///     The clear string.
        /// </param>
        /// <param name="saltString">
        ///     The salt string.
        /// </param>
        /// <param name="standardComp">
        ///     The standard comp.
        /// </param>
        /// <returns>
        ///     The encrypt.
        /// </returns>
        private static string Encrypt(string clearString, string saltString, bool standardComp)
        {
            byte[] buffer = GeneratePasswordBuffer(saltString, clearString, standardComp);
            return Convert.ToBase64String((new YafMembershipProvider()).EncryptPassword(buffer));
        }

        /// <summary>
        ///     Creates a random password based on a miniumum length and a minimum number of non-alphanumeric characters
        /// </summary>
        /// <param name="minPassLength">
        ///     Minimum characters in the password
        /// </param>
        /// <param name="minNonAlphas">
        ///     Minimum non-alphanumeric characters
        /// </param>
        /// <returns>
        ///     Random string
        /// </returns>
        private static string GeneratePassword(int minPassLength, int minNonAlphas)
        {
            return Membership.GeneratePassword(minPassLength < _passwordsize ? _passwordsize : minPassLength, minNonAlphas);
        }

        /// <summary>
        ///     Creates a random string used as Salt for hashing
        /// </summary>
        /// <returns>
        ///     Random string
        /// </returns>
        private static string GenerateSalt()
        {
            var buf = new byte[16];
            var rngCryptoSp = new RNGCryptoServiceProvider();
            rngCryptoSp.GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        ///     Hashes clear bytes to given hashtype
        /// </summary>
        /// <param name="clearBytes">
        ///     Clear bytes to hash
        /// </param>
        /// <param name="hashType">
        ///     hash Algorithm to be used
        /// </param>
        /// <returns>
        ///     Hashed bytes
        /// </returns>
        private static byte[] Hash(byte[] clearBytes, string hashType)
        {
            // MD5, SHA1, SHA256, SHA384, SHA512
            byte[] hash = HashAlgorithm.Create(hashType).ComputeHash(clearBytes);
            return hash;
        }

        /// <summary>
        ///     The hash type.
        /// </summary>
        /// <returns>
        ///     The hash type.
        /// </returns>
        private static string HashType()
        {
            if (Membership.HashAlgorithmType.IsNotSet())
            {
                return "MD5"; // Default Hash Algorithm Type
            }
            else
            {
                return Membership.HashAlgorithmType;
            }
        }

        /// <summary>
        ///     Check to see if password(string) matches required criteria.
        /// </summary>
        /// <param name="password">
        ///     Password to be checked
        /// </param>
        /// <param name="minLength">
        ///     Minimum length required
        /// </param>
        /// <param name="minNonAlphaNumerics">
        ///     Minimum number of Non-alpha numerics in password
        /// </param>
        /// <param name="strengthRegEx">
        ///     Regular Expression Strength
        /// </param>
        /// <returns>
        ///     True/False
        /// </returns>
        private static bool IsPasswordCompliant(
            string password,
            int minLength,
            int minNonAlphaNumerics,
            string strengthRegEx)
        {
            // Check password meets minimum length criteria.
            if (!(password.Length >= minLength))
            {
                return false;
            }

            // Count Non alphanumerics
            int symbolCount = password.ToCharArray().Count(checkChar => !char.IsLetterOrDigit(checkChar));

            // Check password meets minimum alphanumeric criteria
            if (!(symbolCount >= minNonAlphaNumerics))
            {
                return false;
            }

            // Check Reg Expression is present
            if (strengthRegEx.Length > 0)
            {
                // Check password strength meets Password Strength Regex Requirements
                if (!Regex.IsMatch(password, strengthRegEx))
                {
                    return false;
                }
            }

            // Check string meets requirements as set in config
            return true;
        }

        /// <summary>
        ///     Check to see if password(string) matches required criteria.
        /// </summary>
        /// <param name="passsword">
        ///     The passsword.
        /// </param>
        /// <returns>
        ///     True/False
        /// </returns>
        private bool IsPasswordCompliant(string passsword)
        {
            return IsPasswordCompliant(
                passsword,
                this.MinRequiredPasswordLength,
                this.MinRequiredNonAlphanumericCharacters,
                this.PasswordStrengthRegularExpression);
        }

        /// <summary>
        ///     Creates a new <see cref="MembershipUser" /> from a <see cref="DataRow" /> with proper fields.
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private MembershipUser UserFromDataRow(DataRow dr)
        {
            return new MembershipUser(
                this.Name.ToStringDBNull(),
                dr["Username"].ToStringDBNull(),
                dr["UserID"].ToStringDBNull(),
                dr["Email"].ToStringDBNull(),
                dr["PasswordQuestion"].ToStringDBNull(),
                dr["Comment"].ToStringDBNull(),
                dr["IsApproved"].ToBool(),
                dr["IsLockedOut"].ToBool(),
                dr["Joined"].ToDateTime(DateTime.UtcNow),
                dr["LastLogin"].ToDateTime(DateTime.UtcNow),
                dr["LastActivity"].ToDateTime(DateTime.UtcNow),
                dr["LastPasswordChange"].ToDateTime(DateTime.UtcNow),
                dr["LastLockout"].ToDateTime(DateTime.UtcNow));
        }

        #endregion
    }
}