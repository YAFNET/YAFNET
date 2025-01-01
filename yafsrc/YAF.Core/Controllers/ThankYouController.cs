﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Core.Controllers;

using System.Web.Http;

using YAF.Core.Model;
using YAF.Core.Utilities.StringUtils;
using YAF.Types.Models;

/// <summary>
/// The YAF ThankYou controller.
/// </summary>
[RoutePrefix("api")]
public class ThankYouController : ApiController, IHaveServiceLocator
{
    /// <summary>
    ///   Gets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Add Thanks to post
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    [Route("ThankYou/GetThanks/{messageId}")]
    [HttpPost]
    public IHttpActionResult GetThanks(int messageId)
    {
        if (BoardContext.Current.IsGuest)
        {
            return this.NotFound();
        }

        var message = this.GetRepository<Message>().GetById(messageId);

        var userName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        // if the user is empty, return a null object...
        return userName.IsNotSet()
                   ? this.NotFound()
                   : this.Ok(
                       this.Get<IThankYou>().GetThankYou(
                           new UnicodeEncoder().XSSEncode(userName),
                           "BUTTON_THANKSDELETE",
                           "BUTTON_THANKSDELETE_TT",
                           messageId));
    }

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
    public IHttpActionResult AddThanks(int messageId)
    {
        var membershipUser = BoardContext.Current.MembershipUser;

        if (membershipUser is null)
        {
            return this.NotFound();
        }

        var fromUserId = BoardContext.Current.PageUserID;

        var message = this.GetRepository<Message>().GetById(messageId);

        var userName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        if (this.GetRepository<Thanks>().Exists(x => x.MessageID == messageId && x.ThanksFromUserID == fromUserId))
        {
            return this.NotFound();
        }

        this.GetRepository<Thanks>().AddMessageThanks(fromUserId, message.UserID, messageId);

        this.Get<IActivityStream>().AddThanksReceivedToStream(message.UserID, message.TopicID, messageId, fromUserId);
        this.Get<IActivityStream>().AddThanksGivenToStream(fromUserId, message.TopicID, messageId, message.UserID);

        // if the user is empty, return a null object...
        return userName.IsNotSet()
                   ? this.NotFound()
                   : this.Ok(
                       this.Get<IThankYou>().CreateThankYou(
                           new UnicodeEncoder().XSSEncode(userName),
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
    public IHttpActionResult RemoveThanks(int messageId)
    {
        var message = this.GetRepository<Message>().GetById(messageId);

        var userName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        this.GetRepository<Thanks>().RemoveMessageThanks(
            BoardContext.Current.PageUserID,
            messageId);

        this.GetRepository<Activity>()
            .Delete(a => a.MessageID == messageId && (a.Flags == 1024 || a.Flags == 2048));

        return this.Ok(
            this.Get<IThankYou>().CreateThankYou(userName, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageId));
    }
}