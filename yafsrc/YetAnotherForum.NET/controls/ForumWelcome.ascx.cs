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
namespace YAF.Controls
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The forum welcome control which shows the current Time and the Last Visit Time of the Current User.
    /// </summary>
    public partial class ForumWelcome : BaseUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ForumWelcome" /> class.
        /// </summary>
        public ForumWelcome()
        {
            this.PreRender += this.ForumWelcomePreRender;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the ForumWelcome control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumWelcomePreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.TimeNow.Text = this.GetTextFormatted("Current_Time", this.Get<IDateTime>().FormatTime(DateTime.UtcNow));

            var lastVisit = this.Get<IYafSession>().LastVisit;

            if (lastVisit.HasValue && lastVisit.Value != DateTimeHelper.SqlDbMinTime())
            {
                this.TimeLastVisit.Visible = true;
                this.TimeLastVisit.Text = this.GetTextFormatted(
                    "last_visit", this.Get<IDateTime>().FormatDateTime(lastVisit.Value));
            }
            else
            {
                this.TimeLastVisit.Visible = false;
            }
        }

        #endregion
    }
}