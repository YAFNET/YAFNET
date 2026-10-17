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

using YAF.Types.Models;

/// <summary>
/// The Poll Edit Page.
/// </summary>
public partial class PollEdit : ForumPage
{
    /// <summary>
    /// The topic unapproved.
    /// </summary>
    private bool topicUnapproved;

    /// <summary>
    /// Initializes a new instance of the <see cref="PollEdit"/> class.
    ///   Initializes a new instance of the ReportPost class.
    /// </summary>
    public PollEdit()
        : base("POLLEDIT", ForumPages.PollEdit)
    {
    }

    /// <summary>
    /// Gets or sets PollID.
    /// </summary>
    protected int? PollId { get; set; }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="eventArgs">
    /// The event args.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs eventArgs)
    {
        this.ReturnToPage();
    }

    /// <summary>
    /// The is input verified.
    /// </summary>
    /// <returns>
    /// Return if input is verified.
    /// </returns>
    protected bool IsInputVerified()
    {
        if (this.Question.Text.Trim().Length == 0)
        {
            this.PageBoardContext.Notify(this.GetText("POLLEDIT", "NEED_QUESTION"), MessageTypes.warning);
            return false;
        }

        this.Question.Text = HtmlTagHelper.StripHtml(this.Question.Text);

        var count =
            (from RepeaterItem ri in this.ChoiceRepeater.Items
             select ri.FindControlAs<TextBox>("PollChoice").Text.Trim()).Count(value => value.IsSet());

        if (count < 2)
        {
            this.PageBoardContext.Notify(this.GetText("POLLEDIT", "NEED_CHOICES"), MessageTypes.warning);
            return false;
        }

        // Set default value
        if (this.PollExpire.Text.IsNotSet() && this.IsClosedBoundCheckBox.Checked)
        {
            this.PollExpire.Text = "1";
        }

        return true;
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.InitializeVariables();

        this.PollObjectRow1.Visible = this.CanUseImages;
    }

    /// <summary>
    /// The save poll_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="eventArgs">
    /// The event args.
    /// </param>
    protected void SavePoll_Click(object sender, EventArgs eventArgs)
    {
        if (!this.PageBoardContext.ForumPollAccess || !this.IsInputVerified())
        {
            return;
        }

        if (this.CreateOrUpdatePoll())
        {
            this.ReturnToPage();
        }
    }

