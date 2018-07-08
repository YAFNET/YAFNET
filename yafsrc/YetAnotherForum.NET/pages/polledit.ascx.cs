/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Poll Edit Page.
    /// </summary>
    public partial class polledit : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   Table with choices
        /// </summary>
        private Topic topicInfo;

        /// <summary>
        /// The board id.
        /// </summary>
        private int? _boardId;

        /// <summary>
        /// The category id.
        /// </summary>
        private int? _categoryId;

        /// <summary>
        ///   Table with choices
        /// </summary>
        private DataTable _choices;

        /// <summary>
        /// The date poll expire.
        /// </summary>
        private DateTime? _datePollExpire;

        /// <summary>
        /// The days poll expire.
        /// </summary>
        private int _daysPollExpire;

        /// <summary>
        /// The edit board id.
        /// </summary>
        private int? _editBoardId;

        /// <summary>
        /// The edit category id.
        /// </summary>
        private int? _editCategoryId;

        /// <summary>
        /// The edit forum id.
        /// </summary>
        private int? editForumId;

        /// <summary>
        /// The edit topic id.
        /// </summary>
        private int? _editTopicId;

        /// <summary>
        /// The edit message id.
        /// </summary>
        private int? _editMessageId;

        /// <summary>
        /// The forum id.
        /// </summary>
        private int? _forumId;

        /// <summary>
        /// The return forum.
        /// </summary>
        private int? _returnForum;

        /// <summary>
        /// The topic id.
        /// </summary>
        private int? _topicId;

        /// <summary>
        /// The topic unapproved.
        /// </summary>
        private bool _topicUnapproved;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="polledit"/> class. 
        ///   Initializes a new instance of the ReportPost class.
        /// </summary>
        public polledit()
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
            if (this.PollGroupListDropDown.SelectedIndex.ToType<int>() > 0)
            {
                return true;
            }

            if (this.Question.Text.Trim().Length == 0)
            {
                YafContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "NEED_QUESTION"));
                return false;
            }

            this.Question.Text = HtmlHelper.StripHtml(this.Question.Text);

            var notNullcount =
                (from RepeaterItem ri in this.ChoiceRepeater.Items
                 select ((TextBox)ri.FindControl("PollChoice")).Text.Trim()).Count(
                     value => !string.IsNullOrEmpty(value));

            if (notNullcount < 2)
            {
                YafContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "NEED_CHOICES"));
                return false;
            }

            int dateVerified;
            
            if (!int.TryParse(this.PollExpire.Text.Trim(), out dateVerified) && this.PollExpire.Text.Trim().IsSet())
            {
                YafContext.Current.AddLoadMessage(this.GetText("POLLEDIT", "EXPIRE_BAD"));
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
            this.PollExpire.Attributes.Add("style", "width:50px");

            this.InitializeVariables();

            this.PollObjectRow1.Visible = (this.PageContext.IsAdmin || this.Get<YafBoardSettings>().AllowUsersImagedPoll) &&
                                          this.PageContext.ForumPollAccess;

            if (int.TryParse(this.PollExpire.Text.Trim(), out this._daysPollExpire))
            {
                this._datePollExpire = DateTime.UtcNow.AddDays(this._daysPollExpire);
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.AddPageLinks();

            // Admin can attach an existing group if it's a new poll - this.pollID <= 0
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
            {
                return;
            }

            var pollGroup =
                LegacyDb.PollGroupList(this.PageContext.PageUserID, null, this.PageContext.PageBoardID).Distinct(
                    new AreEqualFunc<TypedPollGroup>((id1, id2) => id1.PollGroupID == id2.PollGroupID)).ToList();

            pollGroup.Insert(0, new TypedPollGroup(string.Empty, -1));

            this.PollGroupListDropDown.Items.AddRange(
                pollGroup.Select(x => new ListItem(x.Question, x.PollGroupID.ToString())).ToArray());

            this.PollGroupListDropDown.DataBind();
            this.PollGroupList.Visible = true;
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
        private void AddPageLinks()
        {
            this.PageLinks.AddRoot();

            if (this._categoryId > 0)
            {
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", this._categoryId));
            }

            if (this._returnForum > 0)
            {
                this.PageLinks.AddLink(
                    LegacyDb.forum_list(this.PageContext.PageBoardID, this._returnForum).Rows[0]["Name"].ToString(),
                    YafBuildLink.GetLink(ForumPages.topics, "f={0}", this._returnForum));
            }

            if (this._forumId > 0)
            {
                this.PageLinks.AddLink(
                    LegacyDb.forum_list(this.PageContext.PageBoardID, this._returnForum).Rows[0]["Name"].ToString(),
                    YafBuildLink.GetLink(ForumPages.topics, "f={0}", this._forumId));
            }

            if (this._topicId > 0)
            {
                this.PageLinks.AddLink(
                    this.topicInfo.TopicName, YafBuildLink.GetLink(ForumPages.posts, "t={0}", this._topicId));
            }

            if (this._editMessageId > 0)
            {
                this.PageLinks.AddLink(
                    this.topicInfo.TopicName,
                    YafBuildLink.GetLink(ForumPages.postmessage, "m={0}", this._editMessageId));
            }

            this.PageLinks.AddLink(this.GetText("POLLEDIT", "EDITPOLL"), string.Empty);
        }

        /// <summary>
        /// Checks access rights for the page
        /// </summary>
        private void CheckAccess()
        {
            if (this._boardId > 0 || this._categoryId > 0)
            {
                // invalid category
                var categoryVars = this._categoryId > 0 &&
                                    (this._topicId > 0 || this._editTopicId > 0 || this._editMessageId > 0 ||
                                     this.editForumId > 0 || this._editBoardId > 0 || this._forumId > 0 ||
                                     this._boardId > 0);

                // invalid board vars
                var boardVars = this._boardId > 0 &&
                                 (this._topicId > 0 || this._editTopicId > 0 || this._editMessageId > 0 ||
                                  this.editForumId > 0 || this._editBoardId > 0 || this._forumId > 0 ||
                                  this._categoryId > 0);
                if (!categoryVars || (!boardVars))
                {
                    YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
                }
            }
            else if (this._forumId > 0 && (!this.PageContext.ForumPollAccess))
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
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
            if (int.TryParse(this.PollExpire.Text.Trim(), out this._daysPollExpire))
            {
                this._datePollExpire = DateTime.UtcNow.AddDays(this._daysPollExpire);
            }

            // we are just using existing poll
            if (this.PollId != null)
            {
                var questionPath = this.QuestionObjectPath.Text.Trim();
                var questionMime = string.Empty;

                if (questionPath.IsSet())
                {
                    long length;
                    questionMime = ImageHelper.GetImageParameters(new Uri(questionPath), out length);
                    if (questionMime.IsNotSet())
                    {
                        YafContext.Current.AddLoadMessage(this.GetTextFormatted("POLLIMAGE_INVALID", questionPath));
                        return false;
                    }

                    if (length > this.Get<YafBoardSettings>().PollImageMaxFileSize * 1024)
                    {
                        YafContext.Current.AddLoadMessage(
                            this.GetTextFormatted(
                                "POLLIMAGE_TOOBIG",
                                length / 1024,
                                this.Get<YafBoardSettings>().PollImageMaxFileSize,
                                questionPath));
                        return false;
                    }
                }

                LegacyDb.poll_update(
                    this.PollId,
                    this.Question.Text,
                    this._datePollExpire,
                    this.IsBoundCheckBox.Checked,
                    this.IsClosedBoundCheckBox.Checked,
                    this.AllowMultipleChoicesCheckBox.Checked,
                    this.ShowVotersCheckBox.Checked,
                    this.AllowSkipVoteCheckBox.Checked,
                    questionPath,
                    questionMime);

                foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
                {
                    var choice = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
                    var chid = ((HiddenField)ri.FindControl("PollChoiceID")).Value;

                    var choiceObjectPath = ((TextBox)ri.FindControl("ObjectPath")).Text.Trim();

                    var choiceImageMime = string.Empty;

                    // update choice
                    if (choiceObjectPath.IsSet())
                    {
                        long length;
                        choiceImageMime = ImageHelper.GetImageParameters(new Uri(choiceObjectPath), out length);
                        if (choiceImageMime.IsNotSet())
                        {
                            YafContext.Current.AddLoadMessage(
                                this.GetTextFormatted("POLLIMAGE_INVALID", choiceObjectPath.Trim()));
                            return false;
                        }

                        if (length > this.Get<YafBoardSettings>().PollImageMaxFileSize * 1024)
                        {
                            YafContext.Current.AddLoadMessage(
                                this.GetTextFormatted(
                                    "POLLIMAGE_TOOBIG",
                                    length / 1024,
                                    this.Get<YafBoardSettings>().PollImageMaxFileSize,
                                    choiceObjectPath));
                            return false;
                        }
                    }

                    if (string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
                    {
                        // add choice
                        LegacyDb.choice_add(this.PollId, choice, choiceObjectPath, choiceImageMime);
                    }
                    else if (!string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
                    {
                        LegacyDb.choice_update(chid, choice, choiceObjectPath, choiceImageMime);
                    }
                    else if (!string.IsNullOrEmpty(chid) && string.IsNullOrEmpty(choice))
                    {
                        // remove choice
                        this.GetRepository<Choice>().DeleteById(chid.ToType<int>());
                    }
                }

                return true;
            }
            else
            {
                // User wishes to create a poll  
                // The value was selected, we attach an existing poll
                if (this.PollGroupListDropDown.SelectedIndex.ToType<int>() > 0)
                {
                    var result = LegacyDb.pollgroup_attach(
                        this.PollGroupListDropDown.SelectedValue.ToType<int>(), 
                        this._topicId, 
                        this._forumId, 
                        this._categoryId, 
                        this._boardId);

                    if (result == 1)
                    {
                        this.PageContext.AddLoadMessage(this.GetText("POLLEDIT", "POLLGROUP_ATTACHED"));
                    }

                    return true;
                }

                var questionPath = this.QuestionObjectPath.Text.Trim();
                var questionMime = string.Empty;

                if (questionPath.IsSet())
                {
                    long length;
                    questionMime = ImageHelper.GetImageParameters(new Uri(questionPath), out length);
                    if (questionMime.IsNotSet())
                    {
                        YafContext.Current.AddLoadMessage(
                            this.GetTextFormatted("POLLIMAGE_INVALID", this.QuestionObjectPath.Text.Trim()));
                        return false;
                    }

                    if (length > this.Get<YafBoardSettings>().PollImageMaxFileSize * 1024)
                    {
                        YafContext.Current.AddLoadMessage(
                            this.GetTextFormatted(
                                "POLLIMAGE_TOOBIG", 
                                length / 1024, 
                                this.Get<YafBoardSettings>().PollImageMaxFileSize, 
                                questionPath));
                    }
                }

                var pollSaveList = new List<PollSaveList>();

                var rawChoices = new string[3, this.ChoiceRepeater.Items.Count];
                var j = 0;
                
                foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
                {
                    var choiceObjectPath = ((TextBox)ri.FindControl("ObjectPath")).Text.Trim();

                    var choiceObjectMime = string.Empty;

                    if (choiceObjectPath.IsSet())
                    {
                        long length;
                        choiceObjectMime = ImageHelper.GetImageParameters(new Uri(choiceObjectPath), out length);
                        if (choiceObjectMime.IsNotSet())
                        {
                            YafContext.Current.AddLoadMessage(
                                this.GetTextFormatted("POLLIMAGE_INVALID", choiceObjectPath.Trim()));
                            return false;
                        }

                        if (length > this.Get<YafBoardSettings>().PollImageMaxFileSize * 1024)
                        {
                            YafContext.Current.AddLoadMessage(
                                this.GetTextFormatted(
                                    "POLLIMAGE_TOOBIG", 
                                    length / 1024, 
                                    this.Get<YafBoardSettings>().PollImageMaxFileSize, 
                                    choiceObjectPath));
                            return false;
                        }
                    }

                    rawChoices[0, j] = HtmlHelper.StripHtml(((TextBox)ri.FindControl("PollChoice")).Text.Trim());
                    rawChoices[1, j] = choiceObjectPath;
                    rawChoices[2, j] = choiceObjectMime;
                    j++;
                }

                var realTopic = this._topicId;

                if (this._topicId == null)
                {
                    realTopic = this._editTopicId;
                }

                if (this._datePollExpire == null && this.PollExpire.Text.Trim().IsSet())
                {
                    this._datePollExpire = DateTime.UtcNow.AddDays(this.PollExpire.Text.Trim().ToType<int>());
                }

                pollSaveList.Add(
                    new PollSaveList(
                        this.Question.Text, 
                        rawChoices, 
                        this._datePollExpire, 
                        this.PageContext.PageUserID, 
                        realTopic, 
                        this._forumId, 
                        this._categoryId, 
                        this._boardId, 
                        questionPath, 
                        questionMime, 
                        this.IsBoundCheckBox.Checked, 
                        this.IsClosedBoundCheckBox.Checked, 
                        this.AllowMultipleChoicesCheckBox.Checked, 
                        this.ShowVotersCheckBox.Checked, 
                        this.AllowSkipVoteCheckBox.Checked));
                LegacyDb.poll_save(pollSaveList);
                return true;
            }
        }

        /// <summary>
        /// The init poll ui.
        /// </summary>
        /// <param name="pollID">
        /// The poll ID.
        /// </param>
        private void InitPollUI(int? pollID)
        {
            // we should get the schema anyway
            this._choices = LegacyDb.poll_stats(pollID);
            this._choices.Columns.Add("ChoiceOrderID", typeof(int));

            // First existing values alway 1!
            var existingRowsCount = 1;
            var allExistingRowsCount = this._choices.Rows.Count;

            // we edit existing poll 
            if (this._choices.HasRows())
            {
                if ((this._choices.Rows[0]["UserID"].ToType<int>() != this.PageContext.PageUserID) &&
                    (!this.PageContext.IsAdmin) && (!this.PageContext.ForumModeratorAccess))
                {
                    YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
                }

                this.IsBoundCheckBox.Checked = this._choices.Rows[0]["IsBound"].ToType<bool>();
                this.IsClosedBoundCheckBox.Checked = this._choices.Rows[0]["IsClosedBound"].ToType<bool>();
                this.AllowMultipleChoicesCheckBox.Checked = this._choices.Rows[0]["AllowMultipleChoices"].ToType<bool>();
                this.AllowSkipVoteCheckBox.Checked = this._choices.Rows[0]["AllowSkipVote"].ToType<bool>();
                this.ShowVotersCheckBox.Checked = this._choices.Rows[0]["ShowVoters"].ToType<bool>();
                this.Question.Text = this._choices.Rows[0]["Question"].ToString();
                this.QuestionObjectPath.Text = this._choices.Rows[0]["QuestionObjectPath"].ToString();

                if (this._choices.Rows[0]["Closes"] != DBNull.Value)
                {
                    var closing = (DateTime)this._choices.Rows[0]["Closes"] - DateTime.UtcNow;

                    this.PollExpire.Text = (closing.TotalDays + 1).ToType<int>().ToString();
                }
                else
                {
                    this.PollExpire.Text = null;
                }

                foreach (DataRow choiceRow in this._choices.Rows)
                {
                    choiceRow["ChoiceOrderID"] = existingRowsCount;

                    existingRowsCount++;
                }
            }
            else
            {
                // A new topic is created
                // below check currently if works for topics only, but will do as some things are not enabled 
                if (!this.CanCreatePoll())
                {
                    YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                }
                
                // Get isBound value using page variables. They are initialized here.
                var pgidt = 0;

                // If a topic poll is edited or new topic created
                if (this._topicId > 0 && this.topicInfo != null)
                {
                    // topicid should not be null here 
                    if (!this.topicInfo.PollID.HasValue)
                    {
                        pgidt = this.topicInfo.PollID.Value;

                        var pollGroupData = LegacyDb.pollgroup_stats(pgidt);

                        this.IsBoundCheckBox.Checked = Convert.ToBoolean(pollGroupData.Rows[0]["IsBound"]);
                        //// this.IsClosedBoundCheckBox.Checked = Convert.ToBoolean(DB.pollgroup_stats(pgidt).Rows[0]["IsClosedBound"]);
                    }
                }
                else if (this._forumId > 0 && (!(this._topicId > 0) || (!(this._editTopicId > 0))))
                {
                    // forumid should not be null here
                    pgidt = (int)LegacyDb.forum_list(this.PageContext.PageBoardID, this._forumId).Rows[0]["PollGroupID"];
                }
                else if (this._categoryId > 0)
                {
                    // categoryid should not be null here
                    pgidt =
                        this.GetRepository<Category>()
                            .Listread(this.PageContext.PageUserID, this._categoryId)
                            .GetFirstRowColumnAsValue("PollGroupID", 0);
                }

                if (pgidt > 0)
                {
                    if (LegacyDb.pollgroup_stats(pgidt).Rows[0]["IsBound"].ToType<int>() == 2)
                    {
                        this.IsBoundCheckBox.Checked = true;
                    }

                    if (LegacyDb.pollgroup_stats(pgidt).Rows[0]["IsClosedBound"].ToType<int>() == 4)
                    {
                        this.IsClosedBoundCheckBox.Checked = true;
                    }
                }

                // clear the fields...
                this.PollExpire.Text = string.Empty;
                this.Question.Text = string.Empty;
            }

            // we add dummy rows to data table to fill in repeater empty fields   
            var dummyRowsCount = this.Get<YafBoardSettings>().AllowedPollChoiceNumber - allExistingRowsCount - 1;
            for (var i = 0; i <= dummyRowsCount; i++)
            {
                var drow = this._choices.NewRow();
                drow["ChoiceOrderID"] = existingRowsCount + i;
                this._choices.Rows.Add(drow);
            }

            // Bind choices repeater
            this.ChoiceRepeater.DataSource = this._choices;
            this.ChoiceRepeater.DataBind();
            this.ChoiceRepeater.Visible = true;

            // Show controls
            this.SavePoll.Visible = true;
            this.Cancel.Visible = true;
            this.PollRow1.Visible = true;
            this.PollRowExpire.Visible = true;
            this.IsClosedBound.Visible =
                this.IsBound.Visible = this.Get<YafBoardSettings>().AllowUsersHidePollResults || PageContext.IsAdmin || PageContext.IsForumModerator;
            this.tr_AllowMultipleChoices.Visible = this.Get<YafBoardSettings>().AllowMultipleChoices || PageContext.IsAdmin
                                                   || PageContext.ForumModeratorAccess;
            this.tr_ShowVoters.Visible = true;
            this.tr_AllowSkipVote.Visible = false;
        }

        /// <summary>
        /// Initializes page context query variables.
        /// </summary>
        private void InitializeVariables()
        {
            this.PageContext.QueryIDs =
                new QueryStringIDHelper(
                    new[] { "p", "ra", "ntp", "t", "e", "em", "m", "f", "ef", "c", "ec", "b", "eb", "rf" });

            // we return to a specific place, general token 
            if (this.PageContext.QueryIDs.ContainsKey("ra"))
            {
                this._topicUnapproved = true;
            }

            // we return to a forum (used when a topic should be approved)
            if (this.PageContext.QueryIDs.ContainsKey("f"))
            {
                this._forumId = this._returnForum = this.PageContext.QueryIDs["f"].ToType<int>();
            }

            if (this.PageContext.QueryIDs.ContainsKey("t"))
            {
                this._topicId = this.PageContext.QueryIDs["t"].ToType<int>();
                this.topicInfo = this.GetRepository<Topic>().GetById(this._topicId.ToType<int>());
            }

            if (this.PageContext.QueryIDs.ContainsKey("m"))
            {
                this._editMessageId = this.PageContext.QueryIDs["m"].ToType<int>();
            }

            if (this._editMessageId == null)
            {
                if (this.PageContext.QueryIDs.ContainsKey("ef"))
                {
                    this._categoryId = this.PageContext.QueryIDs["ef"].ToType<int>();
                }

                if (this.editForumId == null)
                {
                    if (this.PageContext.QueryIDs.ContainsKey("c"))
                    {
                        this._categoryId = this.PageContext.QueryIDs["c"].ToType<int>();
                    }

                    if (this._categoryId == null)
                    {
                        if (this.PageContext.QueryIDs.ContainsKey("ec"))
                        {
                            this._editCategoryId = this.PageContext.QueryIDs["ec"].ToType<int>();
                        }

                        if (this._editCategoryId == null)
                        {
                            if (this.PageContext.QueryIDs.ContainsKey("b"))
                            {
                                this._boardId = this.PageContext.QueryIDs["b"].ToType<int>();
                            }

                            if (this._boardId == null)
                            {
                                if (this.PageContext.QueryIDs.ContainsKey("eb"))
                                {
                                    this._editBoardId = this.PageContext.QueryIDs["eb"].ToType<int>();
                                }
                            }
                        }
                    }
                }
            }

            // Check if the user has the page access and variables are correct. 
            this.CheckAccess();

            // handle poll
            if (this.PageContext.QueryIDs.ContainsKey("p"))
            {
                // edit existing poll
                this.PollId = this.PageContext.QueryIDs["p"].ToType<int>();
                this.InitPollUI(this.PollId);
            }
            else
            {
                // new poll
                this.PollRow1.Visible = true;
                this.InitPollUI(null);
            }

            // YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
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
            if (this._editMessageId > 0)
            {
                retliterals = "em";
                retvalue = this._editMessageId;
            }
            else if (this._topicId > 0)
            {
                retliterals = "t";
                retvalue = this._topicId;
            }
            else if (this._forumId > 0)
            {
                retliterals = "f";
                retvalue = this._forumId;
            }
            else if (this.editForumId > 0)
            {
                retliterals = "ef";
                retvalue = this.editForumId;
            }
            else if (this._categoryId > 0)
            {
                retliterals = "c";
                retvalue = this._categoryId;
            }
            else if (this._editCategoryId > 0)
            {
                retliterals = "ec";
                retvalue = this._editCategoryId;
            }
            else if (this._boardId > 0)
            {
                retliterals = "b";
                retvalue = this._boardId;
            }
            else if (this._editBoardId > 0)
            {
                retliterals = "eb";
                retvalue = this._editBoardId;
            }
            else
            {
                retliterals = string.Empty;
                retvalue = 0;
            }

            /* else
                   {
                       YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
                   } */
        }

        /// <summary>
        /// The return to page.
        /// </summary>
        private void ReturnToPage()
        {
            if (this._topicUnapproved)
            {
                // Tell user that his message will have to be approved by a moderator
                var url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this._returnForum);

                if (Config.IsRainbow)
                {
                    YafBuildLink.Redirect(ForumPages.info, "i=1");
                }
                else
                {
                    YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
            }

            // YafBuildLink.Redirect(ForumPages.posts, "m={0}#{0}", this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("m"));      
            string retliterals;
            int? retvalue;

            this.ParamsToSend(out retliterals, out retvalue);

            switch (retliterals)
            {
                case "t":
                    YafBuildLink.Redirect(ForumPages.posts, "t={0}", retvalue);
                    break;

                case "em":

                    YafBuildLink.Redirect(ForumPages.postmessage, "m={0}", retvalue);
                    break;

                case "f":

                    YafBuildLink.Redirect(ForumPages.topics, "f={0}", retvalue);
                    break;
                case "ef":
                    YafBuildLink.Redirect(ForumPages.admin_editforum, "f={0}", retvalue);
                    break;
                case "c":
                    YafBuildLink.Redirect(ForumPages.forum, "c={0}", retvalue);
                    break;
                case "ec":
                    YafBuildLink.Redirect(ForumPages.admin_editcategory, "c={0}", retvalue);
                    break;
                case "b":
                    YafBuildLink.Redirect(ForumPages.forum);
                    break;
                case "eb":
                    YafBuildLink.Redirect(ForumPages.admin_editboard, "b={0}", retvalue);
                    break;
                default:
                    YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
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
            if (!(this._topicId > 0))
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

            if (pollGroupId == null && this.Get<YafBoardSettings>().AllowedPollNumber > 0 &&
                this.PageContext.ForumPollAccess)
            {
                return true;
            }

            // TODO: repeating code
            // Remove repeating PollID values   
            var hTable = new Hashtable();
            var duplicateList = new ArrayList();
            var dtPollGroup = LegacyDb.pollgroup_stats(pollGroupId);

            foreach (DataRow drow in dtPollGroup.Rows)
            {
                if (hTable.Contains(drow["PollID"]))
                {
                    duplicateList.Add(drow);
                }
                else
                {
                    hTable.Add(drow["PollID"], string.Empty);
                }
            }

            foreach (DataRow dRow in duplicateList)
            {
                dtPollGroup.Rows.Remove(dRow);
            }

            dtPollGroup.AcceptChanges();

            // frequently used
            var pollNumber = dtPollGroup.Rows.Count;

            return (pollNumber < this.Get<YafBoardSettings>().AllowedPollNumber) &&
                   (this.Get<YafBoardSettings>().AllowedPollChoiceNumber > 0);
        }

        #endregion
    }
}