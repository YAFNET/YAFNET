/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Providers.Membership
{
  #region Using

  using System;

  using YAF.Providers.Utils;
  using YAF.Types.Extensions;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The user password info.
  /// </summary>
  internal class UserPasswordInfo
  {
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
      this.Password = password;
      this.PasswordSalt = passwordSalt;
      this.PasswordQuestion = passwordQuestion;
      this.PasswordAnswer = passwordAnswer;
      this.PasswordFormat = passwordFormat;
      this.FailedPasswordAttempts = failedPasswordAttempts;
      this.FailedAnswerAttempts = failedAnswerAttempts;
      this.IsApproved = isApproved;
      this.LastLogin = lastLogin;
      this.LastActivity = lastActivity;
      this.UseSalt = useSalt;
      this.HashHex = hashHex;
      this.hashCase = hashCase;
      this.hashRemoveChars = hashRemoveChars;
      this.msCompliant = msCompliant;
    }

    #endregion

    // used to create an instance of this class from the DB...
    #region Properties

    /// <summary>
    /// Gets FailedAnswerAttempts.
    /// </summary>
    public int FailedAnswerAttempts { get; }

    /// <summary>
    /// Gets FailedPasswordAttempts.
    /// </summary>
    public int FailedPasswordAttempts { get; }

    /// <summary>
    /// Gets hashCase.
    /// </summary>
    public string hashCase { get; }

    /// <summary>
    /// Gets a value indicating whether HashHex.
    /// </summary>
    public bool HashHex { get; }

    /// <summary>
    /// Gets hashRemoveChars.
    /// </summary>
    public string hashRemoveChars { get; }

    /// <summary>
    /// Gets a value indicating whether IsApproved.
    /// </summary>
    public bool IsApproved { get; }

    /// <summary>
    /// Gets LastActivity.
    /// </summary>
    public DateTime LastActivity { get; }

    /// <summary>
    /// Gets LastLogin.
    /// </summary>
    public DateTime LastLogin { get; }

    /// <summary>
    /// Gets a value indicating whether msCompliant.
    /// </summary>
    public bool msCompliant { get; }

    /// <summary>
    /// Gets Password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets PasswordAnswer.
    /// </summary>
    public string PasswordAnswer { get; }

    /// <summary>
    /// Gets PasswordFormat.
    /// </summary>
    public int PasswordFormat { get; }

    /// <summary>
    /// Gets PasswordQuestion.
    /// </summary>
    public string PasswordQuestion { get; }

    /// <summary>
    /// Gets PasswordSalt.
    /// </summary>
    public string PasswordSalt { get; }

    /// <summary>
    /// Gets a value indicating whether UseSalt.
    /// </summary>
    public bool UseSalt { get; }

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
      var userData = DB.Current.GetUserPasswordInfo(appName, username, updateUser);

      if (!userData.HasRows())
      {
          return null;
      }

      var userInfo = userData.GetFirstRow();

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

      // nothing found, return null.
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