    /// <summary>
    /// Adds page links to the page
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
    }

    /// <summary>
    /// Checks access rights for the page
    /// </summary>
    private void CheckAccess()
    {
        if (this.PageBoardContext.PageForumID > 0 && !this.PageBoardContext.ForumPollAccess)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
        }
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

        // an invalid value (not a number) is no expiry
        if (int.TryParse(this.PollExpire.Text.Trim(), out var expireDays) && expireDays > 0)
        {
            datePollExpire = DateTime.UtcNow.AddDays(Math.Min(expireDays, 3650));
        }

        // the options the settings don't allow for the user are not taken from the form
        var allowImages = this.CanUseImages;
        var questionPath = allowImages ? this.QuestionObjectPath.Text.Trim() : null;
        var isClosedBound = this.CanHideResults && this.IsClosedBoundCheckBox.Checked;
        var allowMultipleChoices = this.CanUseMultipleChoices && this.AllowMultipleChoicesCheckBox.Checked;

        // we are just using existing poll
        if (this.PollId != null)
        {
            this.GetRepository<Poll>().Update(
                this.PollId.Value,
                this.Question.Text,
                datePollExpire,
                isClosedBound,
                allowMultipleChoices,
                this.ShowVotersCheckBox.Checked,
                questionPath);

            // only the choices of this poll (the ids are sent by the client)
            var pollChoices = this.GetRepository<Choice>().Get(c => c.PollID == this.PollId.Value).Select(c => c.ID)
                .ToList();

            this.ChoiceRepeater.Items.Cast<RepeaterItem>().ForEach(
                item =>
                    {
                        var choiceId = item.FindControlAs<HiddenField>("PollChoiceID").Value.ToType<int>();

                        var choiceName = item.FindControlAs<TextBox>("PollChoice").Text.Trim();
                        var choiceObjectPath = allowImages ? item.FindControlAs<TextBox>("ObjectPath").Text.Trim() : null;

                        if (choiceId != 0 && !pollChoices.Contains(choiceId))
                        {
                            return;
                        }

                        if (choiceId == 0 && choiceName.IsSet())
                        {
                            // add choice
                            this.GetRepository<Choice>().AddChoice(this.PollId.Value, choiceName, choiceObjectPath);
                        }
                        else
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
            this.Question.Text,
            datePollExpire,
            isClosedBound,
            allowMultipleChoices,
            this.ShowVotersCheckBox.Checked,
            questionPath);

        this.ChoiceRepeater.Items.Cast<RepeaterItem>().ForEach(
            item =>
                {
                    var choiceName = item.FindControlAs<TextBox>("PollChoice").Text.Trim();
                    var choiceObjectPath = allowImages ? item.FindControlAs<TextBox>("ObjectPath").Text.Trim() : null;

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
    /// Initializes Poll UI
    /// </summary>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    private void InitPollUI(int? pollId)
    {
        this.AllowMultipleChoicesCheckBox.Text = this.GetText("POLL_MULTIPLECHOICES");
        this.ShowVotersCheckBox.Text = this.GetText("POLL_SHOWVOTERS");
        this.IsClosedBoundCheckBox.Text = this.GetText("pollgroup_closedbound");

        List<Choice> choices;

        if (pollId.HasValue)
        {
            // we edit existing poll
            var pollAndChoices = this.GetRepository<Poll>().GetPollAndChoices(this.PollId.Value);

            var poll = pollAndChoices.FirstOrDefault()?.Item1;

            // only the poll of the topic (the moderator access is the access of the forum of the topic)
            if (poll == null || this.PageBoardContext.PageTopic?.PollID != poll.ID
                             || (poll.UserID != this.PageBoardContext.PageUserID || !this.IsOwnGuestTopic())
                             && !this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            this.IsClosedBoundCheckBox.Checked = poll.PollFlags.IsClosedBound;
            this.AllowMultipleChoicesCheckBox.Checked = poll.PollFlags.AllowMultipleChoice;
            this.ShowVotersCheckBox.Checked = poll.PollFlags.ShowVoters;
            this.Question.Text = poll.Question;
            this.QuestionObjectPath.Text = poll.ObjectPath;

            if (poll.Closes.HasValue)
            {
                var closing = poll.Closes.Value - DateTime.UtcNow;

                this.PollExpire.Text = (closing.TotalDays + 1).ToType<int>().ToString();
            }
            else
            {
                this.PollExpire.Text = string.Empty;
            }

            choices = [.. pollAndChoices.Select(c => c.Item2)];

            var count = this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber - 1 - choices.Count;

            if (count > 0)
            {
                for (var i = 0; i <= count; i++)
                {
                    var choice = new Choice { ID = 0 };

                    choices.Add(choice);
                }
            }
        }
        else
        {
            // A new poll is created
            if (!this.CanCreatePoll())
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
            }

            // clear the fields...
            this.PollExpire.Text = string.Empty;
            this.Question.Text = string.Empty;

            choices = [];

            // we add dummy rows to data table to fill in repeater empty fields
            var dummyRowsCount = this.PageBoardContext.BoardSettings.AllowedPollChoiceNumber - 1;
            for (var i = 0; i <= dummyRowsCount; i++)
            {
                var choice = new Choice { ID = 0 };

                choices.Add(choice);
            }
        }

        // Bind choices repeater
        this.ChoiceRepeater.DataSource = choices;
        this.ChoiceRepeater.DataBind();

        // Show controls
        this.SavePoll.Visible = true;
        this.Cancel.Visible = true;
        this.PollRowExpire.Visible = true;
        this.IsClosedBound.Visible = this.CanHideResults;
        this.tr_AllowMultipleChoices.Visible = this.CanUseMultipleChoices;
    }

    /// <summary>
    /// Gets a value indicating whether the user can add images to the poll.
    /// </summary>
    private bool CanUseImages =>
        (this.PageBoardContext.IsAdmin || this.PageBoardContext.BoardSettings.AllowUsersImagedPoll)
        && this.PageBoardContext.ForumPollAccess;

    /// <summary>
    /// Gets a value indicating whether the user can hide the results until the poll is closed.
    /// </summary>
    private bool CanHideResults =>
        this.PageBoardContext.BoardSettings.AllowUsersHidePollResults || this.PageBoardContext.IsAdmin
                                                                      || this.PageBoardContext.ForumModeratorAccess;

    /// <summary>
    /// Gets a value indicating whether the user can allow multiple choices.
    /// </summary>
    private bool CanUseMultipleChoices =>
        this.PageBoardContext.BoardSettings.AllowMultipleChoices || this.PageBoardContext.IsAdmin
                                                                 || this.PageBoardContext.ForumModeratorAccess;

    /// <summary>
    /// Initializes page context query variables.
    /// </summary>
    private void InitializeVariables()
    {
        // we return to a forum (used when a topic should be approved)
        if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
        {
            this.topicUnapproved = true;
        }

        if (this.Get<HttpRequestBase>().QueryString.Exists("t"))
        {
            this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

            this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);
        }

        // Check if the user has the page access and variables are correct.
        this.CheckAccess();

        // handle poll
        if (this.Get<HttpRequestBase>().QueryString.Exists("p"))
        {
            // edit existing poll
            this.PollId =
                this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p"));
            this.InitPollUI(this.PollId);

            this.PageBoardContext.PageLinks.AddLink(this.GetText("POLLEDIT", "EDITPOLL"), string.Empty);

            this.Header.LocalizedTag = "EDITPOLL";
        }
        else
        {
            // new poll
            this.InitPollUI(null);

            this.PageBoardContext.PageLinks.AddLink(this.GetText("POLLEDIT", "CREATEPOLL"), string.Empty);

            this.Header.LocalizedTag = "CREATEPOLL";
        }
    }

    /// <summary>
    /// The return to page.
    /// </summary>
    private void ReturnToPage()
    {
        if (this.topicUnapproved)
        {
            // Tell user that his message will have to be approved by a moderator
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Moderated);
        }

        this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { t = this.PageBoardContext.PageTopic.ID, name = this.PageBoardContext.PageTopic.TopicName });
    }

    /// <summary>
    /// Checks if a user can create poll.
    /// </summary>
    /// <returns>
    /// The can create poll.
    /// </returns>
    private bool CanCreatePoll()
    {
        // the poll is added to the topic: a topic without a poll, of the user (or a moderator of the forum)
        var topic = this.PageBoardContext.PageTopic;

        if (topic == null || topic.PollID.HasValue || !this.PageBoardContext.ForumPollAccess)
        {
            return false;
        }

        return topic.UserID == this.PageBoardContext.PageUserID && this.IsOwnGuestTopic()
               || this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess;
    }

    /// <summary>
    /// Guests are all the same user: a guest only changes the topics started from the own address (like the buttons
    /// of the posts), every guest could edit or add the polls of all guest topics.
    /// </summary>
    /// <returns>Returns if the topic is not of a guest, or of this guest.</returns>
    private bool IsOwnGuestTopic()
    {
        if (!this.PageBoardContext.IsGuest)
        {
            return true;
        }

        var topicId = this.PageBoardContext.PageTopicID;

        var firstMessage = this.GetRepository<Message>().Get(m => m.TopicID == topicId)
            .OrderBy(m => m.Posted).ThenBy(m => m.ID).FirstOrDefault();

        return firstMessage != null && firstMessage.IP == this.Get<HttpRequestBase>().GetUserRealIPAddress();
    }
}