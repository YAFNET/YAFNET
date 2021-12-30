/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Header.
    /// </summary>
    public partial class OpenAuthProviders : BaseUserControl
    {
        /// <summary>
        /// Gets the provider names.
        /// </summary>
        /// <returns>
        /// Returns the Provider Names
        /// </returns>
        public IEnumerable<string> GetProviderNames()
        {
            return IdentityHelper.GetProviderNames();
        }

        /// <summary>
        /// Do Login
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Login_OnCommand(object sender, CommandEventArgs e)
        {
            var providerName = e.CommandArgument.ToString();

            var redirectUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Account_Login, "auth={0}", providerName);

            IdentityHelper.RegisterExternalLogin(this.Context, providerName, redirectUrl);

            this.Response.StatusCode = 401;
            this.Response.End();
        }
    }
}