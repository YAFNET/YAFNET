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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// The Poll Edit Page.
/// </summary>
public class PollEditModel : ForumPage
{
    /// <summary>
    /// Gets a value indicating whether the topic should be approved (it is not shown yet,
    /// the user gets the info instead).
    /// </summary>
    public bool TopicUnapproved { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PollEditModel"/> class.
    ///   Initializes a new instance of the ReportPost class.
    /// </summary>
    public PollEditModel()
        : base("POLLEDIT", ForumPages.PollEdit)
    {
    }

    /// <summary>
    /// Gets or sets PollID (the poll of the topic, null for a new poll).
    /// </summary>
    public int? PollId { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public PollEditInputModel Input { get; set; }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    public IActionResult OnPostCancel(int? ra = null)
    {
        this.TopicUnapproved = ra == 1;

        return this.ReturnToPage();
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    public IActionResult OnGet(int? pollId = null, int? t = null, int? ra = null)
    {
        this.PollId = null;
        this.Input = new PollEditInputModel();

        // a new topic which should be approved (kept for the save)
        this.TopicUnapproved = ra == 1;

        if (t.HasValue)
        {
            this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

            this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);
        }

        // Check if the user has the page access and variables are correct (before the poll id is kept for the save)
        var denied = this.CheckPollAccess(pollId);

        if (denied is not null)
        {
            return denied;
        }

        // handle poll
        if (pollId.HasValue)
        {
            // edit existing poll
            this.PollId = pollId.Value;

            this.PageBoardContext.PageLinks.AddLink(this.GetText("POLLEDIT", "EDITPOLL"), string.Empty);

            this.PageBoardContext.CurrentForumPage.PageTitle = this.GetText("POLLEDIT", "EDITPOLL");
        }
        else
        {
            this.PageBoardContext.PageLinks.AddLink(this.GetText("POLLEDIT", "CREATEPOLL"), string.Empty);

            this.PageBoardContext.CurrentForumPage.PageTitle = this.GetText("POLLEDIT", "CREATEPOLL");
        }

        List<Choice> choices;

        if (this.PollId.HasValue)
        {
            // we edit existing poll
            var pollAndChoices = this.GetRepository<Poll>().GetPollAndChoices(this.PollId.Value);

            var poll = pollAndChoices.FirstOrDefault()!.Item1;

            if (poll.UserID != this.PageBoardContext.PageUserID && !this.PageBoardContext.IsAdmin
                                                                && !this.PageBoardContext.ForumModeratorAccess)
            {
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            this.Input.IsClosedBoundCheckBox = poll.PollFlags.IsClosedBound;
            this.Input.AllowMultipleChoicesCheckBox = poll.PollFlags.AllowMultipleChoice;
            this.Input.ShowVotersCheckBox = poll.PollFlags.ShowVoters;
            this.Input.Question = poll.Question;
            this.Input.QuestionObjectPath = poll.ObjectPath;

            if (poll.Closes.HasValue)
            {
                var closing = poll.Closes.Value - DateTime.UtcNow;

                this.Input.PollExpire = (closing.TotalDays + 1).ToType<int>().ToString();
            }
            else
            {
                this.Input.PollExpire = string.Empty;
            }

            choices = [.. pollAndChoices.Select(c => c.Item2)];

            var count = this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber - 1 - choices.Count;

            if (count > 0)
            {
                for (var i = 0; i <= count; i++)
                {
                    var choice = new Choice {ID = 0};

                    choices.Add(choice);
                }
            }
        }
        else
        {
            // A new poll is created
            if (!this.CanCreatePoll())
            {
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
            }

            // clear the fields...
            this.Input.PollExpire = string.Empty;
            this.Input.Question = string.Empty;

            choices = [];

            // we add dummy rows to data table to fill in repeater empty fields
            var dummyRowsCount = this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber - 1;
            for (var i = 0; i <= dummyRowsCount; i++)
            {
                var choice = new Choice {ID = 0};

                choices.Add(choice);
            }
        }

        // Bind choices repeater
        this.Input.Choices = choices;

        return this.Page();
    }

    /// <summary>
    /// The save poll_ click.
    /// </summary>
    public IActionResult OnPostSavePoll(int? ra = null)
    {
        this.TopicUnapproved = ra == 1;

        // the edited poll is the poll of the topic (a temp data value was lost when the page was shown
        // again with a warning)
        this.PollId = this.PageBoardContext.PageTopic?.PollID;

        // the same checks as for showing the page, the post can be sent without it
        var denied = this.CheckPollAccess(this.PollId);

        if (denied is not null)
        {
            return denied;
        }

        if (this.Input?.Question is null || this.Input.Question.Trim().Length == 0)
        {
            return this.PageBoardContext.Notify(this.GetText("POLLEDIT", "NEED_QUESTION"), MessageTypes.warning);
        }

        this.Input.Question = HtmlTagHelper.StripHtml(this.Input.Question);

        // the form has as many fields as choices are allowed, more can only be posted directly
        this.Input.Choices = [.. (this.Input.Choices ?? []).Take(this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber)];

        var count = this.Input.Choices.Count(choice => choice.ChoiceName.IsSet());

        if (count < 2)
        {
            return this.PageBoardContext.Notify(this.GetText("POLLEDIT", "NEED_CHOICES"), MessageTypes.warning);
        }

        // the options are only shown with these settings, the values are sent by the client
        // (the images could be loaded from any site)
        var boardSettings = this.PageBoardContext.BoardSettings;

        if (!(this.PageBoardContext.IsAdmin || boardSettings.AllowUsersImagedPoll) || !this.PageBoardContext.ForumPollAccess)
        {
            this.Input.QuestionObjectPath = null;
            this.Input.Choices.ForEach(choice => choice.ObjectPath = null);
        }

        if (!(boardSettings.AllowMultipleChoices || this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess))
        {
            this.Input.AllowMultipleChoicesCheckBox = false;
        }

        if (!(boardSettings.AllowUsersHidePollResults || this.PageBoardContext.IsAdmin || this.PageBoardContext.IsForumModerator))
        {
            this.Input.IsClosedBoundCheckBox = false;
        }

        // Set default value
        if (this.Input.PollExpire.IsNotSet() && this.Input.IsClosedBoundCheckBox)
        {
            this.Input.PollExpire = "1";
        }

        return this.CreateOrUpdatePoll() ? this.ReturnToPage() : this.Page();
    }

    /// <summary>
    /// The save poll.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private bool CreateOrUpdatePoll()
    {
        DateTime? datePollExpire = null;

        // the days are sent as text, not a number or a huge value can not be used (no expiry then)
        if (int.TryParse(this.Input.PollExpire, out var days) && days > 0)
        {
            datePollExpire = DateTime.UtcNow.AddDays(Math.Min(days, 36500));
        }

        // we are just using existing poll
        if (this.PollId.HasValue)
        {
            // only the choices of this poll can be changed or removed
            var pollChoiceIds = this.GetRepository<Choice>().Get(c => c.PollID == this.PollId.Value).Select(c => c.ID)
                .ToHashSet();

            this.GetRepository<Poll>().Update(
                this.PollId.Value,
                this.Input.Question,
                datePollExpire,
                this.Input.IsClosedBoundCheckBox,
                this.Input.AllowMultipleChoicesCheckBox,
                this.Input.ShowVotersCheckBox,
                this.Input.QuestionObjectPath);

            this.Input.Choices.ForEach(
                item =>
                {
                    var choiceId = item.ID;

                    var choiceName = item.ChoiceName;
                    var choiceObjectPath = item.ObjectPath;

                    if (choiceId == 0 && choiceName.IsSet())
                    {
                        // add choice
                        this.GetRepository<Choice>().AddChoice(this.PollId.Value, choiceName, choiceObjectPath);
                    }
                    else if (pollChoiceIds.Contains(choiceId))
                    {
                        if (choiceName.IsSet())
                        {
                            // update choice
                            this.GetRepository<Choice>().UpdateChoice(choiceId, choiceName, choiceObjectPath);
                        }
                        else
                        {
                            // remove choice
                            this.GetRepository<Choice>().DeleteById(choiceId);
                        }
                    }
                });

            return true;
        }

        // Create New Poll
        var newPollId = this.GetRepository<Poll>().Create(
            this.PageBoardContext.PageUserID,
            this.Input.Question,
            datePollExpire,
            this.Input.IsClosedBoundCheckBox,
            this.Input.AllowMultipleChoicesCheckBox,
            this.Input.ShowVotersCheckBox,
            this.Input.QuestionObjectPath);

        this.Input.Choices.ForEach(
            item =>
            {
                var choiceName = item.ChoiceName;
                var choiceObjectPath = item.ObjectPath;

                if (choiceName.IsSet())
                {
                    // add choice
                    this.GetRepository<Choice>().AddChoice(newPollId, choiceName, choiceObjectPath);
                }
            });

        // Attach Poll to topic
        this.GetRepository<Topic>().AttachPoll(this.PageBoardContext.PageTopicID, newPollId);

        return true;
    }

    /// <summary>
    /// The return to page.
    /// </summary>
    private IActionResult ReturnToPage()
    {
        return this.TopicUnapproved
                   ?
                   // Tell user that his message will have to be approved by a moderator
                   this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Moderated)
                   : this.Get<ILinkBuilder>().Redirect(
                       ForumPages.Posts,
                       new {t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName});
    }

    /// <summary>
    /// Checks the access to the poll: poll access in the forum, the poll belongs to the topic and to the
    /// user (or a moderator), a new poll only for the own topic without a poll.
    /// </summary>
    /// <param name="pollId">The poll identifier (null for a new poll).</param>
    /// <returns>Returns the redirect when there is no access, otherwise null</returns>
    private IActionResult CheckPollAccess(int? pollId)
    {
        // polls always belong to a topic (all links contain it), the moderator access is the one of its forum
        var topic = this.PageBoardContext.PageTopic;

        if (this.PageBoardContext.PageForumID == 0 || topic is null || !this.PageBoardContext.ForumPollAccess)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }

        var isModerator = this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess;

        if (pollId.HasValue)
        {
            // the poll of the topic, by its owner or a moderator
            var poll = this.GetRepository<Poll>().GetById(pollId.Value);

            if (poll is null || topic.PollID != poll.ID
                             || poll.UserID != this.PageBoardContext.PageUserID && !isModerator)
            {
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            return null;
        }

        // a new poll only for an own topic (moderators: any topic) without a poll
        if (topic.PollID.HasValue || topic.UserID != this.PageBoardContext.PageUserID && !isModerator)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }

        return null;
    }

    /// <summary>
    /// Checks if a poll can be created.
    /// </summary>
    /// <returns>Returns if a poll can be created</returns>
    private bool CanCreatePoll()
    {
        if (this.PageBoardContext.PageTopic != null)
        {
            return true;
        }

        // admins can add any number of polls
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
        {
            return true;
        }

        if (!this.PageBoardContext.ForumPollAccess)
        {
            return false;
        }

        if (this.PageBoardContext.ForumPollAccess)
        {
            return true;
        }

        return this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber > 0;
    }
 }