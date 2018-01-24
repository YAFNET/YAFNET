/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Core.Services.Startup
{
    #region Using

    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core.Tasks;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///     The yaf initialize db.
    /// </summary>
    public class StartupInitializeDb : BaseStartupService, ICriticalStartupService
    {
        #region Properties

        /// <summary>
        ///     Gets InitVarName.
        /// </summary>
        [NotNull]
        protected override string InitVarName
        {
            get
            {
                return "YafInitializeDb_Init";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The run service.
        /// </summary>
        /// <returns>
        ///     The run service.
        /// </returns>
        protected override bool RunService()
        {
            // init the db...
            string errorString;
            var debugging = false;

#if DEBUG
            debugging = true;
#endif

            if (HttpContext.Current == null)
            {
                return true;
            }

            var response = YafContext.Current.Get<HttpResponseBase>();

            if (Config.ConnectionString == null)
            {
                // attempt to create a connection string...
                response.Redirect("{0}install/default.aspx".FormatWith(YafForumInfo.ForumClientFileRoot));
                
                return false;
            }

            // attempt to init the db...
            if (!LegacyDb.forumpage_initdb(out errorString, debugging))
            {
                // unable to connect to the DB...
                YafContext.Current.Get<HttpSessionStateBase>()["StartupException"] = errorString;
               
                response.Redirect("{0}error.aspx".FormatWith(YafForumInfo.ForumClientFileRoot));
                
                return false;
            }

            // step 2: validate the database version...
            var redirectString = LegacyDb.forumpage_validateversion(YafForumInfo.AppVersion);

            if (!redirectString.IsSet())
            {
                return true;
            }

            response.Redirect("{0}{1}".FormatWith(YafForumInfo.ForumClientFileRoot, redirectString));
            return false;
        }

        #endregion
    }
}