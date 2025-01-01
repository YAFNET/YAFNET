/* Yet Another Forum.NET
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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF ThankYou controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class ThankYouController : ForumBaseController
{
    /// <summary>
    /// Add Thanks to post
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <returns>
    /// Returns ThankYou Info
    /// </returns>
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ThankYouInfo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("GetThanks/{messageId:int}")]
    public async Task<ActionResult<ThankYouInfo>> GetThanks(int messageId)
    {
        if (this.PageBoardContext.IsGuest)
        {
            return this.NotFound();
        }

        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

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
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ThankYouInfo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("AddThanks/{messageId:int}")]
    public ActionResult<ThankYouInfo> AddThanks(int messageId)
    {
        var fromUserId = BoardContext.Current.PageUserID;

        if (this.PageBoardContext.IsGuest)
        {
            return this.NotFound();
        }

        var message = this.GetRepository<Message>().GetById(messageId);

        var userName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        if (this.GetRepository<Thanks>().Exists(x => x.MessageID == messageId && x.ThanksFromUserID == fromUserId))
        {
            return this.NotFound();
        }

        this.GetRepository<Thanks>().AddMessageThanks(fromUserId, message.UserID, messageId);

        this.Get<IActivityStream>().AddThanksReceivedToStream(
            message.UserID,
            message.TopicID,
            messageId,
            fromUserId);

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
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ThankYouInfo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("RemoveThanks/{messageId:int}")]
    public async Task<ActionResult<ThankYouInfo>> RemoveThanks([FromRoute] int messageId)
    {
        var message = await this.GetRepository<Message>().GetByIdAsync(messageId);

        var userName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        this.GetRepository<Thanks>().RemoveMessageThanks(
            this.PageBoardContext.PageUserID,
            messageId);

        await this.GetRepository<Activity>()
            .DeleteAsync(a => a.MessageID == messageId && (a.Flags == 1024 || a.Flags == 2048));

        return this.Ok(this.Get<IThankYou>().CreateThankYou(userName, "BUTTON_THANKS", "BUTTON_THANKS_TT", messageId));
    }
}