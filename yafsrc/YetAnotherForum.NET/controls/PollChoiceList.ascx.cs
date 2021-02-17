/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
        /// Returns image height.
        /// </returns>
        protected int GetImageHeight([NotNull] object mimeType)
        {
            var attrs = mimeType.ToString().Split('!')[1].Split(';');
            return attrs[1].ToType<int>();
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
                this.PageContext.AddLoadMessage(this.GetText("WARN_ALREADY_VOTED"), MessageTypes.warning);
                return;
            }

            if (this.IsLocked)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_TOPIC_LOCKED"), MessageTypes.warning);
                return;
            }

            if (this.IsClosed)
            {
                this.PageContext.AddLoadMessage(this.GetText("WARN_POLL_CLOSED"), MessageTypes.warning);
                return;
            }

            int? userID = null;
            var remoteIP = string.Empty;

            if (this.PageContext.BoardSettings.PollVoteTiedToIP)
            {
                remoteIP = IPHelper.IPStringToLong(this.Get<HttpRequestBase>().ServerVariables["REMOTE_ADDR"]).ToString();
            }

            if (!this.PageContext.IsGuest)
            {
                userID = this.PageContext.PageUserID;
            }

            var choiceId = e.CommandArgument.ToType<int>();

            this.GetRepository<Choice>().Vote(choiceId);

            this.GetRepository<PollVote>().Vote(choiceId, userID, this.PollId, remoteIP);

            // save the voting cookie...
            var cookieCurrent = string.Empty;

            // We check whether is a vote for an option  
            if (this.Get<HttpRequestBase>().Cookies[VotingCookieName(this.PollId.ToType<int>())] != null)
            {
                // Add the voted option to cookie value string
                cookieCurrent = $"{this.Get<HttpRequestBase>().Cookies[VotingCookieName(this.PollId.ToType<int>())].Value},";
                this.Get<HttpRequestBase>().Cookies.Remove(VotingCookieName(this.PollId.ToType<int>()));
            }

            var c = new HttpCookie(
                            VotingCookieName(this.PollId),
                            $"{cookieCurrent}{e.CommandArgument}")
                            {
                                Expires = DateTime.UtcNow
                                    .AddYears(1)
                            };

            this.Get<HttpResponseBase>().Cookies.Add(c);

            // show an info that the user is voted 
            var msg = this.GetText("INFO_VOTED");

            this.BindData();

            // Initialize bubble event to update parent control.
            this.ChoiceVoted?.Invoke(source, e);

            // show the notification  window to user
            this.PageContext.AddLoadMessage(msg, MessageTypes.success);
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
            var trow = item.FindControlRecursiveAs<PlaceHolder>("VoteTr");

            if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            // Voting link 
            var myLinkButton = item.FindControlRecursiveAs<LinkButton>("MyLinkButton1");

            var myChoiceMarker = item.FindControlRecursiveAs<Label>("YourChoice");
            if (this.ChoiceId != null)
            {
                foreach (var mychoice in this.ChoiceId.Where(choice => drowv.Row["ChoiceID"].ToType<int>() == choice))
                {
                    myChoiceMarker.Visible = true;
                }

                /*if (this.Voters != null)
                {
                    // TODO:
                    var voters = item.FindControlRecursiveAs<Label>("Voters");
                    var voterNames = string.Empty;

                    foreach (DataRow row in this.Voters.Rows.Cast<DataRow>().ForEach(row =>
                    {
                        if (row["ChoiceID"].ToType<int>() == drowv["ChoiceID"].ToType<int>() && row["PollID"].ToType<int>() == this.PollId)
                        {
                            voterNames += row["UserName"] + ",";
                        }
                    }

                    voters.Text = voterNames;

                }*/
            }


            myLinkButton.ToolTip = this.GetText("POLLEDIT", "POLL_PLEASEVOTE");
            myLinkButton.Enabled = this.CanVote && !myChoiceMarker.Visible;
            myLinkButton.Visible = true;

            if (!myLinkButton.Enabled)
            {
                myLinkButton.CssClass = "btn btn-success btn-sm disabled";
            }

            // Poll Choice image
            var choiceImage = item.FindControlRecursiveAs<HtmlImage>("ChoiceImage");

            // Don't render if it's a standard image
            if (!drowv.Row["ObjectPath"].IsNullOrEmptyDBField())
            {
                choiceImage.Src = this.HtmlEncode(drowv.Row["ObjectPath"].ToString());

                if (!drowv.Row["MimeType"].IsNullOrEmptyDBField())
                {
                    var aspect = GetImageAspect(drowv.Row["MimeType"]);
                    var imageWidth = 80;

                    choiceImage.Width = imageWidth;
                    choiceImage.Height = (choiceImage.Width / aspect).ToType<int>();

                    choiceImage.Attributes["style"] = $"width:{imageWidth}px; height:{choiceImage.Height}px;";
                }
            }
            else
            {
                choiceImage.Visible = false;
            }

            item.FindControlRecursiveAs<Panel>("resultsSpan").Visible = !this.HideResults;
            item.FindControlRecursiveAs<Label>("VoteSpan").Visible = !this.HideResults;
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
            if (mimeType.IsNullOrEmptyDBField())
            {
                return 1;
            }

            var attrs = mimeType.ToString().Split('!')[1].Split(';');
            var width = attrs[0].ToType<decimal>();
            return width / attrs[1].ToType<decimal>();
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
            return $"poll#{pollId}";
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