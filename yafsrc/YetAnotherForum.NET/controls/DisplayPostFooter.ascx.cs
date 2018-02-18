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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The display post footer.
    /// </summary>
    public partial class DisplayPostFooter : BaseUserControl
    {
        #region Constants and Fields

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the DataRow.
        /// </summary>
        [CanBeNull]
        public DataRow DataRow
        {
            get
            {
                return this.PostData.DataRow;
            }

            set
            {
                this.PostData = new PostDataHelperWrapper(value);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest
        {
            get
            {
                return this.PostData == null || UserMembershipHelper.IsGuestUser(this.PostData.UserId);
            }
        }

        /// <summary>
        ///   Gets access Post Data helper functions.
        /// </summary>
        public PostDataHelperWrapper PostData { get; private set; }

        /// <summary>
        ///   Gets the Provides access to the Toggle Post button.
        /// </summary>
        public ThemeButton TogglePost
        {
            get
            {
                return this.btnTogglePost;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.DisplayPostFooterPreRender;
            base.OnInit(e);
        }

        /// <summary>
        /// Marks as answer click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void MarkAsAnswerClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var messageFlags =
                new MessageFlags(this.PostData.DataRow["Flags"]) { IsAnswer = true };

            // Remove Answer from other messages to avoid duplicate answers!
            this.GetRepository<Topic>().RemoveAnswerMessage(topicId: this.PageContext.PageTopicID);

            if (this.PostData.PostIsAnswer)
            {
                // Remove Current Message 
                messageFlags.IsAnswer = false;

                this.GetRepository<Message>().UpdateFlags(messageId: this.PostData.MessageId, flags: messageFlags.BitValue);
            }
            else
            {
                messageFlags.IsAnswer = true;

                this.GetRepository<Topic>().SetAnswerMessage(topicId: this.PageContext.PageTopicID, messageId: this.PostData.MessageId);

                this.GetRepository<Message>().UpdateFlags(messageId: this.PostData.MessageId, flags: messageFlags.BitValue);
            }

            YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", this.PostData.MessageId);
        }

        /// <summary>
        /// The display post footer_ pre render.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DisplayPostFooterPreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // report post
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ReportPostPermissions)
                && !this.PostData.PostDeleted)
            {
                if (!this.PageContext.IsGuest && this.PageContext.User != null)
                {
                    this.ReportPost.Visible = true;

                    this.ReportPost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.reportpost,
                        "m={0}",
                        this.PostData.MessageId);
                }
            }

            // mark post as answer
            if (!this.PostData.PostDeleted && !this.PageContext.IsGuest && this.PageContext.User != null
                && this.PageContext.PageUserID.Equals(this.DataRow["TopicOwnerID"].ToType<int>())
                && !this.PostData.UserId.Equals(this.PageContext.PageUserID))
            {
                this.MarkAsAnswer.Visible = true;

                if (this.PostData.PostIsAnswer)
                {
                    this.MarkAsAnswer.TextLocalizedTag = "MARK_ANSWER_REMOVE";
                    this.MarkAsAnswer.TitleLocalizedTag = "MARK_ANSWER_REMOVE_TITLE";
                    this.MarkAsAnswer.Icon = "minus-square";
                }
                else
                {
                    this.MarkAsAnswer.TextLocalizedTag = "MARK_ANSWER";
                    this.MarkAsAnswer.TitleLocalizedTag = "MARK_ANSWER_TITLE";
                    this.MarkAsAnswer.Icon = "check-square";
                }
            }
        }

        #endregion
    }
}