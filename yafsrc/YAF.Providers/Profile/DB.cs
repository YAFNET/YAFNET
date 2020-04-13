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

namespace YAF.Providers.Profile
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Text;

    using YAF.Configuration.Pattern;
    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Core.Data;
    using YAF.Data.MsSql.Functions;
    using YAF.Providers.Utils;
    using YAF.Types;
    using YAF.Types.Extensions.Data;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    /// The DB
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
        public static DB Current => PageSingleton<DB>.Instance;

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
            var type = columnType.ToString();

            if (size > 0)
            {
                type += $"({size})";
            }

            var sql = $"ALTER TABLE {CommandTextHelpers.GetObjectName("prov_Profile")} ADD [{name}] {type} NULL";

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
            return BoardContext.Current.Get<IDbFunction>().Scalar.prov_profile_deleteinactive(
                ApplicationName: appName,
                InactiveSinceDate: inactiveSinceDate);
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
            return BoardContext.Current.Get<IDbFunction>().Scalar
                .prov_profile_deleteprofiles(ApplicationName: appName, UserNames: userNames);
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
            return BoardContext.Current.Get<IDbFunction>().Scalar.prov_profile_getnumberinactiveprofiles(
                ApplicationName: appName,
                InactiveSinceDate: inactiveSinceDate);
        }

        /// <summary>
        /// The get profile structure.
        /// </summary>
        /// <returns>
        /// </returns>
        public DataTable GetProfileStructure()
        {
            var sql = $@"SELECT TOP 1 * FROM {CommandTextHelpers.GetObjectName("prov_Profile")}";

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
        public DataSet GetProfiles(
            [NotNull] object appName,
            [NotNull] object pageIndex,
            [NotNull] object pageSize,
            [NotNull] object userNameToMatch,
            [NotNull] object inactiveSinceDate)
        {
            return BoardContext.Current.Get<IDbFunction>().GetDataSet.prov_profile_getprofiles(
                ApplicationName: appName,
                PageIndex: pageIndex,
                PageSize: pageSize,
                UserNameToMatch: userNameToMatch,
                InactiveSinceDate: inactiveSinceDate);
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
            return MsSqlGetStatsFunction.GetDbTypeAndSizeFromString(providerData, out dbType, out size);
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
            var row = Membership.DB.Current.GetUser(appName.ToString(), null, username.ToString(), false);

            return row?["UserID"];
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
        public void SetProfileProperties(
            [NotNull] object appName,
            [NotNull] object userID,
            [NotNull] SettingsPropertyValueCollection values,
            [NotNull] List<SettingsPropertyColumn> settingsColumnsList)
        {
            using (var cmd = new SqlCommand())
            {
                var table = CommandTextHelpers.GetObjectName("prov_Profile");

                var sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                sqlCommand.Append(" WHERE UserId = @UserID) ");
                cmd.Parameters.AddWithValue("@UserID", userID);

                // Build up strings used in the query
                var columnStr = new StringBuilder();
                var valueStr = new StringBuilder();
                var setStr = new StringBuilder();
                var count = 0;

                foreach (var column in settingsColumnsList)
                {
                    // only write if it's dirty
                    if (values[column.Settings.Name].IsDirty)
                    {
                        columnStr.Append(", ");
                        valueStr.Append(", ");
                        columnStr.Append(column.Settings.Name);
                        var valueParam = $"@Value{count}";
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

                sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr);
                sqlCommand.Append(" WHERE UserId = '").Append(userID).Append("'");

                sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserId")
                    .Append(columnStr);
                sqlCommand.Append(") VALUES ('").Append(userID).Append("'").Append(valueStr)
                    .Append(") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                this.DbAccess.ExecuteNonQuery(cmd);
            }
        }

        #endregion
    }
}