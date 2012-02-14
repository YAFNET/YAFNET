/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
	using System.Linq;
	using System.Web.Security;

	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Classes.Pattern;
	using YAF.Core;
	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Utils;

	#endregion

	/// <summary>
	/// The yaf membership db conn manager.
	/// </summary>
	public class MsSqlMembershipDbConnectionProvider : MsSqlDbConnectionProvider
	{
		#region Properties

		/// <summary>
		///   Gets ConnectionString.
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

		#endregion
	}

	/// <summary>
	/// The db.
	/// </summary>
	public class DB
	{
		#region Constants and Fields

		/// <summary>
		///   The _db access.
		/// </summary>
		private readonly IDbAccess _dbAccess = new MsSqlDbAccess(new MsSqlMembershipDbConnectionProvider());

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///   Initializes a new instance of the <see cref = "DB" /> class.
		/// </summary>
		public DB()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///   Gets Current.
		/// </summary>
		public static DB Current
		{
			get
			{
				return PageSingleton<DB>.Instance;
			}
		}

		#endregion

		#region Public Methods

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
		public void ChangePassword([NotNull] string appName, [NotNull] string username, [NotNull] string newPassword, [NotNull] string newSalt, int passwordFormat, [NotNull] string newPasswordAnswer)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_changepassword"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Username", username);
				cmd.AddParam("Password", newPassword);
				cmd.AddParam("PasswordSalt", newSalt);
				cmd.AddParam("PasswordFormat", passwordFormat);
				cmd.AddParam("PasswordAnswer", newPasswordAnswer);

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
		public void ChangePasswordQuestionAndAnswer([NotNull] string appName, [NotNull] string username, [NotNull] string passwordQuestion, [NotNull] string passwordAnswer)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_changepasswordquestionandanswer"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Username", username);
				cmd.AddParam("PasswordQuestion", passwordQuestion);
				cmd.AddParam("PasswordAnswer", passwordAnswer);

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
		public void CreateUser([NotNull] string appName, [NotNull] string username, [NotNull] string password, [NotNull] string passwordSalt, 
			int passwordFormat, [NotNull] string email, [NotNull] string passwordQuestion, [NotNull] string passwordAnswer, 
			bool isApproved, [NotNull] object providerUserKey)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_createuser"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Input Parameters
				cmd.AddParam("Username", username);
				cmd.AddParam("Password", password);
				cmd.AddParam("PasswordSalt", passwordSalt);
				cmd.AddParam("PasswordFormat", passwordFormat);
				cmd.AddParam("Email", email);
				cmd.AddParam("PasswordQuestion", passwordQuestion);
				cmd.AddParam("PasswordAnswer", passwordAnswer);
				cmd.AddParam("IsApproved", isApproved);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				// Input Output Parameters
				cmd.CreateOutputParameter("UserKey", DbType.Guid, direction: ParameterDirection.InputOutput);

				// Execute
				this._dbAccess.ExecuteNonQuery(cmd);

				// Retrieve Output Parameters
				providerUserKey = cmd.Parameters["UserKey"].Value;
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
		public void DeleteUser([NotNull] string appName, [NotNull] string username, bool deleteAllRelatedData)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_deleteuser"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Username", username);
				cmd.AddParam("DeleteAllRelated", deleteAllRelatedData);

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
		public DataTable FindUsersByEmail([NotNull] string appName, [NotNull] string emailToMatch, int pageIndex, int pageSize)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_findusersbyemail"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("EmailAddress", emailToMatch);
				cmd.AddParam("PageIndex", pageIndex);
				cmd.AddParam("PageSize", pageSize);

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
		public DataTable FindUsersByName([NotNull] string appName, [NotNull] string usernameToMatch, int pageIndex, int pageSize)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_findusersbyname"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Username", usernameToMatch);
				cmd.AddParam("PageIndex", pageIndex);
				cmd.AddParam("PageSize", pageSize);

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
		public DataTable GetAllUsers([NotNull] string appName, int pageIndex, int pageSize)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_getallusers"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("PageIndex", pageIndex);
				cmd.AddParam("PageSize", pageSize);

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
		public int GetNumberOfUsersOnline([NotNull] string appName, int timeWindow)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_getnumberofusersonline"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("TimeWindow", timeWindow);
				cmd.AddParam("CurrentTimeUtc", DateTime.UtcNow);

				cmd.CreateOutputParameter("ReturnValue", DbType.Int32, direction: ParameterDirection.ReturnValue);

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
		public DataRow GetUser([NotNull] string appName, [NotNull] object providerUserKey, [NotNull] string userName, bool userIsOnline)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_getuser"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("UserName", userName);
				cmd.AddParam("UserKey", providerUserKey);
				cmd.AddParam("UserIsOnline", userIsOnline);

				using (DataTable dt = this._dbAccess.GetData(cmd))
				{
					return dt.AsEnumerable().Any() ? dt.Rows[0] : null;
				}
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
		public DataTable GetUserNameByEmail([NotNull] string appName, [NotNull] string email)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_getusernamebyemail"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Email", email);

				return this._dbAccess.GetData(cmd);
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
		public DataTable GetUserPasswordInfo([NotNull] string appName, [NotNull] string username, bool updateUser)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_getuser"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("Username", username);
				cmd.AddParam("UserIsOnline", updateUser);

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
		public void ResetPassword([NotNull] string appName, [NotNull] string userName, [NotNull] string password, [NotNull] string passwordSalt, 
			int passwordFormat, 
			int maxInvalidPasswordAttempts, 
			int passwordAttemptWindow)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_resetpassword"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("UserName", userName);
				cmd.AddParam("Password", password);
				cmd.AddParam("PasswordSalt", passwordSalt);
				cmd.AddParam("PasswordFormat", passwordFormat);
				cmd.AddParam("MaxInvalidAttempts", maxInvalidPasswordAttempts);
				cmd.AddParam("PasswordAttemptWindow", passwordAttemptWindow);
				cmd.AddParam("CurrentTimeUtc", DateTime.UtcNow);

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
		public void UnlockUser([NotNull] string appName, [NotNull] string userName)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_unlockuser"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("UserName", userName);

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
		public int UpdateUser([NotNull] object appName, [NotNull] MembershipUser user, bool requiresUniqueEmail)
		{
			using (var cmd = this._dbAccess.GetCommand("prov_updateuser"))
			{
				cmd.AddParam("ApplicationName", appName);

				// Nonstandard args
				cmd.AddParam("UserKey", user.ProviderUserKey);
				cmd.AddParam("UserName", user.UserName);
				cmd.AddParam("Email", user.Email);
				cmd.AddParam("Comment", user.Comment);
				cmd.AddParam("IsApproved", user.IsApproved);
				cmd.AddParam("LastLogin", user.LastLoginDate);
				cmd.AddParam("LastActivity", user.LastActivityDate.ToUniversalTime());
				cmd.AddParam("UniqueEmail", requiresUniqueEmail);

				// Add Return Value
				cmd.CreateOutputParameter("ReturnValue", DbType.Int32, direction: ParameterDirection.ReturnValue);

				this._dbAccess.ExecuteNonQuery(cmd); // Execute Non SQL Query

				return Convert.ToInt32(cmd.Parameters["ReturnValue"].Value); // Return
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
			using (var cmd = this._dbAccess.GetCommand("prov_upgrade"))
			{
				// Nonstandard args
				cmd.AddParam("PreviousVersion", previousVersion);
				cmd.AddParam("NewVersion", newVersion);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				this._dbAccess.ExecuteNonQuery(cmd);
			}
		}

		#endregion
	}
}