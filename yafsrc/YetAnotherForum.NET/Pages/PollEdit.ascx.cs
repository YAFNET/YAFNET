/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Utils.Helpers.ImageUtils;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Poll Edit Page.
    /// </summary>
    public partial class PollEdit : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   Table with choices
        /// </summary>
        private Topic topicInfo;

        /// <summary>
        ///   Table with choices
        /// </summary>
        private DataTable choices;

        /// <summary>
        /// The date poll expire.
        /// </summary>
        private DateTime? datePollExpire;

        /// <summary>
        /// The days poll expire.
        /// </summary>
        private int daysPollExpire;

        /// <summary>
        /// The topic unapproved.
        /// </summary>
        private bool topicUnapproved;

        /// <summary>
        /// The forum id.
        /// </summary>
        private int forumId;

        /// <summary>
        /// The edit message id.
        /// </summary>
        private int? editMessageId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PollEdit"/> class. 
        ///   Initializes a new instance of the ReportPost class.
        /// </summary>
        public PollEdit()
            : base("POLLEDIT")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets PollID.
        /// </summary>
        protected int? PollId { get; set; }

        #endregion

        #region Methods

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
                BoardContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "NEED_QUESTION"), MessageTypes.warning);
                return false;
            }

            this.Question.Text = HtmlHelper.StripHtml(this.Question.Text);

            var count =
                (from RepeaterItem ri in this.ChoiceRepeater.Items
                    select ri.FindControlAs<TextBox>("PollChoice").Text.Trim()).Count(value => value.IsSet());

            if (count < 2)
            {
                BoardContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "NEED_CHOICES"), MessageTypes.warning);
                return false;
            }

            if (!int.TryParse(this.PollExpire.Text.Trim(), out _) && this.PollExpire.Text.Trim().IsSet())
            {
                BoardContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "EXPIRE_BAD"), MessageTypes.warning);
                return false;
            }

            // Set default value
            if (this.PollExpire.Text.Trim().IsNotSet() && this.IsClosedBoundCheckBox.Checked)
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

            this.PollObjectRow1.Visible =
                (this.PageContext.IsAdmin || this.Get<BoardSettings>().AllowUsersImagedPoll) &&
                this.PageContext.ForumPollAccess;

            if (int.TryParse(this.PollExpire.Text.Trim(), out this.daysPollExpire))
            {
                this.datePollExpire = DateTime.UtcNow.AddDays(this.daysPollExpire);
            }
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
            if (!this.PageContext.ForumPollAccess || !this.IsInputVerified())
            {
                return;
            }

            if (this.GetPollID() == true)
            {
                this.ReturnToPage();
            }
        }

        /// <summary>
        /// Adds page links to the page
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            if (this.forumId > 0)
            {
                this.PageLinks.AddForum(this.forumId);
            }

            if (this.topicInfo != null)
            {
                this.PageLinks.AddTopic(this.topicInfo.TopicName, this.topicInfo.ID);
            }

            this.PageLinks.AddLink(this.GetText("POLLEDIT", "EDITPOLL"), string.Empty);
        }

        /// <summary>
        /// Checks access rights for the page
        /// </summary>
        private void CheckAccess()
        {
            if (this.forumId > 0 && !this.PageContext.ForumPollAccess)
            {
                BuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
            }

            // invalid category
            var categoryVars = this.topicInfo != null || this.editMessageId > 0 || this.forumId > 0;

            // invalid board vars
            var boardVars = this.topicInfo != null || this.editMessageId > 0 || this.forumId > 0;
            if (!categoryVars || !boardVars)
            {
                BuildLink.RedirectInfoPage(InfoMessage.Invalid);
            }
        }

        /// <summary>
        /// The get poll id.
        /// </summary>
        /// <returns>
        /// Returns the Poll Id
        /// </returns>
        private bool? GetPollID()
        {
            if (int.TryParse(this.PollExpire.Text.Trim(), out this.daysPollExpire))
            {
                this.datePollExpire = DateTime.UtcNow.AddDays(this.daysPollExpire);
            }

            // we are just using existing poll
            if (this.PollId != null)
            {
                var questionPath = this.QuestionObjectPath.Text.Trim();
                var questionMime = string.Empty;

                if (questionPath.IsSet())
                {
                    questionMime = ImageHelper.GetImageParameters(new Uri(questionPath), out var length);
                    if (questionMime.IsNotSet())
                    {
                        BoardContext.Current.AddLoadMessage(
                            this.GetTextFormatted("POLLIMAGE_INVALID", questionPath),
                            MessageTypes.warning);
                        return false;
                    }

                    if (length > this.Get<BoardSettings>().PollImageMaxFileSize * 1024)
                    {
                        BoardContext.Current.AddLoadMessage(
                            this.GetTextFormatted(
                                "POLLIMAGE_TOOBIG",
                                length / 1024,
                                this.Get<BoardSettings>().PollImageMaxFileSize,
                                questionPath),
                            MessageTypes.warning);
                        return false;
                    }
                }

                this.GetRepository<Poll>().Update(
                    this.PollId,
                    this.Question.Text,
                    this.datePollExpire,
                    this.IsBoundCheckBox.Checked,
                    this.IsClosedBoundCheckBox.Checked,
                    this.AllowMultipleChoicesCheckBox.Checked,
                    this.ShowVotersCheckBox.Checked,
                    this.AllowSkipVoteCheckBox.Checked,
                    questionPath,
                    questionMime);

                foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
                {
                    var choice = ri.FindControlAs<TextBox>("PollChoice").Text.Trim();
                    var child = ri.FindControlAs<HiddenField>("PollChoiceID").Value;

                    var choiceObjectPath = ri.FindControlAs<TextBox>("ObjectPath").Text.Trim();

                    var choiceImageMime = string.Empty;

                    // update choice
                    if (choiceObjectPath.IsSet())
                    {
                        choiceImageMime = ImageHelper.GetImageParameters(new Uri(choiceObjectPath), out var length);
                        if (choiceImageMime.IsNotSet())
                        {
                            BoardContext.Current.AddLoadMessage(
                                this.GetTextFormatted("POLLIMAGE_INVALID", choiceObjectPath.Trim()),
                                MessageTypes.warning);
                            return false;
                        }

                        if (length > this.Get<BoardSettings>().PollImageMaxFileSize * 1024)
                        {
                            BoardContext.Current.AddLoadMessage(
                                this.GetTextFormatted(
                                    "POLLIMAGE_TOOBIG",
                                    length / 1024,
                                    this.Get<BoardSettings>().PollImageMaxFileSize,
                                    choiceObjectPath),
                                MessageTypes.warning);
                            return false;
                        }
                    }

                    if (child.IsNotSet() && choice.IsSet())
                    {
                        // add choice
                        this.GetRepository<Choice>().AddChoice(
                            this.PollId.Value,
                            choice,
                            choiceObjectPath,
                            choiceImageMime);
                    }
                    else if (child.IsSet() && choice.IsSet())
                    {
                        this.GetRepository<Choice>().UpdateChoice(
                            child.ToType<int>(),
                            choice,
                            choiceObjectPath,
                            choiceImageMime);
                    }
                    else if (child.IsSet() && choice.IsNotSet())
                    {
                        // remove choice
                        this.GetRepository<Choice>().DeleteById(child.ToType<int>());
                    }
                }

                return true;
            }
            else
            {
                var questionPath = this.QuestionObjectPath.Text.Trim();
                var questionMime = string.Empty;

                if (questionPath.IsSet())
                {
                    questionMime = ImageHelper.GetImageParameters(new Uri(questionPath), out var length);
                    if (questionMime.IsNotSet())
                    {
                        BoardContext.Current.AddLoadMessage(
                            this.GetTextFormatted("POLLIMAGE_INVALID", this.QuestionObjectPath.Text.Trim()),
                            MessageTypes.warning);
                        return false;
                    }

                    if (length > this.Get<BoardSettings>().PollImageMaxFileSize * 1024)
                    {
                        BoardContext.Current.AddLoadMessage(
                            this.GetTextFormatted(
                                "POLLIMAGE_TOOBIG",
                                length / 1024,
                                this.Get<BoardSettings>().PollImageMaxFileSize,
                                questionPath),
                            MessageTypes.warning);
                    }
                }

                var pollSaveList = new List<PollSaveList>();

                var rawChoices = new string[3, this.ChoiceRepeater.Items.Count];
                var j = 0;

                foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
                {
                    var choiceObjectPath = ri.FindControlAs<TextBox>("ObjectPath").Text.Trim();

                    var choiceObjectMime = string.Empty;

                    if (choiceObjectPath.IsSet())
                    {
                        choiceObjectMime = ImageHelper.GetImageParameters(new Uri(choiceObjectPath), out var length);
                        if (choiceObjectMime.IsNotSet())
                        {
                            BoardContext.Current.AddLoadMessage(
                                this.GetTextFormatted("POLLIMAGE_INVALID", choiceObjectPath.Trim()),
                                MessageTypes.warning);
                            return false;
                        }

                        if (length > this.Get<BoardSettings>().PollImageMaxFileSize * 1024)
                        {
                            BoardContext.Current.AddLoadMessage(
                                this.GetTextFormatted(
                                    "POLLIMAGE_TOOBIG",
                                    length / 1024,
                                    this.Get<BoardSettings>().PollImageMaxFileSize,
                                    choiceObjectPath),
                                MessageTypes.warning);
                            return false;
                        }
                    }

                    rawChoices[0, j] = HtmlHelper.StripHtml(ri.FindControlAs<TextBox>("PollChoice").Text.Trim());
                    rawChoices[1, j] = choiceObjectPath;
                    rawChoices[2, j] = choiceObjectMime;
                    j++;
                }

                var realTopic = this.topicInfo;

                if (this.topicInfo == null)
                {
                    realTopic = null;
                }

                if (this.datePollExpire == null && this.PollExpire.Text.Trim().IsSet())
                {
                    this.datePollExpire = DateTime.UtcNow.AddDays(this.PollExpire.Text.Trim().ToType<int>());
                }

                pollSaveList.Add(
                    new PollSaveList(
                        this.Question.Text,
                        rawChoices,
                        this.datePollExpire,
                        this.PageContext.PageUserID,
                        realTopic.ID,
                        this.forumId,
                        this.PageContext.PageBoardID,
                        questionPath,
                        questionMime,
                        this.IsBoundCheckBox.Checked,
                        this.IsClosedBoundCheckBox.Checked,
                        this.AllowMultipleChoicesCheckBox.Checked,
                        this.ShowVotersCheckBox.Checked,
                        this.AllowSkipVoteCheckBox.Checked));
                this.GetRepository<Poll>().Save(pollSaveList);
                return true;
            }
        }

        /// <summary>
        /// Initializes Poll UI
        /// </summary>
        /// <param name="pollID">
        /// The poll ID.
        /// </param>
        private void InitPollUI(int? pollID)
        {
            // we should get the schema anyway
            this.choices = this.GetRepository<Poll>().StatsAsDataTable(pollID);
            this.choices.Columns.Add("ChoiceOrderID", typeof(int));

            // First existing values always 1!
            var existingRowsCount = 1;
            var allExistingRowsCount = this.choices.Rows.Count;

            this.AllowMultipleChoicesCheckBox.Text = this.GetText("POLL_MULTIPLECHOICES");
            this.AllowSkipVoteCheckBox.Text = this.GetText("POLL_MULTIPLECHOICES");
            this.ShowVotersCheckBox.Text = this.GetText("POLL_SHOWVOTERS");
            this.IsBoundCheckBox.Text = this.GetText("POLLGROUP_BOUNDWARN");
            this.IsClosedBoundCheckBox.Text = this.GetText("pollgroup_closedbound");

            // we edit existing poll 
            if (this.choices.HasRows())
            {
                if (this.choices.Rows[0]["UserID"].ToType<int>() != this.PageContext.PageUserID &&
                    !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                {
                    BuildLink.RedirectInfoPage(InfoMessage.Invalid);
                }

                this.IsBoundCheckBox.Checked = this.choices.Rows[0]["IsBound"].ToType<bool>();
                this.IsClosedBoundCheckBox.Checked = this.choices.Rows[0]["IsClosedBound"].ToType<bool>();
                this.AllowMultipleChoicesCheckBox.Checked = this.choices.Rows[0]["AllowMultipleChoices"].ToType<bool>();
                this.AllowSkipVoteCheckBox.Checked = this.choices.Rows[0]["AllowSkipVote"].ToType<bool>();
                this.ShowVotersCheckBox.Checked = this.choices.Rows[0]["ShowVoters"].ToType<bool>();
                this.Question.Text = this.choices.Rows[0]["Question"].ToString();
                this.QuestionObjectPath.Text = this.choices.Rows[0]["QuestionObjectPath"].ToString();

                if (this.choices.Rows[0]["Closes"] != DBNull.Value)
                {
                    var closing = (DateTime)this.choices.Rows[0]["Closes"] - DateTime.UtcNow;

                    this.PollExpire.Text = (closing.TotalDays + 1).ToType<int>().ToString();
                }
                else
                {
                    this.PollExpire.Text = null;
                }

                this.choices.Rows.Cast<DataRow>().ForEach(
                    row =>
                    {
                        row["ChoiceOrderID"] = existingRowsCount;

                        existingRowsCount++;
                    });
            }
            else
            {
                // A new topic is created
                // below check currently if works for topics only, but will do as some things are not enabled 
                if (!this.CanCreatePoll())
                {
                    BuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                }

                // Get isBound value using page variables. They are initialized here.
                var pgidt = 0;

                // If a topic poll is edited or new topic created
                if (this.topicInfo.ID > 0 && this.topicInfo != null)
                {
                    // topic id should not be null here 
                    if (this.topicInfo.PollID != null)
                    {
                        pgidt = this.topicInfo.PollID.Value;

                        var pollGroupData = this.GetRepository<Poll>().PollGroupStatsAsDataTable(pgidt);

                        this.IsBoundCheckBox.Checked = pollGroupData.Rows[0]["IsBound"].ToType<bool>();
                    }
                }

                if (pgidt > 0)
                {
                    if (this.GetRepository<Poll>().PollGroupStatsAsDataTable(pgidt).Rows[0]["IsBound"].ToType<int>() ==
                        2)
                    {
                        this.IsBoundCheckBox.Checked = true;
                    }

                    if (this.GetRepository<Poll>().PollGroupStatsAsDataTable(pgidt).Rows[0]["IsClosedBound"]
                        .ToType<int>() == 4)
                    {
                        this.IsClosedBoundCheckBox.Checked = true;
                    }
                }

                // clear the fields...
                this.PollExpire.Text = string.Empty;
                this.Question.Text = string.Empty;
            }

            // we add dummy rows to data table to fill in repeater empty fields   
            var dummyRowsCount = this.Get<BoardSettings>().AllowedPollChoiceNumber - allExistingRowsCount - 1;
            for (var i = 0; i <= dummyRowsCount; i++)
            {
                var drow = this.choices.NewRow();
                drow["ChoiceOrderID"] = existingRowsCount + i;
                this.choices.Rows.Add(drow);
            }

            // Bind choices repeater
            this.ChoiceRepeater.DataSource = this.choices;
            this.ChoiceRepeater.DataBind();
            this.ChoiceRepeater.Visible = true;

            // Show controls
            this.SavePoll.Visible = true;
            this.Cancel.Visible = true;
            this.PollRow1.Visible = true;
            this.PollRowExpire.Visible = true;
            this.IsClosedBound.Visible = this.IsBound.Visible =
                this.Get<BoardSettings>().AllowUsersHidePollResults || this.PageContext.IsAdmin ||
                this.PageContext.IsForumModerator;
            this.tr_AllowMultipleChoices.Visible = this.Get<BoardSettings>().AllowMultipleChoices ||
                                                   this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess;
            this.tr_ShowVoters.Visible = true;
            this.tr_AllowSkipVote.Visible = false;
        }

        /// <summary>
        /// Initializes page context query variables.
        /// </summary>
        private void InitializeVariables()
        {
            // we return to a specific place, general token 
            if (this.Get<HttpRequestBase>().QueryString.Exists("ra"))
            {
                this.topicUnapproved = true;
            }

            // we return to a forum (used when a topic should be approved)
            if (this.Get<HttpRequestBase>().QueryString.Exists("f"))
            {
                this.forumId =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("f"));
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("t"))
            {
                var topicId =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("t"));
                this.topicInfo = this.GetRepository<Topic>().GetById(topicId);
            }

            if (this.Get<HttpRequestBase>().QueryString.Exists("m"))
            {
                this.editMessageId =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));
            }

            // Check if the user has the page access and variables are correct. 
            this.CheckAccess();

            // handle poll
            if (this.Get<HttpRequestBase>().QueryString.Exists("p"))
            {
                // edit existing poll
                this.PollId =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p"));
                this.InitPollUI(this.PollId);
            }
            else
            {
                // new poll
                this.PollRow1.Visible = true;
                this.InitPollUI(null);
            }
        }

        /// <summary>
        /// The params to send.
        /// </summary>
        /// <param name="retliterals">
        /// The retliterals.
        /// </param>
        /// <param name="retvalue">
        /// The retvalue.
        /// </param>
        private void ParamsToSend(out string retliterals, out int? retvalue)
        {
            if (this.editMessageId > 0)
            {
                retliterals = "em";
                retvalue = this.editMessageId;
            }
            else if (this.topicInfo != null)
            {
                retliterals = "t";
                retvalue = this.topicInfo.ID;
            }
            else if (this.forumId > 0)
            {
                retliterals = "f";
                retvalue = this.forumId;
            }
            else
            {
                retliterals = string.Empty;
                retvalue = 0;
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
                BuildLink.Redirect(ForumPages.Info, "i=1");
            }

            // BuildLink.Redirect(ForumPages.Posts, "m={0}#{0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));      
            this.ParamsToSend(out var retliterals, out var retvalue);

            switch (retliterals)
            {
                case "t":
                    BuildLink.Redirect(ForumPages.Posts, "t={0}&name={1}", retvalue, this.topicInfo.TopicName);
                    break;

                case "em":

                    BuildLink.Redirect(ForumPages.PostMessage, "m={0}", retvalue);
                    break;

                case "f":

                    BuildLink.Redirect(ForumPages.Topics, "f={0}", retvalue);
                    break;
                default:
                    BuildLink.RedirectInfoPage(InfoMessage.Invalid);
                    break;
            }
        }

        /// <summary>
        /// Checks if a user can create poll.
        /// </summary>
        /// <returns>
        /// The can create poll.
        /// </returns>
        private bool CanCreatePoll()
        {
            if (this.topicInfo != null)
            {
                return true;
            }

            // admins can add any number of polls
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                return true;
            }

            int? pollGroupId = null;
            if (!this.topicInfo.PollID.HasValue)
            {
                pollGroupId = this.topicInfo.PollID.Value;
            }

            if (!this.PageContext.ForumPollAccess)
            {
                return false;
            }

            if (pollGroupId == null && this.Get<BoardSettings>().AllowedPollNumber > 0 &&
                this.PageContext.ForumPollAccess)
            {
                return true;
            }

            // TODO: repeating code
            // Remove repeating PollID values   
            var hashtable = new Hashtable();
            var duplicateList = new ArrayList();
            var pollGroup = this.GetRepository<Poll>().PollGroupStatsAsDataTable(pollGroupId);

            pollGroup.Rows.Cast<DataRow>().ForEach(
                row =>
                {
                    if (hashtable.Contains(row["PollID"]))
                    {
                        duplicateList.Add(row);
                    }
                    else
                    {
                        hashtable.Add(row["PollID"], string.Empty);
                    }
                });

            duplicateList.Cast<DataRow>().ForEach(row => pollGroup.Rows.Remove(row));

            pollGroup.AcceptChanges();

            // frequently used
            var pollNumber = pollGroup.Rows.Count;

            return pollNumber < this.Get<BoardSettings>().AllowedPollNumber &&
                   this.Get<BoardSettings>().AllowedPollChoiceNumber > 0;
        }

        #endregion
    }
}