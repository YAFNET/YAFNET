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

namespace YAF.Modules
{
    #region Using

    using System;

    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Attributes;

    #endregion

    /// <summary>
    /// The time ago module.
    /// </summary>
    [Module("Time Ago Javascript Loading Module", "Tiny Gecko", 1)]
    public class TimeAgoForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.CurrentForumPagePreRender;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the CurrentForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPagePreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.PageContext.BoardSettings.ShowRelativeTime
                || this.PageContext.Vars.ContainsKey("RegisteredTimeago"))
            {
                return;
            }

            BoardContext.Current.PageElements.RegisterJsBlockStartup("timeagoloadjs", JavaScriptBlocks.MomentLoadJs);
            this.PageContext.Vars["RegisteredTimeago"] = true;
        }

        #endregion
    }
}