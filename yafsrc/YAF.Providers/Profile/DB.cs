/* Yet Another Forum.NET
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

using YAF.Providers.Utils;
using YAF.Types.Objects;

namespace YAF.Providers.Profile
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;
    using YAF.Classes.Pattern;
    using YAF.Types.Extensions;
    using YAF.Classes.Data;
    using YAF.Types.Interfaces.Data;
    using YAF.Types;

    #endregion

    /// <summary>
    /// The db.
    /// </summary>
    public class DB : BaseProviderDb
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DB" /> class.
        /// </summary>
        public DB()
            : base(YafProfileProvider.ConnStrAppKeyName)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Current.
        /// </summary>
        public static DB Current
        {
            get { return PageSingleton<DB>.Instance; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add profile column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="columnType">
        /// The column type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public void AddProfileColumn([NotNull] string name, SqlDbType columnType, int size)
        {
            // get column type...
            string type = columnType.ToString();

            if (size > 0)
            {
                type += "(" + size + ")";
            }

            string sql = "ALTER TABLE {0} ADD [{1}] {2} NULL".FormatWith(
                DbHelpers.GetObjectName("prov_Profile"), name, type);

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The delete inactive profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// The delete inactive profiles.
        /// </returns>
        public int DeleteInactiveProfiles([NotNull] object appName, [NotNull] object inactiveSinceDate)
        {
            using (SqlCommand cmd = DbHelpers.GetCommand("prov_profile_deleteinactive"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("InactiveSinceDate", inactiveSinceDate);
                return Convert.ToInt32(this.DbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The delete profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userNames">
        /// The user names.
        /// </param>
        /// <returns>
        /// The delete profiles.
        /// </returns>
        public int DeleteProfiles([NotNull] object appName, [NotNull] object userNames)
        {
            using (SqlCommand cmd = DbHelpers.GetCommand("prov_profile_deleteprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("UserNames", userNames);
                return Convert.ToInt32(this.DbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The get number inactive profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// The get number inactive profiles.
        /// </returns>
        public int GetNumberInactiveProfiles([NotNull] object appName, [NotNull] object inactiveSinceDate)
        {
            using (SqlCommand cmd = DbHelpers.GetCommand("prov_profile_getnumberinactiveprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("InactiveSinceDate", inactiveSinceDate);
                return Convert.ToInt32(this.DbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The get profile structure.
        /// </summary>
        /// <returns>
        /// </returns>
        public DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(DbHelpers.GetObjectName("prov_Profile"));

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return this.DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The get profiles.
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
        /// <param name="userNameToMatch">
        /// The user name to match.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// </returns>
        public DataSet GetProfiles([NotNull] object appName, [NotNull] object pageIndex, [NotNull] object pageSize,
            [NotNull] object userNameToMatch, [NotNull] object inactiveSinceDate)
        {
            using (SqlCommand cmd = DbHelpers.GetCommand("prov_profile_getprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ApplicationName", appName);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("UserNameToMatch", userNameToMatch);
                cmd.Parameters.AddWithValue("InactiveSinceDate", inactiveSinceDate);
                return this.DbAccess.GetDataset(cmd);
            }
        }

        /// <summary>
        /// The get db type and size from string.
        /// </summary>
        /// <param name="providerData">
        /// The provider data.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The get db type and size from string.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
        {
            return LegacyDb.GetDbTypeAndSizeFromString(providerData, out dbType, out size);
        }

        /// <summary>
        /// The get provider user key.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The get provider user key.
        /// </returns>
        public object GetProviderUserKey([NotNull] object appName, [NotNull] object username)
        {
            DataRow row = Membership.DB.Current.GetUser(appName.ToString(), null, username.ToString(), false);

            if (row != null)
            {
                return row["UserID"];
            }

            return null;
        }

        /// <summary>
        /// The set profile properties.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="settingsColumnsList">
        /// The settings columns list.
        /// </param>
        public void SetProfileProperties([NotNull] object appName, [NotNull] object userID,
            [NotNull] SettingsPropertyValueCollection values, [NotNull] List<SettingsPropertyColumn> settingsColumnsList)
        {
            using (var cmd = new SqlCommand())
            {
                string table = DbHelpers.GetObjectName("prov_Profile");

                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                sqlCommand.Append(" WHERE UserId = @UserID) ");
                cmd.Parameters.AddWithValue("@UserID", userID);

                // Build up strings used in the query
                var columnStr = new StringBuilder();
                var valueStr = new StringBuilder();
                var setStr = new StringBuilder();
                int count = 0;

                foreach (SettingsPropertyColumn column in settingsColumnsList)
                {
                    // only write if it's dirty
                    if (values[column.Settings.Name].IsDirty)
                    {
                        columnStr.Append(", ");
                        valueStr.Append(", ");
                        columnStr.Append(column.Settings.Name);
                        string valueParam = "@Value" + count;
                        valueStr.Append(valueParam);
                        cmd.Parameters.AddWithValue(valueParam, values[column.Settings.Name].PropertyValue);

                        if (column.DataType != SqlDbType.Timestamp)
                        {
                            if (count > 0)
                            {
                                setStr.Append(",");
                            }

                            setStr.Append(column.Settings.Name);
                            setStr.Append("=");
                            setStr.Append(valueParam);
                        }

                        count++;
                    }
                }

                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.Parameters.AddWithValue("@LastUpdatedDate", DateTime.UtcNow);

                sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = '").Append(userID.ToString()).Append("'");

                sqlCommand.Append(" END ELSE BEGIN INSERT ")
                    .Append(table)
                    .Append(" (UserId")
                    .Append(columnStr.ToString());
                sqlCommand.Append(") VALUES ('")
                    .Append(userID.ToString())
                    .Append("'")
                    .Append(valueStr.ToString())
                    .Append(
                        ") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        #endregion

        /*
		public static void ValidateAddColumnInProfile( string columnName, SqlDbType columnType )
		{
			SqlCommand cmd = new SqlCommand( sprocName );
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue( "@ApplicationName", appName );
			cmd.Parameters.AddWithValue( "@Username", username );
			cmd.Parameters.AddWithValue( "@IsUserAnonymous", isAnonymous );

			return cmd;
		}
		*/
    }
}