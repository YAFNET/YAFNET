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
    using System.Linq;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// PollList Class
    /// </summary>
    public partial class PollChoiceList : BaseUserControl
    {
        #region Events

        /// <summary>
        ///   The event bubbles info to parent control to rebind repeater.
        /// </summary>
        public event EventHandler ChoiceVoted;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether user can vote
        /// </summary>
        public bool CanVote { get; set; }

        /// <summary>
        ///   Gets or sets the Choice Id array.
        /// </summary>
        public int?[] ChoiceId { get; set; }

        /// <summary>
        ///   Gets or sets the data source.
        /// </summary>
        public DataTable DataSource { get; set; }

        /// <summary>
        ///   Gets or sets number of  days to run.
        /// </summary>
        public int? DaysToRun { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether to hide results.
        /// </summary>
        public bool HideResults { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether parent topic IsClosed
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether parent topic IsLocked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        ///   Gets or sets MaxImageAspect. Stores max aspect to get rows of equal height.
        /// </summary>
        public decimal MaxImageAspect { get; set; }

        /// <summary>
        ///   Gets or sets Voters. Stores users which are voted for a choice.
        /// </summary>
        public DataTable Voters { get; set; }

        /// <summary>
        ///   Gets or sets the PollId for the choices.
        /// </summary>
        public int PollId { get; set; }

        /// <summary>
        ///   Gets or sets number of votes.
        /// </summary>
        public int Votes { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The get image height.
        /// </summary>
        /// <param name="mimeType">
        /// The mime type.
        /// </param>
        /// <returns>
        /// Returnes image height.
        /// </returns>
        protected int GetImageHeight([NotNull] object mimeType)
        {
            var attrs = mimeType.ToString().Split('!')[1].Split(';');
            return attrs[1].ToType<int>();
        }

        /// <summary>
        /// The get poll question.
        /// </summary>
        /// <returns>
        /// Returns poll question string.
        /// </returns>
        [NotNull]
        protected string GetPollQuestion()
        {
            return this.DataSource.Rows[0]["Question"].ToString();
        }

        /// <summary>
        /// Get Theme Contents
        /// </summary>
        /// <param name="page">
        /// The Localization Page.
        /// </param>
        /// <param name="tag">
        /// The Localisation Page Tag.
        /// </param>
        /// <returns>
        /// Returns Theme Content.
        /// </returns>
        protected string GetThemeContents([NotNull] string page, [NotNull] string tag)
        {
            return this.Get<ITheme>().GetItem(page, tag);
        }

        /// <summary>
        /// The get total.
        /// </summary>
        /// <param name="pollId">
        /// The poll Id.
        /// </param>
        /// <returns>
        /// Returns total string.
        /// </returns>
        [NotNull]
        protected string GetTotal([NotNull] object pollId)
        {
            return this.DataSource.Rows[0]["Total"].ToString();
        }

        /// <summary>
        /// The Page_Load event.
        /// </summary>
        /// <param name="sender">
        /// The event sender.
        /// </param>
        /// <param name="e">
        /// The EventArgs e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.LoadMessage.Clear();
            this.BindData();
        }

        /// <summary>
        /// The poll_ item command.
        /// </summary>
        /// <param name="source">
        /// The object source.
        /// </param>
        /// <param name="e">
        /// The RepeaterCommandEventArgs e.
        /// </param>
        protected void Poll_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "vote")
            {
                return;
            }

            if (!this.CanVote)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_ALREADY_VOTED"));
                return;
            }

            if (this.IsLocked)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"));
                return;
            }

            if (this.IsClosed)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_POLL_CLOSED"));
                return;
            }

            object userID = null;
            object remoteIP = null;

            if (this.PageContext.BoardSettings.PollVoteTiedToIP)
            {
                remoteIP = IPHelper.IPStringToLong(this.Request.ServerVariables["REMOTE_ADDR"]).ToString();
            }

            if (!this.PageContext.IsGuest)
            {
                userID = this.PageContext.PageUserID;
            }

            LegacyDb.choice_vote(e.CommandArgument, userID, remoteIP);

            // save the voting cookie...
            var cookieCurrent = String.Empty;

            // We check whether is a vote for an option  
            if (this.Request.Cookies[VotingCookieName(Convert.ToInt32(this.PollId))] != null)
            {
                // Add the voted option to cookie value string
                cookieCurrent = "{0},".FormatWith(
                    this.Request.Cookies[VotingCookieName(Convert.ToInt32(this.PollId))].Value);
                this.Request.Cookies.Remove(VotingCookieName(Convert.ToInt32(this.PollId)));
            }

            var c = new HttpCookie(
                            VotingCookieName(this.PollId),
                            "{0}{1}".FormatWith(cookieCurrent, e.CommandArgument.ToString()))
                            {
                                Expires = DateTime.UtcNow
                                    .AddYears(1)
                            };

            this.Response.Cookies.Add(c);

            // show an info that the user is voted 
            var msg = this.GetText("INFO_VOTED");

            this.BindData();

            // Initialize bubble event to update parent control.
            if (this.ChoiceVoted != null)
            {
                this.ChoiceVoted(source, e);
            }

            // show the notification  window to user
            this.PageContext.AddLoadMessage(msg);
        }

        /// <summary>
        /// The poll_ on item data bound.
        /// </summary>
        /// <param name="source">
        /// The event source.
        /// </param>
        /// <param name="e">
        /// The RepeaterItemEventArgs e.
        /// </param>
        protected void Poll_OnItemDataBound([NotNull] object source, [NotNull] RepeaterItemEventArgs e)
        {
            var item = e.Item;
            var drowv = (DataRowView)e.Item.DataItem;
            var trow = item.FindControlRecursiveAs<HtmlTableRow>("VoteTr");

            if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            // Voting link 
            var myLinkButton = item.FindControlRecursiveAs<LinkButton>("MyLinkButton1");

            var myChoiceMarker = item.FindControlRecursiveAs<HtmlImage>("YourChoice");
            if (this.ChoiceId != null)
            {
                foreach (var mychoice in this.ChoiceId.Where(mychoice => (int)drowv.Row["ChoiceID"] == mychoice))
                {
                    myChoiceMarker.Visible = true;
                }

                if (Voters != null)
                {
                    var himage = item.FindControlRecursiveAs<HtmlImage>("ImgVoteBar");
                    foreach (DataRow row in this.Voters.Rows)
                    {
                        if ((int)row["ChoiceID"] == (int)drowv["ChoiceID"] && (int)row["PollID"] == PollId)
                        {
                            himage.Attributes["title"] = himage.Attributes["title"] + row["UserName"] + ",";
                        }
                    }

                    if (himage.Attributes["title"].IsSet())
                    {
                        himage.Attributes["title"] = himage.Attributes["alt"] = himage.Attributes["title"].TrimEnd(',');
                    }

                }
            }


            myLinkButton.ToolTip = this.GetText("POLLEDIT", "POLL_PLEASEVOTE");
            myLinkButton.Enabled = this.CanVote && !myChoiceMarker.Visible;
            myLinkButton.Visible = true;

            // Poll Choice image
            var choiceImage = item.FindControlRecursiveAs<HtmlImage>("ChoiceImage");
            var choiceAnchor = item.FindControlRecursiveAs<HtmlAnchor>("ChoiceAnchor");

            // Don't render if it's a standard image
            if (!drowv.Row["ObjectPath"].IsNullOrEmptyDBField())
            {
                // choiceAnchor.Attributes["rel"] = "lightbox-group" + Guid.NewGuid().ToString().Substring(0, 5);
                choiceAnchor.HRef = drowv.Row["ObjectPath"].IsNullOrEmptyDBField()
                                        ? this.GetThemeContents("VOTE", "POLL_CHOICE")
                                        : this.HtmlEncode(drowv.Row["ObjectPath"].ToString());

                // choiceAnchor.Title = drowv.Row["ObjectPath"].ToString();
                choiceImage.Src = this.HtmlEncode(drowv.Row["ObjectPath"].ToString());

                if (!drowv.Row["MimeType"].IsNullOrEmptyDBField())
                {
                    var aspect = GetImageAspect(drowv.Row["MimeType"]);
                    var imageWidth = 80;

                    if (this.Get<IYafSession>().UseMobileTheme ?? false)
                    {
                        imageWidth = 40;
                    }

                    choiceImage.Width = imageWidth;
                    choiceImage.Height = Convert.ToInt32(choiceImage.Width / aspect);

                    choiceImage.Attributes["style"] =
                        "width:{0}px; height:{1}px;".FormatWith(imageWidth, choiceImage.Height);

                    // reserved to get equal row heights
                    var height = Convert.ToInt32(this.MaxImageAspect * choiceImage.Width);
                    trow.Attributes["style"] = "height:{0}px;".FormatWith(height);
                }
            }
            else
            {


                choiceImage.Alt = this.GetText("POLLEDIT", "POLL_PLEASEVOTE");
                choiceImage.Src = this.GetThemeContents("VOTE", "POLL_CHOICE");
                choiceAnchor.HRef = string.Empty;
            }

            item.FindControlRecursiveAs<Panel>("MaskSpan").Visible = this.HideResults;
            item.FindControlRecursiveAs<Panel>("resultsSpan").Visible = !this.HideResults;
            item.FindControlRecursiveAs<Panel>("VoteSpan").Visible = !this.HideResults;
        }

        /// <summary>
        /// The remove poll_ completely load.
        /// </summary>
        /// <param name="sender">
        /// The object sender.
        /// </param>
        /// <param name="e">
        /// The EventArgs e.
        /// </param>
        protected void RemovePollCompletely_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
        }

        /// <summary>
        /// The remove poll_ load.
        /// </summary>
        /// <param name="sender">
        /// The object sender.
        /// </param>
        /// <param name="e">
        /// The EventArgs e.
        /// </param>
        protected void RemovePoll_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            ((ThemeButton)sender).Attributes["onclick"] =
                "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLL_DELETE"));
        }

        /// <summary>
        /// The vote width.
        /// </summary>
        /// <param name="o">
        /// The object o.
        /// </param>
        /// <returns>
        /// Returns the vote width.
        /// </returns>
        protected int VoteWidth([NotNull] object o)
        {
            var row = (DataRowView)o;
            return row.Row["Stats"].ToType<int>() * 80 / 100;
        }

        /// <summary>
        /// Returns an image width|height ratio.
        /// </summary>
        /// <param name="mimeType">
        /// The mime type of the image.
        /// </param>
        /// <returns>
        /// The get image aspect.
        /// </returns>
        private static decimal GetImageAspect([NotNull] object mimeType)
        {
            if (!mimeType.IsNullOrEmptyDBField())
            {
                var attrs = mimeType.ToString().Split('!')[1].Split(';');
                var width = attrs[0].ToType<decimal>();
                return width / attrs[1].ToType<decimal>();
            }

            return 1;
        }

        /// <summary>
        /// Gets VotingCookieName.
        /// </summary>
        /// <param name="pollId">
        /// The poll Id.
        /// </param>
        /// <returns>
        /// The voting cookie name.
        /// </returns>
        private static string VotingCookieName(int pollId)
        {
            return "poll#{0}".FormatWith(pollId);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.DataBind();
        }

        /// <summary>
        /// The get poll is closed.
        /// </summary>
        /// <returns>
        /// Returns a 'poll is closed' warning string.
        /// </returns>
        private string GetPollIsClosed()
        {
            var strPollClosed = string.Empty;
            if (this.IsClosed)
            {
                strPollClosed = this.GetText("POLL_CLOSED");
            }

            return strPollClosed;
        }

        /// <summary>
        /// Checks if a poll has no votes.
        /// </summary>
        /// <param name="pollId">
        /// The poll id.
        /// </param>
        /// <returns>
        /// The poll has no votes.
        /// </returns>
        private bool PollHasNoVotes([NotNull] object pollId)
        {
            return this.DataSource.Rows.Cast<DataRow>().Where(dr => dr["PollID"].ToType<int>() == pollId.ToType<int>())
                .All(dr => dr["Votes"].ToType<int>() <= 0);
        }

        #endregion
    }
}