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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// PollList Class
    /// </summary>
    public partial class PollList : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The canChange.
        /// </summary>
        private bool canChange;

        /// <summary>
        /// The poll and choices.
        /// </summary>
        private List<Tuple<Poll, Choice>> pollAndChoices;

        /// <summary>
        /// The poll.
        /// </summary>
        private Poll poll;

        /// <summary>
        /// The user poll votes.
        /// </summary>
        private List<PollVote> userPollVotes;

        /// <summary>
        ///   The IsVoteEvent
        /// </summary>
        private bool isVoteEvent;

        /// <summary>
        ///   The _showResults. Used to store data from parent repeater.
        /// </summary>
        private bool showResults;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets EditMessageId.
        /// </summary>
        public int EditMessageId { get; set; }

        /// <summary>
        ///   Gets or sets ForumId
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        ///   Gets or sets Poll Id
        /// </summary>
        public int? PollId { get; set; }

        /// <summary>
        ///   Gets or sets TopicId
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the topic creator.
        /// </summary>
        public int TopicCreator { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a user can create poll.
        /// </summary>
        /// <returns>
        /// The can create poll.
        /// </returns>
        protected bool CanCreatePoll()
        {
            return this.PageContext.BoardSettings.AllowedPollChoiceNumber > 0 && this.HasOwnerExistingGroupAccess() &&
                   this.PollId >= 0;
        }

        /// <summary>
        /// Checks if user can edit a poll
        /// </summary>
        /// <returns>
        /// The can edit poll.
        /// </returns>
        protected bool CanEditPoll()
        {
            // The host admin can forbid a user to change poll after first vote to avoid fakes.
            if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                // Only if show buttons are enabled user can edit poll
                return this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess ||
                       this.PageContext.PageUserID == this.poll.UserID && this.PollHasNoVotes() &&
                       !this.IsPollClosed();
            }

            // we don't call PollHasNoVotes method here
            return this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess ||
                   this.PageContext.PageUserID == this.poll.UserID && !this.IsPollClosed();
        }

        /// <summary>
        /// Checks if a user can delete poll without deleting it from database
        /// </summary>
        /// <returns>
        /// The can remove poll.
        /// </returns>
        protected bool CanRemovePoll()
        {
            return this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess ||
                                        this.PageContext.PageUserID == this.poll.UserID;
        }

        /// <summary>
        /// Days to run.
        /// </summary>
        /// <param name="closesSoon">
        /// The closes Soon.
        /// </param>
        /// <returns>
        /// The days to run.
        /// </returns>
        protected int? DaysToRun(out bool closesSoon)
        {
            closesSoon = false;

            if (!this.poll.Closes.HasValue)
            {
                return null;
            }

            var ts = this.poll.Closes.Value - DateTime.UtcNow;

            if (ts.TotalDays >= 1)
            {
                return ts.TotalDays.ToType<int>();
            }

            if (!(ts.TotalSeconds > 0))
            {
                return 0;
            }

            closesSoon = true;
            return 1;
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        /// <param name="mimeType">The mime type.</param>
        /// <returns>
        /// The get image height.
        /// </returns>
        protected int GetImageHeight([NotNull] object mimeType)
        {
            var attrs = mimeType.ToString().Split('!')[1].Split(';');

            return attrs[1].ToType<int>();
        }

        /// <summary>
        /// Gets the poll is closed.
        /// </summary>
        /// <returns>
        /// The get poll is closed.
        /// </returns>
        protected string GetPollIsClosed()
        {
            var strPollClosed = string.Empty;
            if (this.IsPollClosed())
            {
                strPollClosed = this.GetText("POLL_CLOSED");
            }

            return strPollClosed;
        }

        /// <summary>
        /// Determines whether [is poll closed] [the specified poll id].
        /// </summary>
        /// <returns>
        /// The is poll closed.
        /// </returns>
        protected bool IsPollClosed()
        {
            var dtr = this.DaysToRun(out _);
            return dtr == 0;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.LoadData();
        }

        /// <summary>
        /// The edit click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void EditClick(object sender, EventArgs e)
        {
            if (this.PageContext.ForumVoteAccess)
            {
                var parameters = this.ParamsToSend();

                parameters.Add("p", this.poll.ID);

                this.Get<LinkBuilder>().Redirect(
                    ForumPages.PollEdit,
                    parameters);
            }
            else
            {
                return;
            }

            this.ReturnToPage();
        }

        /// <summary>
        /// The new click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void NewClick(object sender, EventArgs e)
        {
            if (this.PageContext.ForumVoteAccess)
            {
                this.Get<LinkBuilder>().Redirect(ForumPages.PollEdit, this.ParamsToSend());
            }
            else
            {
                return;
            }

            this.ReturnToPage();
        }

        /// <summary>
        /// The remove click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemoveClick(object sender, EventArgs e)
        {
            if (this.PageContext.ForumVoteAccess)
            {
                this.GetRepository<Poll>().Remove(this.poll.ID);
            }
            else
            {
                return;
            }

            this.ReturnToPage();
        }

        /// <summary>
        /// The event initiated in PollChoiceList.
        /// </summary>
        /// <param name="sender">
        /// The object sender.
        /// </param>
        /// <param name="e">
        /// The EventArgs e.
        /// </param>
        protected void VoteBubbleEvent([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.isVoteEvent = true;
            this.LoadData();
        }

        /// <summary>
        /// The bind create new poll row.
        /// </summary>
        private void BindCreateNewPollRow()
        {
            this.CreatePoll.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PollEdit, this.ParamsToSend());
            this.CreatePoll.DataBind();

            this.NewPollRow.Visible = true;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.pollAndChoices = this.GetRepository<Poll>().GetPollAndChoices(this.PollId.Value);

            // Check if Poll Exist
            if (this.pollAndChoices.FirstOrDefault() == null)
            {
                this.Visible = false;

                return;
            }

            this.poll = this.pollAndChoices.FirstOrDefault().Item1;

            // if the page user can change the poll. Only a group owner, forum moderator  or an admin can do it.   );
            this.canChange = this.poll.UserID == this.PageContext.PageUserID || this.PageContext.IsAdmin ||
                              this.PageContext.ForumModeratorAccess;

            if (this.pollAndChoices.Any())
            {
                // Check if the user is already voted in polls in the group
                this.userPollVotes = this.GetRepository<PollVote>().VoteCheck(this.poll.ID, this.PageContext.PageUserID);

                // We don't display poll command row to everyone
                this.PollCommandRow.Visible = this.HasOwnerExistingGroupAccess();

                this.QuestionLabel.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.poll.Question));

                this.TotalVotes.Text = this.pollAndChoices.Sum(x => x.Item2.Votes).ToString();

                this.EditPoll.Visible = this.CanEditPoll();
                this.CreatePoll.Visible = this.CanCreatePoll();

                // Binding question image
                this.BindPollQuestionImage();

                var pollChoiceList = this.PollChoiceList1;

                var isNotVoted = this.userPollVotes.Any();

                pollChoiceList.UserPollVotes = this.userPollVotes;

                // If guest are not allowed to view options we don't render them
                pollChoiceList.Visible = !(!this.HasVoteAccess() && isNotVoted &&
                                           this.PageContext.IsGuest);

                // This is not a guest w/o poll option view permissions, we bind the control.
                if (pollChoiceList.Visible)
                {
                    pollChoiceList.DataSource = this.pollAndChoices;
                }

                if (this.poll.PollFlags.ShowVoters)
                {
                    pollChoiceList.Voters = this.GetRepository<PollVote>().Voters(this.poll.ID);
                }

                pollChoiceList.PollId = this.poll.ID;
                pollChoiceList.Votes = this.pollAndChoices.Sum(x => x.Item2.Votes);

                // returns number of day to run - null if poll has no expiration date
                var daysToRun = this.DaysToRun(out var closesSoon);
                var isPollClosed = this.IsPollClosed();

                var isClosedBound = this.poll.Closes.HasValue && this.poll.PollFlags.IsClosedBound &&
                                    this.poll.Closes.Value < DateTime.UtcNow;

                switch (isClosedBound)
                {
                    case true when isPollClosed:
                    case false:
                        this.showResults = true;
                        break;
                }

                // The poll had an expiration date and expired without voting
                // show results anyway if poll has no expiration date days to run is null
                if (daysToRun == 0)
                {
                    this.showResults = true;
                }

                pollChoiceList.HideResults = !this.showResults;

                // Clear the fields after the child repeater is bound
                this.showResults = false;

                // Add confirmations to delete buttons
                this.RemovePoll.Visible = this.CanRemovePoll();

                // *************************
                // Poll warnings section
                // *************************
                var notificationString = new StringBuilder();

                if (this.PageContext.IsGuest)
                {
                    notificationString.Append(this.GetText("POLLEDIT", "POLLOPTIONSHIDDEN_GUEST"));
                }
                else
                {
                    // Here warning labels are treated
                    if (this.poll.PollFlags.AllowMultipleChoice)
                    {
                        notificationString.Append(this.GetText("POLLEDIT", "POLL_MULTIPLECHOICES_INFO"));
                    }
                }

                if (!isNotVoted && this.PageContext.ForumVoteAccess)
                {
                    notificationString.AppendFormat(" {0}", this.GetText("POLLEDIT", "POLL_VOTED"));
                }

                // Poll has expiration date
                if (daysToRun > 0)
                {
                    notificationString.AppendFormat(
                        " {0}",
                        !closesSoon
                            ? this.GetTextFormatted("POLL_WILLEXPIRE", daysToRun)
                            : this.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS"));

                    if (isClosedBound)
                    {
                        notificationString.AppendFormat(" {0}", this.GetText("POLLEDIT", "POLL_CLOSEDBOUND"));
                    }
                }
                else if (daysToRun == 0)
                {
                    notificationString.Clear();
                    notificationString.Append(this.GetText("POLLEDIT", "POLL_EXPIRED"));

                    this.PollClosed.Text =
                        $"<span class=\"badge bg-danger ms-1\"><i class=\"fa fa-lock fa-fw\"></i>&nbsp;{this.GetText("POLLEDIT", "POLL_CLOSED")}</span>";
                    this.PollClosed.Visible = true;
                }

                pollChoiceList.CanVote = this.HasVoteAccess() && isNotVoted;
                pollChoiceList.DaysToRun = daysToRun;

                // we should double bind if it is not a vote event bubble.
                if (this.isVoteEvent)
                {
                    pollChoiceList.DataBind();
                }

                pollChoiceList.ChoiceVoted += this.VoteBubbleEvent;

                var notification = notificationString.ToString();

                // we don't display warnings row if no info
                if (notification.IsSet())
                {
                    this.PollNotification.Text = notification;
                    this.Alert.Visible = true;
                }

                // we hide new poll row if a poll exist
                this.NewPollRow.Visible = false;
            }

            this.DataBind();
        }

        /// <summary>
        /// Adds custom or standard image to poll question.
        /// </summary>
        private void BindPollQuestionImage()
        {
            // The image is not from theme
            if (this.poll.ObjectPath.IsSet())
            {
                this.QuestionImage.ImageUrl = this.HtmlEncode(this.poll.ObjectPath);

                this.QuestionImage.AlternateText = this.QuestionImage.ToolTip = this.poll.Question;
            }
            else
            {
                this.QuestionImage.Visible = false;
            }
        }

        /// <summary>
        /// Determines whether [has owner existing group access].
        /// </summary>
        /// <returns>
        /// The has owner existing group access.
        /// </returns>
        private bool HasOwnerExistingGroupAccess()
        {
            if (this.PageContext.BoardSettings.AllowedPollChoiceNumber <= 0)
            {
                return false;
            }

            // if topic id > 0 it can be every member
            if (this.TopicId > 0)
            {
                return this.TopicCreator == this.PageContext.PageUserID || this.PageContext.IsAdmin ||
                       this.PageContext.ForumModeratorAccess;
            }

            // in other places only admins and forum moderators can have access
            return this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess;
        }

        /// <summary>
        /// The method to return if a user already voted
        /// </summary>
        /// <returns>
        /// Return True if a user has voted.
        /// </returns>
        private bool HasVoteAccess()
        {
            // rule out users without voting rights
            // If you come here from topics or edit TopicId should be > 0
            if (!this.PageContext.ForumVoteAccess && this.TopicId > 0)
            {
                return false;
            }

            return !this.IsPollClosed();
        }

        /// <summary>
        /// Handles page controls after an event.
        /// </summary>
        private void LoadData()
        {
            // if this is > 0 then we already have a poll and will display all buttons
            if (this.PollId > 0)
            {
                this.BindData();

                if (!this.canChange && this.PageContext.CurrentForumPage.PageType is ForumPages.PostMessage or ForumPages.EditMessage)
                {
                    this.PollListHolder.Visible = false;
                }
            }
            else
            {
                this.PollListHolder.Visible = false;

                this.BindCreateNewPollRow();
            }
        }

        /// <summary>
        /// A method to return parameters string.
        ///   Constructs parameters string to send to other forms.
        /// </summary>
        /// <returns>
        /// The parameters to send.
        /// </returns>
        private Dictionary<string, object> ParamsToSend()
        {
            var parameters = new Dictionary<string, object>();

            if (this.TopicId > 0)
            {
                parameters.Add("t", this.TopicId);
            }

            if (this.EditMessageId > 0)
            {
                parameters.Add("m", this.EditMessageId);
            }

            if (this.ForumId <= 0)
            {
                return parameters;
            }

            parameters.Add("f", this.ForumId);

            return parameters;
        }

        /// <summary>
        /// Checks if a poll has no votes.
        /// </summary>
        /// <returns>
        /// The poll has no votes.
        /// </returns>
        private bool PollHasNoVotes()
        {
            return this.pollAndChoices.All(row => row.Item2.Votes <= 0);
        }

        /// <summary>
        /// Returns user to the original call page.
        /// </summary>
        private void ReturnToPage()
        {
            // We simply return here to the page where the control is put. It can be made other way.
            if (this.PageContext.CurrentForumPage.PageType == ForumPages.Posts)
            {
                if (this.TopicId > 0)
                {
                    this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { t = this.TopicId, name = this.PageContext.PageTopic.TopicName });
                }
            }
            else
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }
        }

        #endregion
    }
}