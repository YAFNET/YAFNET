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
        public static void ChangePassword(string appName, string username, string oldPassword, string newPassword)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static void ChangePasswordQuestionAndAnswer(string appName, string username, string passwordQuestion, string passwordAnswer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static void CreateUser(string appName, string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static void DeleteUser(string appName, string username, bool deleteAllRelatedData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public static DataTable FindUsersByEmail(string appName, string emailToMatch, int pageIndex, int pageSize)
        {
            throw new Exception("The method or operation is not implemented.");

        }

        public static DataTable FindUsersByName(string appName, string usernameToMatch, int pageIndex, int pageSize)
        {

            throw new Exception("The method or operation is not implemented.");
        }

        public static DataTable GetAllUsers(string appName, int pageIndex, int pageSize)
        {
            using (SqlCommand cmd = new SqlCommand("yafmp_getallusers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@pageIndex", pageSize);
                return DBAccess.GetData(cmd);
            }
        }

        public static DataRow GetNumberOfUsersOnline(string appName, int TimeWindow)
        {
            using (SqlCommand cmd = new SqlCommand("yafmp_getnumberofusersonline"))
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
            using (SqlCommand cmd = new SqlCommand("yafmp_getuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserName", userName);
                cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                return DBAccess.GetData(cmd).Rows[0];
            }

        }

        public static DataRow GetUser(string appName, object providerUserKey, bool userIsOnline)
        {
            using (SqlCommand cmd = new SqlCommand("yafmp_getuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
                cmd.Parameters.AddWithValue("@UserIsOnline", userIsOnline);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                return DBAccess.GetData(cmd).Rows[0];
            }

        }

        public static DataTable GetUserPasswordInfo(string appName,string username,bool updateUser)
        {
            using (SqlCommand cmd = new SqlCommand("GetUserPasswordInfo"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApplicationName", appName);
                // Nonstandard args
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@UpdateUser", updateUser);
                cmd.Parameters.AddWithValue("@CurrentTimeUtc", DateTime.UtcNow);
                return DBAccess.GetData(cmd);
            }

        }

        public static DataTable GetUserNameByEmail(string appName, string email)
        {
            using (SqlCommand cmd = new SqlCommand("yafmp_getusernamebyemail"))
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
            using (SqlCommand cmd = new SqlCommand("yafmp_resetpassword"))
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
            using (SqlCommand cmd = new SqlCommand("yafmp_unlockuser"))
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
            using (SqlCommand cmd = new SqlCommand("yafmp_updateuser"))
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
