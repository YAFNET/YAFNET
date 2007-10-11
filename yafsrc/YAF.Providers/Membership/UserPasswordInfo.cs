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
        bool _isApproved;
        DateTime _lastLogin, _lastActivity;

        public UserPasswordInfo(string AppName, string Username, bool UpdateUser)
        {
            DataTable userData = DB.GetUserPasswordInfo(AppName, Username, UpdateUser);
            if (userData.Rows.Count != 0)
            {
                DataRow userInfo = userData.Rows[0];
                _password = userInfo["Password"].ToString();
                _passwordSalt = userInfo["PasswordSalt"].ToString();
                _passwordQuestion = userInfo["PasswordQuestion"].ToString();
                _passwordAnswer = userInfo["PasswordAnswer"].ToString();

                _passwordFormat = CleanUtils.ToInt(userInfo["PasswordFormat"]);

                _failedPasswordAttempts = CleanUtils.ToInt(userInfo["FailedPasswordAttempts"]);
                _failedAnswerAttempts = CleanUtils.ToInt(userInfo["FailedAnswerAttempts"]);

                _isApproved = Convert.ToBoolean(userInfo["IsApproved"]);

                _lastLogin = Convert.ToDateTime(userInfo["LastLogin"]);
                _lastActivity = Convert.ToDateTime(userInfo["LastActivity"]);
            }
        }

        public bool IsCorrectPassword(string passwordToCheck)
        {
            return this.Password.Equals(YafMembershipProvider.EncryptString(passwordToCheck, this.PasswordFormat, this.PasswordSalt));
        }

        public bool IsCorrectAnswer(string answerToCheck)
        {
            return this.PasswordAnswer.Equals((YafMembershipProvider.EncryptString(answerToCheck, this.PasswordFormat, this.PasswordSalt)));
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
    }
}
