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
namespace YAF.Modules
{
    #region Using

    using System;

    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// Suspend Check Forum Module
    /// </summary>
    [Module("Suspend Check Module", "Tiny Gecko", 1)]
    public class SuspendCheckForumModule : SimpleBaseForumModule
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SuspendCheckForumModule"/> class.
        /// </summary>
        /// <param name="preLoadPage">
        /// The pre load page.
        /// </param>
        public SuspendCheckForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> preLoadPage)
        {
            preLoadPage.HandleEvent += this._preLoadPage_HandleEvent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check if the user needs to be un-suspended or redirected to the info page
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void _preLoadPage_HandleEvent(
            [NotNull] object sender,
            [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
        {
            // check for suspension if enabled...
            if (!this.PageContext.Globals.IsSuspendCheckEnabled)
            {
                return;
            }

            if (!this.PageContext.IsSuspended)
            {
                return;
            }

            if (this.Get<IDateTimeService>().GetUserDateTime(this.PageContext.SuspendedUntil)
                <= this.Get<IDateTimeService>().GetUserDateTime(DateTime.UtcNow))
            {
                this.GetRepository<User>().Suspend(this.PageContext.PageUserID);

                this.Get<ISendNotification>().SendUserSuspensionEndedNotification(
                    this.PageContext.User.Email,
                    this.PageContext.User.DisplayOrUserName());

                this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageContext.PageUserID));
            }
            else
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Suspended);
            }
        }

        #endregion
    }
}