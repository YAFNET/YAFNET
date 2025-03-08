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

using Microsoft.AspNetCore.Authorization;

namespace YAF.Core.Controllers;

using System;
using System.Collections.Generic;

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Posts controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Authorize]
[Route("[controller]")]
public class PostsController : ForumBaseController
{
    /// <summary>
    /// Remove or Mark Post as Answer
    /// </summary>
    [HttpGet]
    [Route("MarkAsAnswer/{m:int}")]
    public IActionResult MarkAsAnswer(int m)
    {
        try
        {
            var messages = this.Get<ISessionService>().GetPageData<List<PagedMessage>>();

            var source = messages.Find(message => message.MessageID == m);

            if (source == null)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
            }

            if (source.IsDeleted || this.PageBoardContext.IsGuest || this.PageBoardContext.MembershipUser == null
                || !this.PageBoardContext.PageUserID.Equals(source.TopicOwnerID)
                || source.UserID.Equals(this.PageBoardContext.PageUserID))
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Post,
                    new { m, name = source.Topic, t = source.TopicID });
            }


            var messageFlags = new MessageFlags(source.Flags);

            if (messageFlags.IsAnswer)
            {
                // Remove Current Message
                messageFlags.IsAnswer = false;

                this.GetRepository<Message>().UpdateFlags(source.MessageID, messageFlags.BitValue);

                this.GetRepository<Topic>().RemoveAnswerMessage(source.TopicID);
            }
            else
            {
                // Check for duplicates
                var answerMessageId = this.GetRepository<Topic>().GetAnswerMessage(source.TopicID);

                if (answerMessageId != null)
                {
                    var message = this.GetRepository<Message>().GetById(answerMessageId.Value);

                    var oldMessageFlags = new MessageFlags(message.Flags) { IsAnswer = false };

                    this.GetRepository<Message>().UpdateFlags(message.ID, oldMessageFlags.BitValue);
                }

                messageFlags.IsAnswer = true;

                this.GetRepository<Topic>().SetAnswerMessage(source.TopicID, source.MessageID);

                this.GetRepository<Message>().UpdateFlags(source.MessageID, messageFlags.BitValue);
            }

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post,
                new { m, name = source.Topic, t = source.TopicID });
        }
        catch (Exception)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Post, new { m, name = "Topic" });
        }
    }
}