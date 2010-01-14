/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Pattern;

namespace YAF.Providers.Membership
{
  /// <summary>
  /// The yaf membership db conn manager.
  /// </summary>
  public class YafMembershipDBConnManager : YafDBConnManager
  {
    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    public override string ConnectionString
    {
      get
      {
        if (YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] != null)
        {
          return YafContext.Application[YafMembershipProvider.ConnStrAppKeyName] as string;
        }

        return Config.ConnectionString;
      }
    }
  }

  /// <summary>
  /// The db.
  /// </summary>
  public class DB
  {
    /// <summary>
    /// The _db access.
    /// </summary>
    private YafDBAccess _dbAccess = new YafDBAccess();

    /// <summary>
    /// Initializes a new instance of the <see cref="DB"/> class.
    /// </summary>
    public DB()
    {
      this._dbAccess.SetConnectionManagerAdapter<YafMembershipDBConnManager>();
    }

    /// <summary>
    /// Gets Current.
    /// </summary>
    public static DB Current
    {
      get
      {
        return PageSingleton<DB>.Instance;
      }
    }

    /// <summary>
    /// The upgrade membership.
    /// </summary>
    /// <param name="previousVersion">
    /// The previous version.
    /// </param>
    /// <param name="newVersion">
    /// The new version.
    /// </param>
    public void UpgradeMembership(int previousVersion, int newVersion)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_upgrade")))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        // Nonstandard args
        cmd.Parameters.AddWithValue("@PreviousVersion", previousVersion);
        cmd.Parameters.AddWithValue("@NewVersion", newVersion);

        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The change password.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="newPassword">
    /// The new password.
    /// </param>
    /// <param name="newSalt">
    /// The new salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="newPasswordAnswer">
    /// The new password answer.
    /// </param>
    public void ChangePassword(string appName, string username, string newPassword, string newSalt, int passwordFormat, string newPasswordAnswer)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_changepassword")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Password", newPassword);
        cmd.Parameters.AddWithValue("@PasswordSalt", newSalt);
        cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("@PasswordAnswer", newPasswordAnswer);

        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The change password question and answer.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="passwordQuestion">
    /// The password question.
    /// </param>
    /// <param name="passwordAnswer">
    /// The password answer.
    /// </param>
    public void ChangePasswordQuestionAndAnswer(string appName, string username, string passwordQuestion, string passwordAnswer)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_changepasswordquestionandanswer")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@PasswordQuestion", passwordQuestion);
        cmd.Parameters.AddWithValue("@PasswordAnswer", passwordAnswer);
        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The create user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="passwordSalt">
    /// The password salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
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
    public void CreateUser(
      string appName, 
      string username, 
      string password, 
      string passwordSalt, 
      int passwordFormat, 
      string email, 
      string passwordQuestion, 
      string passwordAnswer, 
      bool isApproved, 
      object providerUserKey)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_createuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Input Parameters
        cmd.Parameters.AddWithValue("Username", username);
        cmd.Parameters.AddWithValue("Password", password);
        cmd.Parameters.AddWithValue("PasswordSalt", passwordSalt);
        cmd.Parameters.AddWithValue("PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("PasswordQuestion", passwordQuestion);
        cmd.Parameters.AddWithValue("PasswordAnswer", passwordAnswer);
        cmd.Parameters.AddWithValue("IsApproved", isApproved);

        // Input Output Parameters
        var paramUserKey = new SqlParameter("UserKey", SqlDbType.UniqueIdentifier);
        paramUserKey.Direction = ParameterDirection.InputOutput;
        paramUserKey.Value = providerUserKey;
        cmd.Parameters.Add(paramUserKey);

        // Execute
        this._dbAccess.ExecuteNonQuery(cmd);

        // Retrieve Output Parameters
        providerUserKey = paramUserKey.Value;
      }
    }

    /// <summary>
    /// The delete user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="username">
    /// The username.
    /// </param>
    /// <param name="deleteAllRelatedData">
    /// The delete all related data.
    /// </param>
    public void DeleteUser(string appName, string username, bool deleteAllRelatedData)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_deleteuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@DeleteAllRelated", deleteAllRelatedData);
        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The find users by email.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="emailToMatch">
    /// The email to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable FindUsersByEmail(string appName, string emailToMatch, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_findusersbyemail")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@EmailAddress", emailToMatch);
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._dbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The find users by name.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable FindUsersByName(string appName, string usernameToMatch, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_findusersbyname")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", usernameToMatch);
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._dbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get all users.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetAllUsers(string appName, int pageIndex, int pageSize)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_getallusers")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        cmd.Parameters.AddWithValue("@PageSize", pageSize);
        return this._dbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get number of users online.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="timeWindow">
    /// The time window.
    /// </param>
    /// <returns>
    /// The get number of users online.
    /// </returns>
    public int GetNumberOfUsersOnline(string appName, int timeWindow)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_getnumberofusersonline")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@TimeWindow", timeWindow);
        cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
        var p = new SqlParameter("ReturnValue", SqlDbType.Int);
        p.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.Add(p);
        this._dbAccess.ExecuteNonQuery(cmd);
        return Convert.ToInt32(cmd.Parameters["ReturnValue"].Value);
      }
    }

    /// <summary>
    /// The get user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userIsOnline">
    /// The user is online.
    /// </param>
    /// <returns>
    /// </returns>
    public DataRow GetUser(string appName, object providerUserKey, string userName, bool userIsOnline)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_getuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
        cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
        using (DataTable dt = this._dbAccess.GetData(cmd))
        {
          if (dt.Rows.Count > 0)
          {
            return dt.Rows[0];
          }
          else
          {
            return null;
          }
        }
      }
    }

    /// <summary>
    /// The get user password info.
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
    /// <returns>
    /// </returns>
    public DataTable GetUserPasswordInfo(string appName, string username, bool updateUser)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_getuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@UserIsOnline", updateUser);
        return this._dbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The get user name by email.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// </returns>
    public DataTable GetUserNameByEmail(string appName, string email)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_getusernamebyemail")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@Email", email);
        return this._dbAccess.GetData(cmd);
      }
    }

    /// <summary>
    /// The reset password.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="passwordSalt">
    /// The password salt.
    /// </param>
    /// <param name="passwordFormat">
    /// The password format.
    /// </param>
    /// <param name="maxInvalidPasswordAttempts">
    /// The max invalid password attempts.
    /// </param>
    /// <param name="passwordAttemptWindow">
    /// The password attempt window.
    /// </param>
    public void ResetPassword(
      string appName, string userName, string password, string passwordSalt, int passwordFormat, int maxInvalidPasswordAttempts, int passwordAttemptWindow)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_resetpassword")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        cmd.Parameters.AddWithValue("@Password", password);
        cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
        cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
        cmd.Parameters.AddWithValue("@MaxInvalidAttempts", maxInvalidPasswordAttempts);
        cmd.Parameters.AddWithValue("@PasswordAttemptWindow", passwordAttemptWindow);
        cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);

        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The unlock user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    public void UnlockUser(string appName, string userName)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_unlockuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("@UserName", userName);
        this._dbAccess.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The update user.
    /// </summary>
    /// <param name="appName">
    /// The app name.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="requiresUniqueEmail">
    /// The requires unique email.
    /// </param>
    /// <returns>
    /// The update user.
    /// </returns>
    public int UpdateUser(object appName, MembershipUser user, bool requiresUniqueEmail)
    {
      using (var cmd = new SqlCommand(YafDBAccess.GetObjectName("prov_updateuser")))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ApplicationName", appName);

        // Nonstandard args
        cmd.Parameters.AddWithValue("UserKey", user.ProviderUserKey);
        cmd.Parameters.AddWithValue("UserName", user.UserName);
        cmd.Parameters.AddWithValue("Email", user.Email);
        cmd.Parameters.AddWithValue("Comment", user.Comment);
        cmd.Parameters.AddWithValue("IsApproved", user.IsApproved);
        cmd.Parameters.AddWithValue("LastLogin", user.LastLoginDate);
        cmd.Parameters.AddWithValue("LastActivity", user.LastActivityDate.ToUniversalTime());
        cmd.Parameters.AddWithValue("UniqueEmail", requiresUniqueEmail);

        // Add Return Value
        var p = new SqlParameter("ReturnValue", SqlDbType.Int);
        p.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.Add(p);

        this._dbAccess.ExecuteNonQuery(cmd); // Execute Non SQL Query
        return Convert.ToInt32(p.Value); // Return
      }
    }
  }
}