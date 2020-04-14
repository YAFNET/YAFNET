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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Globalization;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The forum welcome control which shows the current Time and the Last Visit Time of the Current User.
    /// </summary>
    public partial class BoardAnnouncement : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The on pre render.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            if (this.Get<BoardSettings>().BoardAnnouncement.IsNotSet())
            {
                this.Visible = false;
                return;
            }

            var dateTime = Convert.ToDateTime(
                this.Get<BoardSettings>().BoardAnnouncementUntil,
                CultureInfo.InvariantCulture); 

            if (dateTime <= DateTime.Now)
            {
                var boardSettings = this.Get<BoardSettings>();

                boardSettings.BoardAnnouncementUntil = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
                boardSettings.BoardAnnouncement = string.Empty;

                // save the settings to the database
                ((LoadBoardSettings)boardSettings).SaveRegistry();

                // Reload forum settings
                this.PageContext.BoardSettings = null;

                // delete no show
                this.Visible = false;
                return;
            }

            this.Badge.CssClass = $"badge badge-{this.Get<BoardSettings>().BoardAnnouncementType} mr-1";

            this.Announcement.CssClass = $"alert alert-{this.Get<BoardSettings>().BoardAnnouncementType} alert-dismissible";
            this.Message.Text = this.Get<BoardSettings>().BoardAnnouncement;

            this.DataBind();

            base.OnPreRender(e);
        }

        #endregion
    }
}