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

using YAF.Core.BasePages;
using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The Poll controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Route("[controller]")]
public class PollController : ForumBaseController
{
    /// <summary>
    /// Removes the poll.
    /// </summary>
    /// <param name="pollId">The poll identifier.</param>
    /// <returns>IActionResult.</returns>
    [Route("RemovePoll/{pollId:int}")]
    public IActionResult RemovePoll(int pollId)
    {
        var poll = this.GetRepository<Poll>().GetById(pollId);

        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess
                                           && this.PageBoardContext.PageUserID != poll.UserID)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Info,
                new { info = InfoMessage.Invalid.ToType<int>() });
        }

        var topic = this.GetRepository<Topic>().GetSingle(t => t.PollID == pollId);

        this.GetRepository<Poll>().Remove(poll.ID);

        this.PageBoardContext.SessionNotify(this.GetText("REMOVEPOLL_MSG"), MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new { t = topic.ID, name = topic.TopicName });
    }

    /// <summary>
    /// Submit Poll Vote
    /// </summary>
    /// <param name="choiceId">The choice identifier.</param>
    /// <param name="topicId">The topic identifier.</param>
    /// <param name="pollId">The poll identifier.</param>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns>IActionResult.</returns>
    [Route("Vote/{choiceId:int}/{topicId:int}/{pollId:int}/{forumId:int}")]
    public IActionResult Vote(int choiceId, int topicId, int pollId, int forumId)
    {
        var poll = this.GetRepository<Poll>().GetById(pollId);
        var topic = this.GetRepository<Topic>().GetById(topicId);
        var forumAccess = this.GetRepository<ActiveAccess>()
            .GetSingle(x => x.UserID == this.PageBoardContext.PageUserID && x.ForumID == forumId);
        var userPollVotes = this.GetRepository<PollVote>().VoteCheck(poll.ID, this.PageBoardContext.PageUserID);

        var isClosed = this.Get<PollService>().IsPollClosed(poll);
        var canVote = forumAccess.VoteAccess && (userPollVotes.NullOrEmpty() || userPollVotes.TrueForAll(v => choiceId != v.ChoiceID))
                      || poll.PollFlags.AllowMultipleChoice && forumAccess.VoteAccess
                                                            && userPollVotes.TrueForAll(v => choiceId != v.ChoiceID);

        var redirect = this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new {t = topic.ID, name = topic.TopicName});

        if (!canVote)
        {
            this.PageBoardContext.SessionNotify(
                this.GetText("WARN_ALREADY_VOTED"),
                MessageTypes.warning);
            return redirect;
        }

        if (topic.TopicFlags.IsLocked)
        {
            this.PageBoardContext.SessionNotify(
                this.GetText("WARN_TOPIC_LOCKED"),
                MessageTypes.warning);
            return redirect;
        }

        if (isClosed)
        {
            this.PageBoardContext.SessionNotify(
                this.GetText("WARN_POLL_CLOSED"),
                MessageTypes.warning);

            return redirect;
        }

        this.GetRepository<Choice>().Vote(choiceId);

        this.GetRepository<PollVote>().Vote(choiceId, this.PageBoardContext.PageUserID, pollId);

        this.PageBoardContext.SessionNotify(this.GetText("INFO_VOTED"), MessageTypes.success);

        return redirect;
    }
}