/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;

    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.Dapper;

    using YAF.Classes.Data;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Buddy repository extensions.
    /// </summary>
    public static class BuddyRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets all the buddies of a certain user.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="fromUserID">From user identifier.</param>
        /// <returns>
        /// The <see cref="DataTable" /> containing the buddy list.
        /// </returns>
        public static DataTable List(this IRepository<Buddy> repository, int fromUserID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.buddy_list(FromUserID: fromUserID);
        }
        /*
        /// <summary>
        /// Adds a buddy request. (Should be approved later by "ToUserID")
        /// </summary>
        /// <param name="FromUserID">The from user id.</param>
        /// <param name="ToUserID">The to user id.</param>
        /// <param name="useDisplayName">Display name of the use.</param>
        /// <returns>
        /// The name of the second user + Whether this request is approved or not.
        /// </returns>
        public static string[] AddRequest(
            this IRepository<Buddy> repository, 
            int fromUserID,
            int toUserID,
            bool useDisplayName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            bool approved = false;
            string paramOutput = null;

            repository.DbFunction.Query.buddy_addrequest(
                    FromUserID: fromUserID,
                    ToUserID: toUserID,
                    UTCTIMESTAMP: DateTime.UtcNow,
                    approved: out approved,
                    UseDisplayName: useDisplayName,
                    paramOutput: out paramOutput);

            //return new[] { paramOutput, approved.ToString() };

            
        }*/
        
        /// <summary>
        /// Approves a buddy request.
        /// </summary>
        /// <param name="FromUserID">
        /// The from user id.
        /// </param>
        /// <param name="ToUserID">
        /// The to user id.
        /// </param>
        /// <param name="Mutual">
        /// Should the requesting user (ToUserID) be added to FromUserID's buddy list too?
        /// </param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        public static string ApproveRequest(this IRepository<Buddy> repository, int fromUserID, int toUserID, bool mutual, bool useDisplayName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var parameters = new DynamicParameters();

            parameters.Add("@FromUserID", fromUserID);
            parameters.Add("@ToUserID", toUserID);
            parameters.Add("@mutual", mutual);
            parameters.Add("@UTCTIMESTAMP", DateTime.UtcNow);
            parameters.Add("@UseDisplayName", useDisplayName);
            parameters.Add("@paramOutput", dbType: DbType.String, direction: ParameterDirection.Output);

            repository.DbAccess.GetCommand().Execute("buddy_addrequest", parameters, CommandType.StoredProcedure);

            return parameters.Get<string>("@paramOutput");
        }
        /*
        /// <summary>
        /// Denies a buddy request.
        /// </summary>
        /// <param name="FromUserID">The from user id.</param>
        /// <param name="ToUserID">The to user id.</param>
        /// <param name="useDisplayName">Display name of the use.</param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        [NotNull]
        public static string DenyRequest(this IRepository<Buddy> repository, int fromUserID, int toUserID, bool useDisplayName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            string paramOutput = null;

            repository.DbFunction.Query.buddy_denyrequest(
                    FromUserID: fromUserID,
                    ToUserID: toUserID,
                    UseDisplayName: useDisplayName,
                    paramOutput: out paramOutput);

            return paramOutput;
        }

        

        /// <summary>
        /// Removes the "ToUserID" from "FromUserID"'s buddy list.
        /// </summary>
        /// <param name="FromUserID">The from user id.</param>
        /// <param name="ToUserID">The to user id.</param>
        /// <param name="useDisplayName">Display name of the use.</param>
        /// <returns>
        /// The name of the second user.
        /// </returns>
        [NotNull]
        public static string Remove(this IRepository<Buddy> repository, int fromUserID, int toUserID, bool useDisplayName)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            string paramOutput = null;

            var test = repository.DbFunction.Query.buddy_remove(
                    FromUserID: fromUserID,
                    ToUserID: toUserID,
                    UseDisplayName: useDisplayName,
                    paramOutput: out paramOutput);


           return test.ToString();
        }*/

        #endregion
    }
}