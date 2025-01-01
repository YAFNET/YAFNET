﻿/* Yet Another Forum.NET
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

namespace YAF.Controls;

using System.Text;

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// PollList Class
/// </summary>
public partial class PollChoiceList : BaseUserControl
{
    /// <summary>
    ///   The event bubbles info to parent control to rebind repeater.
    /// </summary>
    public event EventHandler ChoiceVoted;

    /// <summary>
    ///   Gets or sets a value indicating whether user can vote
    /// </summary>
    public bool CanVote { get; set; }

    /// <summary>
    ///   Gets or sets the Choice Id array.
    /// </summary>
    public List<PollVote> UserPollVotes { get; set; }

    /// <summary>
    ///   Gets or sets the data source.
    /// </summary>
    public List<Tuple<Poll, Choice>> DataSource { get; set; }

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
    ///   Gets or sets Voters. Stores users which are voted for a choice.
    /// </summary>
    public List<Tuple<PollVote, User>> Voters { get; set; }

    /// <summary>
    ///   Gets or sets the PollId for the choices.
    /// </summary>
    public int PollId { get; set; }

    /// <summary>
    ///   Gets or sets number of votes.
    /// </summary>
    public int Votes { get; set; }

    /// <summary>
    /// The Page_Load event.
    /// </summary>
    /// <param name="sender">
    /// The event sender.
    /// </param>
    /// <param name="e">
    /// The EventArgs e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
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
    protected void Poll_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "vote")
        {
            return;
        }

        if (!this.CanVote)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_ALREADY_VOTED"), MessageTypes.warning);
            return;
        }

        if (this.IsLocked)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_TOPIC_LOCKED"), MessageTypes.warning);
            return;
        }

        if (this.IsClosed)
        {
            this.PageBoardContext.Notify(this.GetText("WARN_POLL_CLOSED"), MessageTypes.warning);
            return;
        }

        var choiceId = e.CommandArgument.ToType<int>();

        this.GetRepository<Choice>().Vote(choiceId);

        this.GetRepository<PollVote>().Vote(choiceId, this.PageBoardContext.PageUserID, this.PollId);

        this.BindData();

        // Initialize bubble event to update parent control.
        this.ChoiceVoted?.Invoke(source, e);

        this.PageBoardContext.Notify(this.GetText("INFO_VOTED"), MessageTypes.success);
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
    protected void Poll_OnItemDataBound(object source, RepeaterItemEventArgs e)
    {
        var item = e.Item;
        var choice = (Tuple<Poll, Choice>)e.Item.DataItem;

        if (item.ItemType != ListItemType.Item && item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        // Voting link 
        var voteButton = item.FindControlRecursiveAs<ThemeButton>("VoteButton");

        var myChoiceMarker = item.FindControlRecursiveAs<Label>("YourChoice");

        if (this.UserPollVotes.Any() && this.UserPollVotes.Exists(v => choice.Item2.ID == v.ChoiceID))
        {
            myChoiceMarker.Visible = true;
        }

        if (!this.Voters.NullOrEmpty())
        {
            var voters = item.FindControlRecursiveAs<Label>("Voters");
            var voterNames = new StringBuilder();

            var votersByChoice = this.Voters.Where(i => i.Item1.ChoiceID == choice.Item2.ID).ToList();

            if (votersByChoice.Any())
            {
                voterNames.Append("(");

                votersByChoice.ForEach(
                    itemTuple => voterNames.AppendFormat(
                        "{0}, ",
                        itemTuple.Item2.DisplayOrUserName()));

                voterNames.Remove(voterNames.Length - 2, 2);

                voterNames.Append(")");

                voters.Text = voterNames.ToString();
            }
        }

        voteButton.Enabled = this.CanVote && !myChoiceMarker.Visible;
        voteButton.Visible = true;

        // override if multi vote
        if (this.DataSource[0].Item1.PollFlags.AllowMultipleChoice && this.UserPollVotes.TrueForAll(v => choice.Item2.ID != v.ChoiceID))
        {
            voteButton.Enabled = true;
        }

        // Poll Choice image
        var choiceImage = item.FindControlRecursiveAs<Image>("ChoiceImage");

        // Don't render if it's a standard image
        if (choice.Item2.ObjectPath.IsSet())
        {
            choiceImage.ImageUrl = this.HtmlEncode(choice.Item2.ObjectPath);

            choiceImage.AlternateText =
                choiceImage.ToolTip = this.Get<IBadWordReplace>().Replace(choice.Item2.ChoiceName);
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
    protected int VoteWidth(object o)
    {
        try
        {
            var itemTuple = (Tuple<Poll, Choice>)o;

            var votes = this.DataSource.Sum(x => x.Item2.Votes);

            return 100 * itemTuple.Item2.Votes / votes;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.DataBind();
    }
}