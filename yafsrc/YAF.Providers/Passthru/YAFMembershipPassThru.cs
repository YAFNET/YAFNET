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

namespace YAFProviders.Passthru
{
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Web.Security;

    /// <summary>
  /// The yaf membership pass thru.
  /// </summary>
  internal class YAFMembershipPassThru : MembershipProvider
  {
    /// <summary>
    /// The _real provider.
    /// </summary>
    private MembershipProvider _realProvider;

    /// <summary>
    /// Gets or sets ApplicationName.
    /// </summary>
    public override string ApplicationName
    {
      get => this._realProvider.ApplicationName;

      set => this._realProvider.ApplicationName = value;
    }

    /// <summary>
    /// Gets a value indicating whether EnablePasswordReset.
    /// </summary>
    public override bool EnablePasswordReset => this._realProvider.EnablePasswordReset;

    /// <summary>
    /// Gets a value indicating whether EnablePasswordRetrieval.
    /// </summary>
    public override bool EnablePasswordRetrieval => this._realProvider.EnablePasswordRetrieval;

    /// <summary>
    /// Gets MaxInvalidPasswordAttempts.
    /// </summary>
    public override int MaxInvalidPasswordAttempts => this._realProvider.MaxInvalidPasswordAttempts;

    /// <summary>
    /// Gets MinRequiredNonAlphanumericCharacters.
    /// </summary>
    public override int MinRequiredNonAlphanumericCharacters => this._realProvider.MinRequiredNonAlphanumericCharacters;

    /// <summary>
    /// Gets MinRequiredPasswordLength.
    /// </summary>
    public override int MinRequiredPasswordLength => this._realProvider.MinRequiredPasswordLength;

    /// <summary>
    /// Gets PasswordAttemptWindow.
    /// </summary>
    public override int PasswordAttemptWindow => this._realProvider.PasswordAttemptWindow;

    /// <summary>
    /// Gets PasswordFormat.
    /// </summary>
    public override MembershipPasswordFormat PasswordFormat => this._realProvider.PasswordFormat;

    /// <summary>
    /// Gets PasswordStrengthRegularExpression.
    /// </summary>
    public override string PasswordStrengthRegularExpression => this._realProvider.PasswordStrengthRegularExpression;

    /// <summary>
    /// Gets a value indicating whether RequiresQuestionAndAnswer.
    /// </summary>
    public override bool RequiresQuestionAndAnswer => this._realProvider.RequiresQuestionAndAnswer;

    /// <summary>
    /// Gets a value indicating whether RequiresUniqueEmail.
    /// </summary>
    public override bool RequiresUniqueEmail => this._realProvider.RequiresUniqueEmail;

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="config">
    /// The config.
    /// </param>
    /// <exception cref="ProviderException">
    /// </exception>
    public override void Initialize(string name, NameValueCollection config)
    {
      var realProviderName = config["passThru"];


      if (realProviderName == null || realProviderName.Length < 1)
      {
        throw new ProviderException("Pass Thru provider name has not been specified in the web.config");
      }

      // Remove passThru configuration attribute
      config.Remove("passThru");

      // Check for further attributes
      if (config.Count > 0)
      {
        // Throw Provider error as no more attributes were expected
        throw new ProviderException("Unrecognised Attribute on the Membership PassThru Provider");
      }

      // Initialise the "Real" membership provider
      this._realProvider = Membership.Providers[realProviderName];
    }

    /// <summary>
    /// The change password.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="oldPassword">
    /// The old password.
    /// </param>
    /// <param name="newPassword">
    /// The new password.
    /// </param>
    /// <returns>
    /// The change password.
    /// </returns>
    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      return this._realProvider.ChangePassword(username, oldPassword, newPassword);
    }

    /// <summary>
    /// The change password question and answer.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="newPasswordQuestion">
    /// The new password question.
    /// </param>
    /// <param name="newPasswordAnswer">
    /// The new password answer.
    /// </param>
    /// <returns>
    /// The change password question and answer.
    /// </returns>
    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      return this._realProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
    }

    /// <summary>
    /// The create user.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="passwordQuestion">
    /// The password question.
    /// </param>
    /// <param name="passwordAnswer">
    /// The password answer.
    /// </param>
    /// <param name="isApproved">
    /// The is approved.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="status">
    /// The status.
    /// </param>
    /// <returns>
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
      return this._realProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
    }

    /// <summary>
    /// The delete user.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="deleteAllRelatedData">
    /// The delete all related data.
    /// </param>
    /// <returns>
    /// The delete user.
    /// </returns>
    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      return this._realProvider.DeleteUser(username, deleteAllRelatedData);
    }

    /// <summary>
    /// The find users by email.
    /// </summary>
    /// <param name="emailToMatch">
    /// The email to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      return this._realProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
    }

    /// <summary>
    /// The find users by name.
    /// </summary>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      return this._realProvider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
    }

    /// <summary>
    /// The get all users.
    /// </summary>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      return this._realProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
    }

    /// <summary>
    /// The get number of users online.
    /// </summary>
    /// <returns>
    /// The get number of users online.
    /// </returns>
    public override int GetNumberOfUsersOnline()
    {
      return this._realProvider.GetNumberOfUsersOnline();
    }

    /// <summary>
    /// The get password.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="answer">
    /// The answer.
    /// </param>
    /// <returns>
    /// The get password.
    /// </returns>
    public override string GetPassword(string username, string answer)
    {
      return this._realProvider.GetPassword(username, answer);
    }

    /// <summary>
    /// The get user.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="userIsOnline">
    /// The user is online.
    /// </param>
    /// <returns>
    /// </returns>
    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      return this._realProvider.GetUser(username, userIsOnline);
    }

    /// <summary>
    /// The get user.
    /// </summary>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="userIsOnline">
    /// The user is online.
    /// </param>
    /// <returns>
    /// </returns>
    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      return this._realProvider.GetUser(providerUserKey, userIsOnline);
    }

    /// <summary>
    /// The get user name by email.
    /// </summary>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// The get user name by email.
    /// </returns>
    public override string GetUserNameByEmail(string email)
    {
      return this._realProvider.GetUserNameByEmail(email);
    }

    /// <summary>
    /// The reset password.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="answer">
    /// The answer.
    /// </param>
    /// <returns>
    /// The reset password.
    /// </returns>
    public override string ResetPassword(string username, string answer)
    {
      return this._realProvider.ResetPassword(username, answer);
    }

    /// <summary>
    /// The unlock user.
    /// </summary>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <returns>
    /// The unlock user.
    /// </returns>
    public override bool UnlockUser(string userName)
    {
      return this._realProvider.UnlockUser(userName);
    }

    /// <summary>
    /// The update user.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    public override void UpdateUser(MembershipUser user)
    {
      this._realProvider.UpdateUser(user);
    }

    /// <summary>
    /// The validate user.
    /// </summary>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// The validate user.
    /// </returns>
    public override bool ValidateUser(string username, string password)
    {
      return this._realProvider.ValidateUser(username, password);
    }
  }
}