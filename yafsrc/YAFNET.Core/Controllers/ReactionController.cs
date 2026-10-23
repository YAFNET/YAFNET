/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The YAF Reaction controller.
/// </summary>
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class ReactionController : ForumBaseController
{
    /// <summary>
    /// Reacts to a post: adds the reaction, changes the reaction of the user to the new one,
    ///   or removes it if the user clicks his reaction again.
    /// </summary>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="reaction">
    /// The reaction (see <see cref="ReactionType"/>).
    /// </param>
    /// <returns>
    /// Returns the Reaction Info
    /// </returns>
    [ValidateAntiForgeryToken]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReactionInfo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("React/{messageId:int}/{reaction:int}")]
    public async Task<ActionResult<ReactionInfo>> React([FromRoute] int messageId, [FromRoute] int reaction)
    {
        var userId = this.PageBoardContext.PageUserID;

        if (this.PageBoardContext.IsGuest || !Enum.IsDefined(typeof(ReactionType), reaction))
        {
            return this.NotFound();
        }

        // only messages the user can read, and not the own ones
        var message = this.GetRepository<Message>().GetMessageWithAccess(messageId, userId)?.Item2;

        if (message is null || message.UserID == userId)
        {
            return this.NotFound();
        }

        var existing = this.GetRepository<Thanks>()
            .GetSingle(x => x.MessageID == messageId && x.ThanksFromUserID == userId);

        if (existing is null)
        {
            this.GetRepository<Thanks>().AddMessageThanks(userId, message.UserID, messageId, (ReactionType)reaction);

            this.Get<IActivityStream>().AddThanksReceivedToStream(
                message.UserID,
                message.TopicID,
                messageId,
                userId);

            this.Get<IActivityStream>().AddThanksGivenToStream(userId, message.TopicID, messageId, message.UserID);
        }
        else if (existing.Reaction == reaction)
        {
            this.GetRepository<Thanks>().RemoveMessageThanks(userId, messageId);

            // only the activities of this reaction: "received" (from this user) and "given" (by this user)
            await this.GetRepository<Activity>()
                .DeleteAsync(
                    a => a.MessageID == messageId
                         && (a.Flags == 1024 && a.FromUserID == userId || a.Flags == 2048 && a.UserID == userId));
        }
        else
        {
            this.GetRepository<Thanks>().UpdateMessageReaction(userId, messageId, (ReactionType)reaction);
        }

        return this.Ok(this.Get<IReactions>().GetReactionInfo(messageId));
    }
}
