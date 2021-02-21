/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Pages.Admin
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    ///    The version info page.
    /// </summary>
    public partial class version : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                try
                {
                    var version = this.Get<IDataCache>().GetOrSet(
                        "LatestVersion",
                        () => this.Get<ILatestInformation>().GetLatestVersion(),
                        TimeSpan.FromDays(1));

                    string lastVersion = version.Version;
                    var lastVersionDate = (DateTime)version.VersionDate;

                    this.LatestVersion.Text = this.GetTextFormatted("LATEST_VERSION", lastVersion, this.Get<IDateTime>().FormatDateShort(lastVersionDate));

                    this.UpgradeVersionHolder.Visible = lastVersionDate.ToUniversalTime() > BoardInfo.AppVersionDate.ToUniversalTime();

                    this.Download.NavigateUrl = version.UpgradeUrl;
                    this.Download.DataBind();
                }
                catch (Exception)
                {
                    this.LatestVersion.Visible = false;
                }

                this.RunningVersion.Text = this.GetTextFormatted(
                    "RUNNING_VERSION",
                    BoardInfo.AppVersionName,
                    this.Get<IDateTime>().FormatDateShort(BoardInfo.AppVersionDate));
            }

            this.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, BuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_VERSION", "TITLE"), string.Empty);

            this.Page.Header.Title = this.GetText("ADMIN_VERSION", "TITLE");
        }

        #endregion
    }
}