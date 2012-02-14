/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
  #region Using

  using System;
  using System.Data;

  using YAF.Providers.Utils;

  #endregion

  /// <summary>
  /// The user password info.
  /// </summary>
  internal class UserPasswordInfo
  {
    #region Constants and Fields

    /// <summary>
    /// The _failed answer attempts.
    /// </summary>
    private int _failedAnswerAttempts;

    /// <summary>
    /// The _failed password attempts.
    /// </summary>
    private int _failedPasswordAttempts;

    /// <summary>
    /// The _hash case.
    /// </summary>
    private string _hashCase;

    /// <summary>
    /// The _hash hex.
    /// </summary>
    private bool _hashHex;

    /// <summary>
    /// The _hash remove chars.
    /// </summary>
    private string _hashRemoveChars;

    /// <summary>
    /// The _is approved.
    /// </summary>
    private bool _isApproved;

    /// <summary>
    /// The _last activity.
    /// </summary>
    private DateTime _lastActivity;

    /// <summary>
    /// The _last login.
    /// </summary>
    private DateTime _lastLogin;

    /// <summary>
    /// The _ms compliant.
    /// </summary>
    private bool _msCompliant;

    /// <summary>
    /// The _password.
    /// </summary>
    private string _password;

    /// <summary>
    /// The _password answer.
    /// </summary>
    private string _passwordAnswer;

    /// <summary>
    /// The _password format.
    /// </summary>
    private int _passwordFormat;

    /// <summary>
    /// The _password question.
    /// </summary>
    private string _passwordQuestion;

    /// <summary>
    /// The _password salt.
    /// </summary>
    private string _passwordSalt;

    /// <summary>
    /// The _use salt.
    /// </summary>
    private bool _useSalt;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UserPasswordInfo"/> class. 
    /// Called to create a new UserPasswordInfo class instance.
    /// </summary>
    /// <param name="password">
    /// </param>
    /// <param name="passwordSalt">
    /// </param>
    /// <param name="passwordQuestion">
    /// </param>
    /// <param name="passwordAnswer">
    /// </param>
    /// <param name="passwordFormat">
    /// </param>
    /// <param name="failedPasswordAttempts">
    /// </param>
    /// <param name="failedAnswerAttempts">
    /// </param>
    /// <param name="isApproved">
    /// </param>
    /// <param name="useSalt">
    /// </param>
    /// <param name="lastLogin">
    /// </param>
    /// <param name="lastActivity">
    /// </param>
    /// <param name="hashHex">
    /// The hash Hex.
    /// </param>
    /// <param name="hashCase">
    /// The hash Case.
    /// </param>
    /// <param name="hashRemoveChars">
    /// The hash Remove Chars.
    /// </param>
    /// <param name="msCompliant">
    /// The ms Compliant.
    /// </param>
    public UserPasswordInfo(
      string password, 
      string passwordSalt, 
      string passwordQuestion, 
      string passwordAnswer, 
      int passwordFormat, 
      int failedPasswordAttempts, 
      int failedAnswerAttempts, 
      bool isApproved, 
      bool useSalt, 
      DateTime lastLogin, 
      DateTime lastActivity, 
      bool hashHex, 
      string hashCase, 
      string hashRemoveChars, 
      bool msCompliant)
    {
      // nothing to do except set the local variables...
      this._password = password;
      this._passwordSalt = passwordSalt;
      this._passwordQuestion = passwordQuestion;
      this._passwordAnswer = passwordAnswer;
      this._passwordFormat = passwordFormat;
      this._failedPasswordAttempts = failedPasswordAttempts;
      this._failedAnswerAttempts = failedAnswerAttempts;
      this._isApproved = isApproved;
      this._lastLogin = lastLogin;
      this._lastActivity = lastActivity;
      this._useSalt = useSalt;
      this._hashHex = hashHex;
      this._hashCase = hashCase;
      this._hashRemoveChars = hashRemoveChars;
      this._msCompliant = msCompliant;
    }

    #endregion

    // used to create an instance of this class from the DB...
    #region Properties

    /// <summary>
    /// Gets FailedAnswerAttempts.
    /// </summary>
    public int FailedAnswerAttempts
    {
      get
      {
        return this._failedAnswerAttempts;
      }
    }

    /// <summary>
    /// Gets FailedPasswordAttempts.
    /// </summary>
    public int FailedPasswordAttempts
    {
      get
      {
        return this._failedPasswordAttempts;
      }
    }

    /// <summary>
    /// Gets hashCase.
    /// </summary>
    public string hashCase
    {
      get
      {
        return this._hashCase;
      }
    }

    /// <summary>
    /// Gets a value indicating whether HashHex.
    /// </summary>
    public bool HashHex
    {
      get
      {
        return this._hashHex;
      }
    }

    /// <summary>
    /// Gets hashRemoveChars.
    /// </summary>
    public string hashRemoveChars
    {
      get
      {
        return this._hashRemoveChars;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsApproved.
    /// </summary>
    public bool IsApproved
    {
      get
      {
        return this._isApproved;
      }
    }

    /// <summary>
    /// Gets LastActivity.
    /// </summary>
    public DateTime LastActivity
    {
      get
      {
        return this._lastActivity;
      }
    }

    /// <summary>
    /// Gets LastLogin.
    /// </summary>
    public DateTime LastLogin
    {
      get
      {
        return this._lastLogin;
      }
    }

    /// <summary>
    /// Gets a value indicating whether msCompliant.
    /// </summary>
    public bool msCompliant
    {
      get
      {
        return this._msCompliant;
      }
    }

    /// <summary>
    /// Gets Password.
    /// </summary>
    public string Password
    {
      get
      {
        return this._password;
      }
    }

    /// <summary>
    /// Gets PasswordAnswer.
    /// </summary>
    public string PasswordAnswer
    {
      get
      {
        return this._passwordAnswer;
      }
    }

    /// <summary>
    /// Gets PasswordFormat.
    /// </summary>
    public int PasswordFormat
    {
      get
      {
        return this._passwordFormat;
      }
    }

    /// <summary>
    /// Gets PasswordQuestion.
    /// </summary>
    public string PasswordQuestion
    {
      get
      {
        return this._passwordQuestion;
      }
    }

    /// <summary>
    /// Gets PasswordSalt.
    /// </summary>
    public string PasswordSalt
    {
      get
      {
        return this._passwordSalt;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UseSalt.
    /// </summary>
    public bool UseSalt
    {
      get
      {
        return this._useSalt;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The create instance from db.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="updateUser">
    /// The update user.
    /// </param>
    /// <param name="useSalt">
    /// The use salt.
    /// </param>
    /// <param name="hashHex">
    /// The hash hex.
    /// </param>
    /// <param name="hashCase">
    /// The hash case.
    /// </param>
    /// <param name="hashRemoveChars">
    /// The hash remove chars.
    /// </param>
    /// <param name="msCompliant">
    /// The ms compliant.
    /// </param>
    /// <returns>
    /// </returns>
    public static UserPasswordInfo CreateInstanceFromDB(
      string appName, 
      string username, 
      bool updateUser, 
      bool useSalt, 
      bool hashHex, 
      string hashCase, 
      string hashRemoveChars, 
      bool msCompliant)
    {
      DataTable userData = DB.Current.GetUserPasswordInfo(appName, username, updateUser);

      if (userData.Rows.Count > 0)
      {
        DataRow userInfo = userData.Rows[0];

        // create a new instance of the UserPasswordInfo class
        return new UserPasswordInfo(
          userInfo["Password"].ToStringDBNull(), 
          userInfo["PasswordSalt"].ToStringDBNull(), 
          userInfo["PasswordQuestion"].ToStringDBNull(), 
          userInfo["PasswordAnswer"].ToStringDBNull(), 
          userInfo["PasswordFormat"].ToInt(), 
          userInfo["FailedPasswordAttempts"].ToInt(), 
          userInfo["FailedAnswerAttempts"].ToInt(), 
          userInfo["IsApproved"].ToBool(), 
          useSalt, 
          userInfo["LastLogin"].ToDateTime(DateTime.UtcNow),
          userInfo["LastActivity"].ToDateTime(DateTime.UtcNow), 
          hashHex, 
          hashCase, 
          hashRemoveChars, 
          msCompliant);
      }

      // nothing found, return null.
      return null;
    }

    /// <summary>
    /// Checks the user answer against the one provided for validity
    /// </summary>
    /// <param name="answerToCheck">
    /// </param>
    /// <returns>
    /// The is correct answer.
    /// </returns>
    public bool IsCorrectAnswer(string answerToCheck)
    {
      return
        this.PasswordAnswer.Equals(
          YafMembershipProvider.EncodeString(
            answerToCheck, 
            this.PasswordFormat, 
            this.PasswordSalt, 
            this.UseSalt, 
            this.HashHex, 
            this.hashCase, 
            this.hashRemoveChars, 
            this.msCompliant));
    }

    /// <summary>
    /// Checks the password against the one provided for validity
    /// </summary>
    /// <param name="passwordToCheck">
    /// </param>
    /// <returns>
    /// The is correct password.
    /// </returns>
    public bool IsCorrectPassword(string passwordToCheck)
    {
      return
        this.Password.Equals(
          YafMembershipProvider.EncodeString(
            passwordToCheck, 
            this.PasswordFormat, 
            this.PasswordSalt, 
            this.UseSalt, 
            this.HashHex, 
            this.hashCase, 
            this.hashRemoveChars, 
            this.msCompliant));
    }

    #endregion
  }
}