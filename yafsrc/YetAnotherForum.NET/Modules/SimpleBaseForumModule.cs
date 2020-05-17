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

    using YAF.Core.BaseModules;
    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The simple base forum module.
    /// </summary>
    public class SimpleBaseForumModule : BaseForumModule
    {
        #region Properties

        /// <summary>
        ///   Gets CurrentForumPage.
        /// </summary>
        public ForumPage CurrentForumPage => this.PageContext.CurrentForumPage;

        /// <summary>
        ///   Gets ForumControl.
        /// </summary>
        public Forum ForumControl => (Forum)this.ForumControlObj;

        /// <summary>
        ///   Gets ForumPageType.
        /// </summary>
        public ForumPages ForumPageType => this.PageContext.ForumPageType;

        #endregion

        #region Public Methods

        /// <summary>
        /// The initialization.
        /// </summary>
        public override void Init()
        {
            this.ForumControl.BeforeForumPageLoad += this.ForumControl_BeforeForumPageLoad;
            this.ForumControl.AfterForumPageLoad += this.ForumControl_AfterForumPageLoad;
            this.InitForum();
        }

        /// <summary>
        /// The initialization after page.
        /// </summary>
        public virtual void InitAfterPage()
        {
        }

        /// <summary>
        /// The initialization before page.
        /// </summary>
        public virtual void InitBeforePage()
        {
        }

        /// <summary>
        /// The initialization forum.
        /// </summary>
        public virtual void InitForum()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The forum control_ after forum page load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumControl_AfterForumPageLoad([NotNull] object sender, [NotNull] AfterForumPageLoad e)
        {
            this.InitAfterPage();
        }

        /// <summary>
        /// The forum control_ before forum page load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ForumControl_BeforeForumPageLoad([NotNull] object sender, [NotNull] BeforeForumPageLoad e)
        {
            this.InitBeforePage();
        }

        #endregion
    }
}