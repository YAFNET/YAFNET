/* Yet Another Forum.net
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
  // YAF.Pages
  #region Using

  using System;
  using System.Collections;
  using System.Data;
  using System.Linq;
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// PollList Class
  /// </summary>
  public partial class PollList : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The group Notification String.
    /// </summary>
    private readonly string _groupNotificationString = string.Empty;

    /// <summary>
    ///   The _canChange.
    /// </summary>
    private bool _canChange;

    /// <summary>
    ///   The _dt Votes.
    /// </summary>
    private DataTable _dtAllPollGroupVotes;

    /// <summary>
    ///   The _dt poll.
    /// </summary>
    private DataTable _dtPollAllChoices;

    /// <summary>
    ///   The _dt PollGroup.
    /// </summary>
    private DataTable _dtPollGroupAllChoices;

    /// <summary>
    ///   The isBound.
    /// </summary>
    private bool _isBound;

    /// <summary>
    ///   The IsVoteEvent
    /// </summary>
    private bool _isVoteEvent;

    /// <summary>
    ///   The _showResults. Used to store data from parent repeater.
    /// </summary>
    private bool _showResults;

    /// <summary>
    ///   The topic User.
    /// </summary>
    private int? _topicUser;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets BoardId
    /// </summary>
    public int BoardId { get; set; }

    /// <summary>
    ///   Gets or sets CategoryId
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    ///   Gets or sets EditBoardId.
    ///   Used to return to edit board page.
    ///   Currently is not implemented.
    /// </summary>
    public int EditBoardId { get; set; }

    /// <summary>
    ///   Gets or sets EditCategoryId
    /// </summary>
    public int EditCategoryId { get; set; }

    /// <summary>
    ///   Gets or sets EditForumId
    /// </summary>
    public int EditForumId { get; set; }

    /// <summary>
    ///   Gets or sets EditMessageId.
    /// </summary>
    public int EditMessageId { get; set; }

    /// <summary>
    ///   Gets or sets ForumId
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether the parent topic is locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    ///   Gets or sets MaxImageAspect. Stores max aspect to get rows of equal height.
    /// </summary>
    public decimal MaxImageAspect { get; set; }

    /// <summary>
    ///   Gets or sets PollGroupID
    /// </summary>
    public int? PollGroupId { get; set; }

    /// <summary>
    ///   Gets or sets PollNumber
    /// </summary>
    public int PollNumber { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether  we are editing a post
    /// </summary>
    public bool PostEdit { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether show poll management buttons.
    /// </summary>
    public bool ShowButtons { get; set; }

    /// <summary>
    ///   Gets or sets TopicId
    /// </summary>
    public int TopicId { get; set; }

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
        return (this.PollNumber < this.Get<YafBoardSettings>().AllowedPollNumber) &&
             (this.Get<YafBoardSettings>().AllowedPollChoiceNumber > 0) && this.HasOwnerExistingGroupAccess() &&
             (this.PollGroupId >= 0);
    }

    /// <summary>
    /// Checks if user can edit a poll
    /// </summary>
    /// <param name="pollId">
    /// </param>
    /// <returns>
    /// The can edit poll.
    /// </returns>
    protected bool CanEditPoll([NotNull] object pollId)
    {
      // The host admin can forbid a user to change poll after first vote to avoid fakes.    
        if (!this.Get<YafBoardSettings>().AllowPollChangesAfterFirstVote)
      {
        // Only if show buttons are enabled user can edit poll 
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                (this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>() &&
                 (this.PollHasNoVotes(pollId) && (!this.IsPollClosed(pollId)))));
      }

      // we don't call PollHasNoVotes method here
      return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>() &&
               (!this.IsPollClosed(pollId))));
    }

    /// <summary>
    /// Checks if a user can remove all polls in a group
    /// </summary>
    /// <returns>
    /// The can remove group.
    /// </returns>
    protected bool CanRemoveGroup()
    {
      bool hasNoVotes = true;

      foreach (DataRow dr in this._dtPollAllChoices.Rows.Cast<DataRow>().Where(dr => dr["Votes"].ToType<int>() > 0))
      {
        hasNoVotes = false;
      }

      if (!this.Get<YafBoardSettings>().AllowPollChangesAfterFirstVote)
      {
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                (this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>() &&
                 hasNoVotes));
      }

      return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>()));
    }

    /// <summary>
    /// Checks if  a user can remove all polls in a group completely.
    /// </summary>
    /// <returns>
    /// The can remove group completely.
    /// </returns>
    protected bool CanRemoveGroupCompletely()
    {
      return this.ShowButtons && this.PageContext.IsAdmin;
    }

    /// <summary>
    /// Checks if a user can delete group from all places, but not completely
    /// </summary>
    /// <returns>
    /// The can remove group everywhere.
    /// </returns>
    protected bool CanRemoveGroupEverywhere()
    {
      return this.ShowButtons && this.PageContext.IsAdmin;
    }

    /// <summary>
    /// Checks if a user can delete poll without deleting it from database
    /// </summary>
    /// <param name="pollId">
    /// </param>
    /// <returns>
    /// The can remove poll.
    /// </returns>
    protected bool CanRemovePoll([NotNull] object pollId)
    {
      return false;

      /*return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroupAllChoices.Rows[0]["GroupUserID"]))); */
    }

    /// <summary>
    /// Checks if a user can delete poll completely from database.
    /// </summary>
    /// <param name="pollId">
    /// </param>
    /// <returns>
    /// The can remove poll completely.
    /// </returns>
    protected bool CanRemovePollCompletely([NotNull] object pollId)
    {
        if (!this.Get<YafBoardSettings>().AllowPollChangesAfterFirstVote)
      {
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsModerator ||
                (this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>() &&
                 this.PollHasNoVotes(pollId)));
      }

      return this.PollHasNoVotes(pollId) && this.ShowButtons &&
             (this.PageContext.IsAdmin ||
              this.PageContext.PageUserID == this._dtPollGroupAllChoices.Rows[0]["GroupUserID"].ToType<int>());
    }

    /// <summary>
    /// The days to run.
    /// </summary>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    /// <param name="soon">
    /// The soon.
    /// </param>
    /// <returns>
    /// The days to run.
    /// </returns>
    protected int? DaysToRun([NotNull] object pollId, out bool soon)
    {
      soon = false;

      foreach (TimeSpan ts in from DataRow dr in this._dtPollGroupAllChoices.Rows
                              where dr["Closes"] != DBNull.Value && pollId.ToType<int>() == dr["PollID"].ToType<int>()
                              select Convert.ToDateTime(dr["Closes"]) - DateTime.UtcNow)
      {
        if (ts.TotalDays >= 1)
        {
          return ts.TotalDays.ToType<int>();
        }

        if (ts.TotalSeconds > 0)
        {
          soon = true;
          return 1;
        }

        return 0;
      }

      return null;
    }

    /// <summary>
    /// The get image height.
    /// </summary>
    /// <param name="mimeType">
    /// The mime type.
    /// </param>
    /// <returns>
    /// The get image height.
    /// </returns>
    protected int GetImageHeight([NotNull] object mimeType)
    {
      string[] attrs = mimeType.ToString().Split('!')[1].Split(';');

      return attrs[1].ToType<int>();
    }

    /// <summary>
    /// The get poll is closed.
    /// </summary>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    /// <returns>
    /// The get poll is closed.
    /// </returns>
    protected string GetPollIsClosed([NotNull] object pollId)
    {
      string strPollClosed = string.Empty;
      if (this.IsPollClosed(pollId))
      {
        strPollClosed = this.GetText("POLL_CLOSED");
      }

      return strPollClosed;
    }

    /// <summary>
    /// The get poll question.
    /// </summary>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    /// <returns>
    /// The get poll question.
    /// </returns>
    protected string GetPollQuestion([NotNull] object pollId)
    {
      foreach (DataRow dr in
        this._dtPollGroupAllChoices.Rows.Cast<DataRow>().Where(dr => pollId.ToType<int>() == dr["PollID"].ToType<int>()))
      {
        return this.HtmlEncode(this.Get<IBadWordReplace>().Replace(dr["Question"].ToString()));
      }

      return string.Empty;
    }

    /// <summary>
    /// Get Theme Contents
    /// </summary>
    /// <param name="page">
    /// The Page
    /// </param>
    /// <param name="tag">
    /// Tag
    /// </param>
    /// <returns>
    /// Content
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
    /// The get total.
    /// </returns>
    protected string GetTotal([NotNull] object pollId)
    {
      foreach (DataRow dr in
        this._dtPollGroupAllChoices.Rows.Cast<DataRow>().Where(dr => pollId.ToType<int>() == dr["PollID"].ToType<int>()))
      {
        return this.HtmlEncode(dr["Total"].ToString());
      }

      return string.Empty;
    }

    /// <summary>
    /// The is not voted.
    /// </summary>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    /// <param name="allowManyChoices">
    /// The allow Many Choices.
    /// </param>
    /// <param name="pollChoices">
    /// The poll Choices.
    /// </param>
    /// <param name="hasVote">
    /// The has Vote.
    /// </param>
    /// <returns>
    /// The is not voted.
    /// </returns>
    protected bool IsNotVoted(int pollId, bool allowManyChoices, [NotNull] out int?[] pollChoices, out bool hasVote)
    {
      pollChoices = null;

      // check for voting cookie
      HttpCookie httpCookie = this.Request.Cookies[this.VotingCookieName(pollId.ToType<int>())];
      if (httpCookie != null && httpCookie.Value != null)
      {
        int pchcntr1 = 0;
        int choicescount = 0;
        string[] choiceArray = httpCookie.Value.Split(',');
        if (choiceArray.GetLength(0) > 0)
        {
          pollChoices = new int?[choiceArray.GetLength(0)];
          foreach (DataRow drch in
            this._dtPollAllChoices.Rows.Cast<DataRow>().Where(drch => (int)drch["PollID"] == pollId))
          {
            for (int j = 0; j < choiceArray.GetLength(0); j++)
            {
              int pollChoicesFromCookie;

              if (!int.TryParse(choiceArray[j], out pollChoicesFromCookie))
              {
                continue;
              }

              if (pollChoicesFromCookie != (int)drch["ChoiceID"])
              {
                continue;
              }

              pollChoices[pchcntr1] = pollChoicesFromCookie;
              pchcntr1++;
            }

            choicescount++;
          }
        }

        if (pchcntr1 > 0)
        {
          hasVote = true;
          if (allowManyChoices)
          {
            if (pchcntr1 < choicescount)
            {
              return true;
            }
          }

          return false;
        }
      }

      // voting is not tied to IP and they are a guest...
      if (this.PageContext.IsGuest && !this.Get<YafBoardSettings>().PollVoteTiedToIP)
      {
        hasVote = false;
        return true;
      }

      // Check if a user already voted
      int pchcntr = this._dtAllPollGroupVotes.Rows.Cast<DataRow>().Count(drch => (int)drch["PollID"] == pollId);

      if (pchcntr == 0)
      {
        hasVote = false;
        return true;
      }

      pollChoices = new int?[pchcntr];
      int choicescount1 = 0;
      pchcntr = 0;

      // tha_watcha: Added some IsNullOrEmptyDBField checks otherwise it would throw an System.InvalidCastException: Specified cast is not valid, on older polls.
      foreach (DataRow drch in from DataRow drch in this._dtPollAllChoices.Rows
                               where !drch["PollID"].IsNullOrEmptyDBField()
                               where (int)drch["PollID"] == pollId
                               select drch)
      {
        DataRow drch2 = drch;
        DataRow drch3 = drch;

        foreach (DataRow drch1 in from DataRow drch1 in this._dtAllPollGroupVotes.Rows
                                  where
                                    !drch2["ChoiceID"].IsNullOrEmptyDBField() &&
                                    !drch1["ChoiceID"].IsNullOrEmptyDBField()
                                  where (int)drch3["ChoiceID"] == (int)drch1["ChoiceID"]
                                  select drch1)
        {
          pollChoices[pchcntr] = (int)drch["ChoiceID"];

          pchcntr++;
        }

        choicescount1++;
      }

      if (pchcntr > 0)
      {
        hasVote = true;
        if (allowManyChoices)
        {
          if (pchcntr < choicescount1)
          {
            return true;
          }
        }

        return false;
      }

      hasVote = false;
      return true;
    }

    /// <summary>
    /// The is poll closed.
    /// </summary>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    /// <returns>
    /// The is poll closed.
    /// </returns>
    protected bool IsPollClosed([NotNull] object pollId)
    {
      bool soon;
      int? dtr = this.DaysToRun(pollId, out soon);
      return dtr == 0;
    }

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender">
    /// The object sender.
    /// </param>
    /// <param name="e">
    /// The EventArgs e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// PollGroup_ItemCommand
    /// </summary>
    /// <param name="source">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void PollGroup_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "new" && this.PageContext.ForumVoteAccess)
      {
        YafBuildLink.Redirect(ForumPages.polledit, "{0}", this.ParamsToSend());
      }

      if (e.CommandName == "edit" && this.PageContext.ForumVoteAccess)
      {
        YafBuildLink.Redirect(ForumPages.polledit, "{0}&p={1}", this.ParamsToSend(), e.CommandArgument.ToString());
      }

      if (e.CommandName == "remove" && this.PageContext.ForumVoteAccess)
      {
        // ChangePollShowStatus(false);
        if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
        {
          LegacyDb.poll_remove(this.PollGroupId, e.CommandArgument, this.BoardId, false, false);
          this.ReturnToPage();

          // BindData();
        }
      }

      if (e.CommandName == "removeall" && this.PageContext.ForumVoteAccess)
      {
        if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
        {
          LegacyDb.poll_remove(this.PollGroupId, e.CommandArgument, this.BoardId, true, false);
          this.ReturnToPage();

          // BindData();
        }
      }

      if (e.CommandName == "removegroup" && this.PageContext.ForumVoteAccess)
      {
        LegacyDb.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, false, false);
        this.ReturnToPage();

        // BindData();
      }

      if (e.CommandName == "removegroupall" && this.PageContext.ForumVoteAccess)
      {
        LegacyDb.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, true, false);
        this.ReturnToPage();

        // BindData();
      }

      if (e.CommandName != "removegroupevery" || !this.PageContext.ForumVoteAccess)
      {
        return;
      }
      
      this.ReturnToPage();

      // BindData();
    }

    /// <summary>
    /// The PollGroup item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PollGroup_OnItemDataBound([NotNull] object source, [NotNull] RepeaterItemEventArgs e)
    {
      RepeaterItem item = e.Item;
      var drowv = (DataRowView)e.Item.DataItem;
      if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
      {
        // clear the value after choiced are bounded
        // the parameter is used to get correct aspect ratio if the poll is with images 
        this.MaxImageAspect = 1;

        // We don't display poll command row to everyone 
        item.FindControlRecursiveAs<HtmlTableRow>("PollCommandRow").Visible = this.HasOwnerExistingGroupAccess() &&
                                                                              this.ShowButtons;

        // Binding question image
        this.BindPollQuestionImage(item, drowv);

        var pollChoiceList = item.FindControlRecursiveAs<PollChoiceList>("PollChoiceList1");

        var pollId = drowv.Row["PollID"].ToType<int>();
        int?[] choicePId;
        bool hasVote;
        bool isNotVoted = this.IsNotVoted(
          pollId, drowv.Row["AllowMultipleChoices"].ToType<bool>(), out choicePId, out hasVote);
        bool cvote = this.HasVoteAccess(pollId) ? isNotVoted : false;
        pollChoiceList.ChoiceId = choicePId;

        // If guest are not allowed to view options we don't render them
        pollChoiceList.Visible = !cvote && !this.PageContext.BoardSettings.AllowGuestsViewPollOptions &&
                                 this.PageContext.IsGuest
                                   ? false
                                   : true;

        bool isClosedBound = false;
        bool allowsMultipleChoices = false;
        bool showVoters = false;

        // This is not a guest w/o poll option view permissions, we bind the control.
        if (pollChoiceList.Visible)
        {
          DataTable thisPollTable = this._dtPollAllChoices.Copy();
          foreach (DataRow thisPollTableRow in thisPollTable.Rows)
          {
            if (thisPollTableRow["PollID"].ToType<int>() != pollId.ToType<int>())
            {
              thisPollTableRow.Delete();
            }
            else
            {
              // in this section we get a custom image aspect, the option is argueable
              if (!thisPollTableRow["MimeType"].IsNullOrEmptyDBField())
              {
                decimal currentAspect = GetImageAspect(thisPollTableRow["MimeType"]);
                if (currentAspect > this.MaxImageAspect)
                {
                  this.MaxImageAspect = currentAspect;
                }
              }
            }
          }

          thisPollTable.AcceptChanges();
          pollChoiceList.DataSource = thisPollTable;
          isClosedBound = Convert.ToBoolean(thisPollTable.Rows[0]["IsClosedBound"]);
          allowsMultipleChoices = Convert.ToBoolean(thisPollTable.Rows[0]["AllowMultipleChoices"]);
          showVoters = Convert.ToBoolean(thisPollTable.Rows[0]["ShowVoters"]);
        }

        string voters = string.Empty;

        if (showVoters)
        {
            DataTable dtAllPollGroupVotesShow = LegacyDb.pollgroup_votecheck(drowv.Row["PollGroupID"].ToType<int>(), null, null);
            dtAllPollGroupVotesShow.Rows.Cast<DataRow>().Count(drch => (int)drch["PollID"] == pollId);
            pollChoiceList.Voters = dtAllPollGroupVotesShow;
        }

        pollChoiceList.PollId = pollId.ToType<int>();
        pollChoiceList.MaxImageAspect = this.MaxImageAspect;

       
        // returns number of day to run - null if poll has no expiration date 
        bool soon;
        int? daystorun = this.DaysToRun(pollId, out soon);
        bool isPollClosed = this.IsPollClosed(pollId);

        // *************************
        // Show|hide results section
        // *************************
        // this._canVote = this.HasVoteAccess(pollId) && isNotVoted;

        // Poll voting is bounded - you can't see results before voting in each poll
        int hasVoteEmptyCounter = 0;
        int cnt = 0;

        // compare a number of voted polls with number of polls
        foreach (DataRow dr in this._dtPollGroupAllChoices.Rows)
        {
          bool hasVoteEmpty;

          bool voted =
            !this.IsNotVoted(
              (int)dr["PollID"], dr["AllowMultipleChoices"].ToType<bool>(), out choicePId, out hasVoteEmpty);
          if (hasVoteEmpty)
          {
            hasVoteEmptyCounter++;
          }

          bool isclosedbound = !dr["Closes"].IsNullOrEmptyDBField() && Convert.ToBoolean(dr["IsClosedBound"])
                                 ? dr["Closes"].ToType<DateTime>() < DateTime.UtcNow
                                 : false;

          if (voted || isclosedbound)
          {
            cnt++;
          }
        }

        bool warningMultiplePolls = hasVoteEmptyCounter >= this._dtPollGroupAllChoices.Rows.Count &&
                                    hasVoteEmptyCounter > 0;
        if (this._isBound)
        {
          // If user is voted in all polls or the poll allows multiple votes and he's voted for 1 choice
          if ((cnt >= this.PollNumber) || warningMultiplePolls)
          {
            if (isClosedBound)
            {
              if (isPollClosed)
              {
                this._showResults = true;
              }
            }
            else
            {
              this._showResults = true;
            }
          }
        }
        else if (!this._isBound && isClosedBound && isPollClosed)
        {
          this._showResults = true;
        }

        if (!isClosedBound && !this._isBound)
        {
          this._showResults = true;
        }

        // The poll had an expiration date and expired without voting 
        // show results anyway if poll has no expiration date daystorun is null  
        if (daystorun == 0)
        {
          this._showResults = true;
        }

        pollChoiceList.HideResults = !this._showResults;

        // Clear the fields after the child repeater is bound
        this._showResults = false;

        // this._canVote = false;

        // Add confirmations to delete buttons
        this.AddPollButtonConfirmations(item, pollId);

        // *************************
        // Poll warnings section
        // *************************
        string notificationString = string.Empty;

        // Here warning labels are treated
        if (cvote)
        {
          if (this._isBound && this.PollNumber > 1 && hasVoteEmptyCounter < this._dtPollGroupAllChoices.Rows.Count)
          {
            notificationString += this.GetText("POLLEDIT", "POLLGROUP_BOUNDWARN");
          }
        }

        if (cvote && hasVoteEmptyCounter > 0 && allowsMultipleChoices)
        {
          notificationString +=
            " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_MULTIPLECHOICES_INFO"));
        }

        if (this.PageContext.IsGuest)
        {
          if (!cvote)
          {
              if (!this.Get<YafBoardSettings>().AllowGuestsViewPollOptions)
            {
              notificationString +=
                " {0}".FormatWith(this.GetText("POLLEDIT", "POLLOPTIONSHIDDEN_GUEST"));
            }

            if (isNotVoted)
            {
              notificationString +=
                " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_NOPERM_GUEST"));
            }
          }
        }

        if (!isNotVoted &&
            (this.PageContext.ForumVoteAccess ||
             (this.PageContext.BoardVoteAccess && (this.BoardId > 0 || this.EditBoardId > 0))))
        {
          notificationString += " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_VOTED"));
        }

        // Poll has expiration date
        if (daystorun > 0)
        {
          var pollClosedImage = item.FindControlRecursiveAs<HtmlImage>("PollClosedImage");
          pollClosedImage.Src = this.GetThemeContents("VOTE", "POLL_NOTCLOSED");
          if (!soon)
          {
            notificationString +=
              " {0}".FormatWith(this.GetTextFormatted("POLL_WILLEXPIRE", daystorun));
            pollClosedImage.Alt = this.GetTextFormatted("POLL_WILLEXPIRE", daystorun);
          }
          else
          {
            notificationString +=
              " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS"));
            pollClosedImage.Alt = this.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS");
          }

          if (isClosedBound)
          {
            notificationString +=
              " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_CLOSEDBOUND"));
          }

          pollClosedImage.Attributes["title"] = pollClosedImage.Alt;
          pollClosedImage.Visible = true;
        }
        else if (daystorun == 0)
        {
          notificationString += " {0}".FormatWith(this.GetText("POLLEDIT", "POLL_EXPIRED"));

          var pollClosedImage = item.FindControlRecursiveAs<HtmlImage>("PollClosedImage");
          pollClosedImage.Src = this.GetThemeContents("VOTE", "POLL_CLOSED");
          pollClosedImage.Alt = this.GetText("POLLEDIT", "POLL_CLOSED");
          pollClosedImage.Attributes["title"] = pollClosedImage.Alt;
          pollClosedImage.Visible = true;
        }

        pollChoiceList.CanVote = cvote;
        pollChoiceList.DaysToRun = daystorun;

        // we should double bind if it is not a vote event bubble. 
        if (this._isVoteEvent)
        {
          pollChoiceList.DataBind();
        }

        pollChoiceList.ChoiceVoted += this.VoteBubbleEvent;

        // we don't display warnings row if no info
        if (notificationString.IsSet())
        {
          item.FindControlRecursiveAs<HtmlTableRow>("PollInfoTr").Visible = true;
          var pn = item.FindControlRecursiveAs<Label>("PollNotification");
          pn.Text = notificationString;
          pn.Visible = true;
        }
      }

      // Populate PollGroup Repeater footer
      if (item.ItemType != ListItemType.Footer)
      {
        return;
      }

      if (this._groupNotificationString.IsSet())
      {
        // we don't display warnings row if no info
        item.FindControlRecursiveAs<HtmlTableRow>("PollInfoTr").Visible = true;
        var pgn = item.FindControlRecursiveAs<Label>("PollGroupNotification");
        pgn.Text = this._groupNotificationString;
        pgn.Visible = true;
      }

      this.AddPollGroupButtonConfirmations(item);
    }

    /// <summary>
    /// The remove poll_ completely load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
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
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RemovePoll_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLL_DELETE"));
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
      this._isVoteEvent = true;
      this.LoadData();
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
    protected string VotingCookieName(int pollId)
    {
      return "poll#{0}".FormatWith(pollId);
    }

    /// <summary>
    /// Returns an image width|height ratio.
    /// </summary>
    /// <param name="mimeType">
    /// </param>
    /// <returns>
    /// The get image aspect.
    /// </returns>
    private static decimal GetImageAspect([NotNull] object mimeType)
    {
      if (!mimeType.IsNullOrEmptyDBField())
      {
        string[] attrs = mimeType.ToString().Split('!')[1].Split(';');
        var width = attrs[0].ToType<decimal>();
        return width / attrs[1].ToType<decimal>();
      }

      return 1;
    }

    /// <summary>
    /// The methods add poll manipulatation confirmation pop-ups to button actions.
    /// </summary>
    /// <param name="ri">
    /// The RepeaterItem ri.
    /// </param>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    private void AddPollButtonConfirmations([NotNull] RepeaterItem ri, int pollId)
    {
      // Add confirmations to delete buttons
      var removePollAll = ri.FindControlRecursiveAs<ThemeButton>("RemovePollAll");
      removePollAll.Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
      removePollAll.Visible = this.CanRemovePollCompletely(pollId);

      var removePoll = ri.FindControlRecursiveAs<ThemeButton>("RemovePoll");
      removePoll.Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLL_DELETE"));
      removePoll.Visible = this.CanRemovePoll(pollId);
    }

    /// <summary>
    /// The methods add poll group manipulatation confirmation pop-ups to button actions.
    /// </summary>
    /// <param name="ri">
    /// The RepeaterItem ri.
    /// </param>
    private void AddPollGroupButtonConfirmations([NotNull] RepeaterItem ri)
    {
      var pgcr = ri.FindControlRecursiveAs<HtmlTableRow>("PollGroupCommandRow");
      pgcr.Visible = this.HasOwnerExistingGroupAccess() && this.ShowButtons;

      // return confirmations for poll group 
      if (!pgcr.Visible)
      {
        return;
      }

      ri.FindControlRecursiveAs<ThemeButton>("RemoveGroup").Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.GetText("POLLEDIT", "ASK_POLLGROUP_DETACH"));

      ri.FindControlRecursiveAs<ThemeButton>("RemoveGroupAll").Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(
          this.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_ALL"));

      ri.FindControlRecursiveAs<ThemeButton>("RemoveGroupEverywhere").Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(
          this.GetText("POLLEDIT", "ASK_POLLROUP_DETACH_EVR"));
    }

    /// <summary>
    /// The bind create new poll row.
    /// </summary>
    private void BindCreateNewPollRow()
    {
      var cpr = this.CreatePoll1;

      // ChangePollShowStatus(true);
      cpr.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.polledit, "{0}", this.ParamsToSend());
      cpr.DataBind();
      cpr.Visible = true;
      this.NewPollRow.Visible = true;
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.PollNumber = 0;
      this._dtPollAllChoices = LegacyDb.pollgroup_stats(this.PollGroupId);

      // Here should be a poll group, remove the value to avoid exception if a deletion was not safe. 
      if (!(this._dtPollAllChoices.Rows.Count > 0))
      {
        LegacyDb.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, true, true);          
        return;
      }

      // if the page user can cheange the poll. Only a group owner, forum moderator  or an admin can do it.   );
      this._canChange = (this._dtPollAllChoices.Rows[0]["GroupUserID"].ToType<int>() == this.PageContext.PageUserID) ||
                        this.PageContext.IsAdmin || this.PageContext.IsForumModerator;

      // check if we should hide pollgroup repeater when a message is posted
      if (this.PageContext.ForumPageType == ForumPages.postmessage)
      {
        this.PollGroup.Visible = ((this.EditMessageId > 0) && (!this._canChange)) || this._canChange;
      }
      else
      {
        this.PollGroup.Visible = true;
      }

      this._dtPollGroupAllChoices = this._dtPollAllChoices.Copy();

      // TODO: repeating code - move to Utils
      // Remove repeating PollID values   
      var hTable = new Hashtable();
      var duplicateList = new ArrayList();

      foreach (DataRow drow in this._dtPollGroupAllChoices.Rows)
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
        this._dtPollGroupAllChoices.Rows.Remove(dRow);
      }

      this._dtPollGroupAllChoices.AcceptChanges();

      // frequently used
      this.PollNumber = this._dtPollGroupAllChoices.Rows.Count;

      if (this._dtPollGroupAllChoices.Rows.Count > 0)
      {
        // Check if the user is already voted in polls in the group 
        object userId = null;
        object remoteIp = null;

        if (this.Get<YafBoardSettings>().PollVoteTiedToIP)
        {
          remoteIp = IPHelper.IPStrToLong(this.Request.UserHostAddress).ToString();
        }

        if (!this.PageContext.IsGuest)
        {
          userId = this.PageContext.PageUserID;
        }
         
        this._dtAllPollGroupVotes = LegacyDb.pollgroup_votecheck(_dtPollGroupAllChoices.Rows[0]["PollGroupID"].ToType<int>(), userId, remoteIp);

        this._isBound = this._dtPollGroupAllChoices.Rows[0]["IsBound"].ToType<bool>();

        this.PollGroup.DataSource = this._dtPollGroupAllChoices;

        // we hide new poll row if a poll exist
        this.NewPollRow.Visible = false;
      }

      this.DataBind();
    }

    /// <summary>
    /// Adds custom or standard image to poll question.
    /// </summary>
    /// <param name="item">
    /// The RepeaterItem item.
    /// </param>
    /// <param name="drowv">
    /// The DataRowView drowv.
    /// </param>
    private void BindPollQuestionImage([NotNull] RepeaterItem item, [NotNull] DataRowView drowv)
    {
      var questionImage = item.FindControlRecursiveAs<HtmlImage>("QuestionImage");
      var questionAnchor = item.FindControlRecursiveAs<HtmlAnchor>("QuestionAnchor");

      // The image is not from theme
      if (!drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField())
      {
        // questionAnchor.Attributes["rel"] = "lightbox-group" + Guid.NewGuid().ToString().Substring(0, 5);
        questionAnchor.HRef = drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField()
                                ? this.GetThemeContents("VOTE", "POLL_CHOICE")
                                : this.HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());

        questionImage.Src = this.HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());

        if (!drowv.Row["QuestionMimeType"].IsNullOrEmptyDBField())
        {
          decimal aspect = GetImageAspect(drowv.Row["QuestionMimeType"]);

          questionImage.Width = 80;

          if (this.Get<IYafSession>().UseMobileTheme ?? false)
          {
            questionImage.Width = 40;
          }

          questionImage.Height = (questionImage.Width / aspect).ToType<int>();
        }
      }
      else
      {
        // image from theme no need to resize it
        questionImage.Alt = this.GetText("POLLEDIT", "POLL_PLEASEVOTE");
        questionImage.Src = this.GetThemeContents("VOTE", "POLL_QUESTION");
        questionAnchor.HRef = string.Empty;
      }
    }

    /// <summary>
    /// The has owner existing group access.
    /// </summary>
    /// <returns>
    /// The has owner existing group access.
    /// </returns>
    private bool HasOwnerExistingGroupAccess()
    {
        if (this.Get<YafBoardSettings>().AllowedPollChoiceNumber > 0)
      {
        // if topicid > 0 it can be every member
        if (this.TopicId > 0)
        {
          return (this._topicUser == this.PageContext.PageUserID) || this.PageContext.IsAdmin ||
                 this.PageContext.IsForumModerator;
        }

        // only admins can edit this
        if (this.CategoryId > 0 || this.BoardId > 0)
        {
          return this.PageContext.IsAdmin;
        }

        // in other places only admins and forum moderators can have access
        return this.PageContext.IsAdmin || this.PageContext.IsForumModerator;
      }

      return false;
    }

    /// <summary>
    /// The method to return if a user already voted
    /// </summary>
    /// <param name="pollId">
    /// The pollId value.
    /// </param>
    /// <returns>
    /// Return True if a user has voted.
    /// </returns>
    private bool HasVoteAccess([NotNull] object pollId)
    {
      // rule out users without voting rights
      // If you come here from topics or edit TopicId should be > 0
      if (!this.PageContext.ForumVoteAccess && this.TopicId > 0)
      {
        return false;
      }

      if (!this.PageContext.BoardVoteAccess && this.BoardId > 0)
      {
        return false;
      }

      return !this.IsPollClosed(pollId);
    }

    /// <summary>
    /// Handles page controls after an event.
    /// </summary>
    private void LoadData()
    {
        // Only if this control is in a topic we find the topic creator
        if (this.TopicId > 0)
        {
            DataRow dti = LegacyDb.topic_info(this.TopicId);
            this._topicUser = dti["UserID"].ToType<int>();
            if (!dti["PollID"].IsNullOrEmptyDBField())
            {
                this.PollGroupId = dti["PollID"].ToType<int>();
            }
        }

        // We check here various variants if a poll exists, as we don't know from which place comes the call
        bool existingPoll = (this.PollGroupId > 0) && ((this.TopicId > 0) || (this.ForumId > 0) || (this.BoardId > 0));

        // Here we'll find whether we should display create new poll button only 
        bool topicPoll = this.PageContext.ForumPollAccess &&
                         (this.EditMessageId > 0 || (this.TopicId > 0 && this.ShowButtons));
        bool forumPoll = this.EditForumId > 0 || (this.ForumId > 0 && this.ShowButtons);
        bool categoryPoll = this.EditCategoryId > 0 || (this.CategoryId > 0 && this.ShowButtons);
        bool boardPoll = this.PageContext.BoardVoteAccess &&
                         (this.EditBoardId > 0 || (this.BoardId > 0 && this.ShowButtons));

        this.NewPollRow.Visible = this.ShowButtons && (topicPoll || forumPoll || categoryPoll || boardPoll) &&
                                  this.HasOwnerExistingGroupAccess() && (!existingPoll);

        // if this is > 0 then we already have a poll and will display all buttons
        if (this.PollGroupId > 0)
        {
            this.BindData();

            if (!this._canChange && this.PageContext.ForumPageType == ForumPages.postmessage)
            {
                this.PollListHolder.Visible = false;
            }
        }
        else
        {
            if (this.NewPollRow.Visible)
            {
                this.BindCreateNewPollRow();
            }
            else
            {
                this.PollListHolder.Visible = false;
            }
        }
    }

      /// <summary>
    /// A method to return parameters string. 
    ///   Consrtucts parameters string to send to other forms.
    /// </summary>
    /// <returns>
    /// The params to send.
    /// </returns>
    private string ParamsToSend()
    {
      string sb = String.Empty;

      if (this.TopicId > 0)
      {
        sb += "t={0}".FormatWith(this.TopicId);
      }

      if (this.EditMessageId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "m={0}".FormatWith(this.EditMessageId);
      }

      if (this.ForumId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "f={0}".FormatWith(this.ForumId);
      }

      if (this.EditForumId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "ef={0}".FormatWith(this.EditForumId);
      }

      if (this.CategoryId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "c={0}".FormatWith(this.CategoryId);
      }

      if (this.EditCategoryId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "ec={0}".FormatWith(this.EditCategoryId);
      }

      if (this.BoardId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "b={0}".FormatWith(this.BoardId);
      }

      if (this.EditBoardId > 0)
      {
        if (sb.IsSet())
        {
          sb += '&';
        }

        sb += "eb={0}".FormatWith(this.EditBoardId);
      }

      return sb;
    }

    /// <summary>
    /// Checks if a poll has no votes.
    /// </summary>
    /// <param name="pollId">
    /// </param>
    /// <returns>
    /// The poll has no votes.
    /// </returns>
    private bool PollHasNoVotes([NotNull] object pollId)
    {
      return
        this._dtPollAllChoices.Rows.Cast<DataRow>().Where(dr => dr["PollID"].ToType<int>() == pollId.ToType<int>()).All(
          dr => dr["Votes"].ToType<int>() <= 0);
    }

    /// <summary>
    /// Returns user to the original call page.
    /// </summary>
    private void ReturnToPage()
    {
      // We simply return here to the page where the control is put. It can be made other way.
      switch (this.PageContext.ForumPageType)
      {
        case ForumPages.posts:
          if (this.TopicId > 0)
          {
            YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.TopicId);
          }

          break;
        case ForumPages.forum:

          // This is a poll on the coard main page
          if (this.BoardId > 0)
          {
            YafBuildLink.Redirect(ForumPages.forum);
          }

          // This is a poll in a category list 
          if (this.CategoryId > 0)
          {
            YafBuildLink.Redirect(ForumPages.forum, "c={0}", this.CategoryId);
          }

          break;
        case ForumPages.topics:

          // this is a poll in forums topic view
          if (this.ForumId > 0)
          {
            YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.ForumId);
          }

          break;
        case ForumPages.postmessage:
          if (this.EditMessageId > 0)
          {
            YafBuildLink.Redirect(ForumPages.postmessage, "m={0}", this.EditMessageId);
          }

          break;
        case ForumPages.admin_editforum:

          // This is a poll on edit forum page
          if (this.EditForumId > 0)
          {
            YafBuildLink.Redirect(ForumPages.admin_editforum, "f={0}", this.ForumId);
          }

          break;
        case ForumPages.admin_editcategory:

          // this is a poll on edit category page
          if (this.EditCategoryId > 0)
          {
            YafBuildLink.Redirect(ForumPages.admin_editcategory, "c={0}", this.EditCategoryId);
          }

          break;
        case ForumPages.admin_editboard:

          // this is a poll on edit board page
          if (this.EditBoardId > 0)
          {
            YafBuildLink.Redirect(ForumPages.admin_editboard, "b={0}", this.EditBoardId);
          }

          break;
        default:
          YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
          break;
      }
    }

    #endregion
  }
}