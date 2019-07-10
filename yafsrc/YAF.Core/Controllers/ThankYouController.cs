﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Core.Controllers
{
    using System.Web.Http;

    using YAF.Classes;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers.StringUtils;

    /// <summary>
    /// The YAF ThankYou controller.
    /// </summary>
    [RoutePrefix("api")]
    public class ThankYouController : ApiController, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        ///   Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => YafContext.Current.ServiceLocator;

        #endregion

        /// <summary>
        /// Add Thanks to post
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [Route("ThankYou/AddThanks/{messageId}")]
        [HttpPost]
        public IHttpActionResult AddThanks([NotNull] int messageId)
        {
            var membershipUser = UserMembershipHelper.GetUser();

            if (membershipUser == null)
            {
                return this.NotFound();
            }

            var username = this.GetRepository<Thanks>().AddMessageThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey),
                messageId,
                this.Get<YafBoardSettings>().EnableDisplayName);

            // if the user is empty, return a null object...
            return username.IsNotSet()
                       ? (IHttpActionResult)this.NotFound()
                       : this.Ok(
                           this.Get<IThankYou>().CreateThankYou(
                               new UnicodeEncoder().XSSEncode(username),
                               "BUTTON_THANKSDELETE",
                               "BUTTON_THANKSDELETE_TT",
                               messageId));
        }

        /// <summary>
        /// This method is called asynchronously when the user clicks on "Remove Thank" button.
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <returns>
        /// Returns ThankYou Info
        /// </returns>
        [Route("ThankYou/RemoveThanks/{messageId}")]
        [HttpPost]
        public IHttpActionResult RemoveThanks([NotNull] int messageId)
        {
            var username = this.GetRepository<Thanks>().RemoveMessageThanks(
                UserMembershipHelper.GetUserIDFromProviderUserKey(UserMembershipHelper.GetUser().ProviderUserKey),
                messageId,
                this.Get<YafBoardSettings>().EnableDisplayName);

            return this.Ok(
                this.Get<IThankYou>().CreateThankYou(username, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageId));
        }
    }
}
