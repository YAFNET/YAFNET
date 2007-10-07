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
using System.Data.SqlClient;

using YAF.Classes.Data;

namespace YAF.Providers.Membership
{
    public class DB
    {
        public static void ChangePassword(string appName, string username, string newPassword, string newSalt, int passwordFormat, string newPasswordAnswer)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_changepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", newPassword);
                cmd.Parameters.AddWithValue("@PasswordSalt", newSalt);
                cmd.Parameters.AddWithValue("@PasswordFormat", passwordFormat);
                cmd.Parameters.AddWithValue("@PasswordAnswer", newPasswordAnswer);

                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        public static void ChangePasswordQuestionAndAnswer(string appName, string username, string passwordQuestion, string passwordAnswer)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_changepasswordquestionandanswer"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordQuestion", passwordQuestion);
                cmd.Parameters.AddWithValue("@PasswordAnswer", passwordAnswer);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        public static void CreateUser(string appName, string username, string password, string passwordSalt, int passwordFormat, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_createuser"))
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
                SqlParameter paramUserKey = new SqlParameter("UserKey", SqlDbType.UniqueIdentifier);
                paramUserKey.Direction = ParameterDirection.InputOutput;
                paramUserKey.Value = providerUserKey;
                cmd.Parameters.Add(paramUserKey);

                //Execute
                DBAccess.ExecuteNonQuery(cmd);
                //Retrieve Output Parameters
                providerUserKey = paramUserKey.Value;

            }
        }

        public static void DeleteUser(string appName, string username, bool deleteAllRelatedData)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_deleteuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@DeleteAllRelated", deleteAllRelatedData);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        public static DataTable FindUsersByEmail(string appName, string emailToMatch, int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_findusersbyemail"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@EmailAddress", emailToMatch);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DBAccess.GetData(cmd);
            }
        }

        public static DataTable FindUsersByName(string appName, string usernameToMatch, int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_findusersbyname"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", usernameToMatch);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DBAccess.GetData(cmd);
            }
        }

        public static DataTable GetAllUsers(string appName, int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getallusers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                return DBAccess.GetData(cmd);
            }
        }

        public static DataRow GetNumberOfUsersOnline(string appName, int TimeWindow)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getnumberofusersonline"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@TimeWindow", TimeWindow);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                return DBAccess.GetData(cmd).Rows[0];
            }
        }

        public static DataRow GetUser(string appName, string userName, bool userIsOnline)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
                return DBAccess.GetData(cmd).Rows[0];
            }

        }

        public static DataRow GetUser(string appName, object providerUserKey, bool userIsOnline)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
                cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
                return DBAccess.GetData(cmd).Rows[0];
            }

        }

        public static DataTable GetUserPasswordInfo(string appName, string username,bool updateUser)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@UserIsOnline", updateUser);
                return DBAccess.GetData(cmd);
            }

        }

        public static DataTable GetUserNameByEmail(string appName, string email)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_getusernamebyemail"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Email", email);
                return DBAccess.GetData(cmd);
            }
        }


        public static void ResetPassword(string appName, string userName, string password, string passwordSalt, int passwordFormat, int maxInvalidPasswordAttempts,int passwordAttemptWindow)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_resetpassword"))
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

                DBAccess.ExecuteNonQuery(cmd);
            }

        }

        public static void UnlockUser(string appName, string userName)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_unlockuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

        public static void UpdateUser(object appName, MembershipUser user, bool requiresUniqueEmail)
        {
            using (SqlCommand cmd = new SqlCommand("yafprov_updateuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserKey", user.ProviderUserKey);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Comment", user.Comment);
                cmd.Parameters.AddWithValue("@IsApproved", user.IsApproved);
                cmd.Parameters.AddWithValue("@LastLoginDate", user.LastLoginDate);
                cmd.Parameters.AddWithValue("@LastActivityDate", user.LastActivityDate.ToUniversalTime());
                cmd.Parameters.AddWithValue("@UniqueEmail", requiresUniqueEmail);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                DBAccess.ExecuteNonQuery(cmd);
            }
        }

    }
}
