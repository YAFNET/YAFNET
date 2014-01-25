/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;

    using YAF.Controls;
    using YAF.Core;
    using YAF.RegisterV2;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///     Summary description for register.
    /// </summary>
    public partial class version : AdminPage
    {
        #region Fields

        /// <summary>
        ///     The _last version.
        /// </summary>
        private long _lastVersion;

        /// <summary>
        ///     The _last version date.
        /// </summary>
        private DateTime _lastVersionDate;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets LastVersion.
        /// </summary>
        protected string LastVersion
        {
            get
            {
                return YafForumInfo.AppVersionNameFromCode(this._lastVersion);
            }
        }

        /// <summary>
        ///     Gets LastVersionDate.
        /// </summary>
        protected string LastVersionDate
        {
            get
            {
                return this.Get<IDateTime>().FormatDateShort(this._lastVersionDate);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The page_ load.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var reg = new RegisterV2())
                {
                    this._lastVersion = reg.LatestVersion();
                    this._lastVersionDate = reg.LatestVersionDate();
                }

                this.Upgrade.Visible = this._lastVersion > YafForumInfo.AppVersionCode;

                this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
                this.PageLinks.AddLink(this.GetText("ADMIN_VERSION", "TITLE"), string.Empty);

                this.Page.Header.Title = "{0} - {1}".FormatWith(
                    this.GetText("ADMIN_ADMIN", "Administration"),
                    this.GetText("ADMIN_VERSION", "TITLE"));

                this.RunningVersion.Text = this.GetTextFormatted(
                    "RUNNING_VERSION",
                    YafForumInfo.AppVersionName,
                    this.Get<IDateTime>().FormatDateShort(YafForumInfo.AppVersionDate));

                this.LatestVersion.Text = this.GetTextFormatted(
                    "LATEST_VERSION",
                    this.LastVersion,
                    this.LastVersionDate);
            }

            this.DataBind();
        }

        #endregion
    }
}