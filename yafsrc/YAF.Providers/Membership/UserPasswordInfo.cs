using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace YAF.Providers.Membership
{
    class UserPasswordInfo
    {
        // Instance Variables
        string _password, _passwordSalt, _passwordQuestion, _passwordAnswer;
        int _passwordFormat, _failedPasswordAttempts, _failedAnswerAttempts;
        bool _isApproved, _useSalt;
        DateTime _lastLogin, _lastActivity;

        /// <summary>
        /// Called to create a new UserPasswordInfo class instance.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="passwordFormat"></param>
        /// <param name="failedPasswordAttempts"></param>
        /// <param name="failedAnswerAttempts"></param>
        /// <param name="isApproved"></param>
        /// <param name="useSalt"></param>
        /// <param name="lastLogin"></param>
        /// <param name="lastActivity"></param>
        public UserPasswordInfo(string password, string passwordSalt, string passwordQuestion, string passwordAnswer, 
                                int passwordFormat, int failedPasswordAttempts, int failedAnswerAttempts,
                                bool isApproved, bool useSalt, DateTime lastLogin, DateTime lastActivity)
        {
            // nothing to do except set the local variables...
            _password = password;
            _passwordSalt = passwordSalt;
            _passwordQuestion = passwordQuestion;
            _passwordAnswer = passwordAnswer;
            _passwordFormat = passwordFormat;
            _failedPasswordAttempts = failedPasswordAttempts;
            _failedAnswerAttempts = failedAnswerAttempts;
            _isApproved = isApproved;
            _lastLogin = lastLogin;
            _lastActivity = lastActivity;
            _useSalt = useSalt;
        }

        // used to create an instance of this class from the DB...
        public static UserPasswordInfo CreateInstanceFromDB(string appName, string username, bool updateUser, bool useSalt)
        {
            DataTable userData = DB.GetUserPasswordInfo(appName, username, updateUser);

            if (userData.Rows.Count > 0)
            {
                DataRow userInfo = userData.Rows[0];
                // create a new instance of the UserPasswordInfo class
                return new UserPasswordInfo( userInfo["Password"].ToString(), userInfo["PasswordSalt"].ToString(), userInfo["PasswordQuestion"].ToString(), userInfo["PasswordAnswer"].ToString(),
                                            Utils.Transform.ToInt(userInfo["PasswordFormat"]), Utils.Transform.ToInt(userInfo["FailedPasswordAttempts"]), Utils.Transform.ToInt(userInfo["FailedAnswerAttempts"]),
                                            Convert.ToBoolean(userInfo["IsApproved"]), useSalt, Convert.ToDateTime(userInfo["LastLogin"]), Convert.ToDateTime(userInfo["LastActivity"]));
            }

            // nothing found, return null.
            return null;
        }

        /// <summary>
        /// Checks the password against the one provided for validity
        /// </summary>
        /// <param name="passwordToCheck"></param>
        /// <returns></returns>
        public bool IsCorrectPassword(string passwordToCheck)
        {
            return this.Password.Equals(YafMembershipProvider.EncodeString(passwordToCheck, this.PasswordFormat, this.PasswordSalt, this.UseSalt));
        }

        /// <summary>
        /// Checks the user answer against the one provided for validity
        /// </summary>
        /// <param name="answerToCheck"></param>
        /// <returns></returns>
        public bool IsCorrectAnswer(string answerToCheck)
        {
            return this.PasswordAnswer.Equals((YafMembershipProvider.EncodeString(answerToCheck, this.PasswordFormat, this.PasswordSalt, this.UseSalt)));
        }

        public string Password
        {
            get { return _password; }
        }

        public string PasswordQuestion
        {
            get { return _passwordQuestion; }
        }

        public string PasswordAnswer
        {
            get { return _passwordAnswer; }
        }

        public string PasswordSalt
        {
            get { return _passwordSalt; }
        }

        public int PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public int FailedPasswordAttempts
        {
            get { return _failedPasswordAttempts; }
        }

        public int FailedAnswerAttempts
        {
            get { return _failedAnswerAttempts; }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
        }

        public DateTime LastLogin
        {
            get { return _lastLogin; }
        }

        public DateTime LastActivity
        {
            get { return _lastActivity; }
        }

        public Boolean UseSalt
        {
            get { return _useSalt; }
        }
    }
}
