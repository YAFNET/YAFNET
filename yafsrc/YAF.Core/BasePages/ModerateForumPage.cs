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

namespace YAF.Core.BasePages
{
    #region Using

    using System;

    using YAF.Types;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The moderate forum page.
    /// </summary>
    public class ModerateForumPage : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
        /// </summary>
        public ModerateForumPage()
        : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
        /// </summary>
        /// <param name="transPage">
        /// The trans page.
        /// </param>
        public ModerateForumPage([CanBeNull] string transPage)
          : base(transPage)
        {
            this.Load += this.ModeratePage_Load;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets PageName.
        /// </summary>
        public override string PageName => $"moderate_{base.PageName}";

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the ModeratePage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ModeratePage_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // Only moderators are allowed here
            if (!this.PageContext.IsModeratorInAnyForum)
            {
                BuildLink.AccessDenied();
            }
        }

        #endregion
    }
}