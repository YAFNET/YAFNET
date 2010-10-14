/* Yet Another Forum.net
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.controls
{
  // YAF.Pages
  #region Using

  using System;
  using System.Collections;
  using System.Data;
  using System.Linq;
  using System.Text;
  using System.Web;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// PollList Class
  /// </summary>
  public partial class PollList : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _canChange.
    /// </summary>
    private bool _canChange;

    /// <summary>
    ///   The _canVote. Used to store data from parent repeater.
    /// </summary>
    // private bool _canVote;

    /// <summary>
    ///   The _dt poll.
    /// </summary>
    private DataTable _dtPoll;

    /// <summary>
    ///   The _dt PollGroup.
    /// </summary>
    private DataTable _dtPollGroup;

    /// <summary>
    ///   The _dt Votes.
    /// </summary>
    private DataTable _dtVotes;

    /// <summary>
    ///   The _showResults. Used to store data from parent repeater.
    /// </summary>
    private bool _showResults;

    /// <summary>
    ///   The isBound.
    /// </summary>
    private bool isBound;

    /// <summary>
    ///   The topic User.
    /// </summary>
    private int? topicUser;

    /// <summary>
    /// The group Notification String.
    ///  </summary>
    string _groupNotificationString = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets BoardId
    /// </summary>
    public int BoardId { get; set; }

    /// <summary>
    /// Gets or sets CategoryId
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets EditBoardId.
    /// Used to return to edit board page.
    /// Currently is not implemented.
    /// </summary>
    public int EditBoardId { get; set; }

    /// <summary>
    /// Gets or sets EditCategoryId
    /// </summary>
    public int EditCategoryId { get; set; }

    /// <summary>
    /// Gets or sets EditForumId
    /// </summary>
    public int EditForumId { get; set; }

    /// <summary>
    /// Gets or sets EditMessageId.
    /// </summary>
    public int EditMessageId { get; set; }

    /// <summary>
    /// Gets or sets ForumId
    /// </summary>
    public int ForumId { get; set; }

    /// <summary>
    /// Gets or sets IsLocked
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Gets or sets MaxImageAspect. Stores max aspect to get rows of equal height.
    /// </summary>
    public decimal MaxImageAspect { get; set; }

    /// <summary>
    /// Gets or sets PollGroupID
    /// </summary>
    public int? PollGroupId { get; set; }

    /// <summary>
    /// Gets or sets PollNumber
    /// </summary>
    public int PollNumber { get; set; }

    /// <summary>
    /// Gets or sets if we are editing a post
    /// </summary>
    public bool PostEdit { get; set; }

    /// <summary>
    /// Gets or sets ShowButtons
    /// </summary>
    public bool ShowButtons { get; set; }

    /// <summary>
    /// Gets or sets TopicId
    /// </summary>
    public int TopicId { get; set; }

    #endregion

    #region Protected Methods

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
    protected string GetThemeContents(string page, string tag)
    {
      return this.PageContext.Theme.GetItem(page, tag);
    }

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
      return (this.PollNumber < this.PageContext.BoardSettings.AllowedPollNumber) &&
             (this.PageContext.BoardSettings.AllowedPollChoiceNumber > 0) && this.HasOwnerExistingGroupAccess() &&
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
    protected bool CanEditPoll(object pollId)
    {
      // The host admin can forbid a user to change poll after first vote to avoid fakes.    
      if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
      {
        // Only if show buttons are enabled user can edit poll 
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]) &&
                (this.PollHasNoVotes(pollId) && (!this.IsPollClosed(pollId)))));
      }

      // we don't call PollHasNoVotes method here
      return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]) &&
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

      foreach (DataRow dr in this._dtPoll.Rows.Cast<DataRow>().Where(dr => Convert.ToInt32(dr["Votes"]) > 0))
      {
          hasNoVotes = false;
      }

      if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
      {
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
                (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]) && hasNoVotes));
      }

      return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"])));
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
    protected bool CanRemovePoll(object pollId)
    {
      return this.ShowButtons &&
             (this.PageContext.IsAdmin || this.PageContext.IsForumModerator ||
              (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"])));
    }

    /// <summary>
    /// Checks if a user can delete poll completely from database.
    /// </summary>
    /// <param name="pollId">
    /// </param>
    /// <returns>
    /// The can remove poll completely.
    /// </returns>
    protected bool CanRemovePollCompletely(object pollId)
    {
      if (!this.PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
      {
        return this.ShowButtons &&
               (this.PageContext.IsAdmin || this.PageContext.IsModerator ||
                (this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]) &&
                  this.PollHasNoVotes(pollId)));
      }

      return this.PollHasNoVotes(pollId) && this.ShowButtons &&
             (this.PageContext.IsAdmin ||
              this.PageContext.PageUserID == Convert.ToInt32(this._dtPollGroup.Rows[0]["GroupUserID"]));
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
    protected int? DaysToRun(object pollId, out bool soon)
    {
      soon = false;
      foreach (DataRow dr in this._dtPollGroup.Rows)
      {
        if (dr["Closes"] != DBNull.Value && Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
        {
            TimeSpan ts = (Convert.ToDateTime(dr["Closes"]) - DateTime.UtcNow);
            if (ts.TotalDays >= 1)
            {
                return Convert.ToInt32(ts.TotalDays);
            }

            if (ts.TotalSeconds > 0)
            {
                soon = true;
                return 1;
            }
          DateTime tCloses = Convert.ToDateTime(dr["Closes"]).Date;
          return 0;
        }
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
    protected int GetImageHeight(object mimeType)
    {
      string[] attrs = mimeType.ToString().Split('!')[1].Split(';');
      return Convert.ToInt32(attrs[1]);
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
    protected string GetPollIsClosed(object pollId)
    {
      string strPollClosed = string.Empty;
      if (this.IsPollClosed(pollId))
      {
        strPollClosed = this.PageContext.Localization.GetText("POLL_CLOSED");
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
    protected string GetPollQuestion(object pollId)
    {
      foreach (DataRow dr in this._dtPollGroup.Rows)
      {
        if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
        {
          return this.HtmlEncode(this.Get<YafBadWordReplace>().Replace(dr["Question"].ToString()));
        }
      }

      return string.Empty;
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
    protected string GetTotal(object pollId)
    {
      foreach (DataRow dr in this._dtPollGroup.Rows)
      {
        if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
        {
          return this.HtmlEncode(dr["Total"].ToString());
        }
      }

      return string.Empty;
    }

    /// <summary>
    /// The is not voted.
    /// </summary>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    /// <returns>
    /// The is not voted.
    /// </returns>
    protected bool IsNotVoted(object pollId, out int choiceId)
    {
     
      // check for voting cookie
      HttpCookie httpCookie = this.Request.Cookies[this.VotingCookieName(Convert.ToInt32(pollId))];
      if (httpCookie != null && int.TryParse(httpCookie.Value, out choiceId))
      {
        return false;
      }
      choiceId = 0;
      // voting is not tied to IP and they are a guest...
      if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.PollVoteTiedToIP)
      {
        return true;
      }

      // Check if a user already voted
        foreach (DataRow drow in
            _dtVotes.Rows.Cast<DataRow>().Where(drow => Convert.ToInt32(drow["PollID"]) == Convert.ToInt32(pollId)))
        {
            choiceId = drow["ChoiceID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(drow["ChoiceID"]);
            return false;
        }
    
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
    protected bool IsPollClosed(object pollId)
    {
        bool soon;
        int? dtr = this.DaysToRun(pollId, out soon);
        return dtr == 0;
    }

    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      // Only if this control is in a topic we find the topic creator
            if (this.TopicId > 0)
            {
                DataRow dti = DB.topic_info(this.TopicId);
                this.topicUser = Convert.ToInt32(dti["UserID"]);
                if (!dti["PollID"].IsNullOrEmptyDBField())
                {
                    this.PollGroupId = Convert.ToInt32(dti["PollID"]);
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
      
      this.NewPollRow.Visible = this.ShowButtons && (topicPoll || forumPoll || categoryPoll || boardPoll) && this.HasOwnerExistingGroupAccess() && (!existingPoll);
     
      // if this is > 0 then we already have a poll and will display all buttons
      if (this.PollGroupId > 0)
      {
        this.BindData();
      }
      else
      {
          if (this.NewPollRow.Visible)
        {
          this.BindCreateNewPollRow();
        }
      }
    }

    /// <summary>
    /// PollGroup_ItemCommand
    /// </summary>
    /// <param name="source">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void PollGroup_ItemCommand(object source, RepeaterCommandEventArgs e)
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
          DB.poll_remove(this.PollGroupId, e.CommandArgument, this.BoardId, false, false);
          this.ReturnToPage();
          // BindData();
        }
      }

      if (e.CommandName == "removeall" && this.PageContext.ForumVoteAccess)
      {
        if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
        {
          DB.poll_remove(this.PollGroupId, e.CommandArgument, this.BoardId, true, false);
          this.ReturnToPage();
          // BindData();
        }
      }

      if (e.CommandName == "removegroup" && this.PageContext.ForumVoteAccess)
      {
        DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, false, false);
        this.ReturnToPage();

        // BindData();
      }

      if (e.CommandName == "removegroupall" && this.PageContext.ForumVoteAccess)
      {
        DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, true, false);
        this.ReturnToPage();

        // BindData();
      }

      if (e.CommandName == "removegroupevery" && this.PageContext.ForumVoteAccess)
      {
        DB.pollgroup_remove(this.PollGroupId, this.TopicId, this.ForumId, this.CategoryId, this.BoardId, false, true);
        this.ReturnToPage();

        // BindData();
      }
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
    protected void PollGroup_OnItemDataBound(object source, RepeaterItemEventArgs e)
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
        BindPollQuestionImage(item, drowv);
          
        var pollChoiceList = item.FindControlRecursiveAs<PollChoiceList>("PollChoiceList1");

       
        int pollId = drowv.Row["PollID"].ToType<int>();
        int choicePId = 0;
        bool isNotVoted = this.IsNotVoted(pollId, out choicePId);
        bool cvote = HasVoteAccess(pollId) ? isNotVoted : false;
        

        // If guest are not allowed to view options we don't render them
        pollChoiceList.Visible = !cvote && !this.PageContext.BoardSettings.AllowGuestsViewPollOptions &&
                         this.PageContext.IsGuest
                           ? false
                           : true;

        bool isClosedBound = false;

        // This is not a guest w/o poll option view permissions, we bind the control.
        if (pollChoiceList.Visible)
        {
            pollChoiceList.ChoiceId = choicePId;

            DataTable thisPollTable = this._dtPoll.Copy();
            foreach (DataRow thisPollTableRow in thisPollTable.Rows)
            {
                if (Convert.ToInt32(thisPollTableRow["PollID"]) != Convert.ToInt32(pollId))
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
        if (this.isBound)
        {
            // compare a number of voted polls with number of polls
            if ((this._dtPollGroup.Rows.Cast<DataRow>().Count(
                    dr => !this.IsNotVoted(dr["PollID"], out choicePId) && !this.IsPollClosed(dr["PollID"]))) >= this.PollNumber)
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
        else if (!this.isBound && isClosedBound && isPollClosed)
        {
            this._showResults = true;
        }
        if (!isClosedBound && !this.isBound)
        {
            this._showResults = true;
        }

          pollChoiceList.HideResults = !_showResults;

        // Clear the fields after the child repeater is bound
        this._showResults = false;
       // this._canVote = false;

        // Add confirmations to delete buttons
        AddPollButtonConfirmations(item, pollId);
       
        // *************************
        // Poll warnings section
        // *************************
        string notificationString = string.Empty;
        // Here warning labels are treated
 
       
       
        if (cvote)
        {
          if (this.isBound && this.PollNumber > 1 && this.PollNumber >= this._dtVotes.Rows.Count)
          {
              notificationString += this.PageContext.Localization.GetText("POLLEDIT", "POLLGROUP_BOUNDWARN");
          }
          
        }

        if (this.PageContext.IsGuest)
        {
            if (!cvote)
            {
                if (!this.PageContext.BoardSettings.AllowGuestsViewPollOptions)
                {
                    notificationString += " {0}".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "POLLOPTIONSHIDDEN_GUEST"));
                }
                if (isNotVoted)
                {
                    notificationString += " {0}".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "POLL_NOPERM_GUEST"));
                }
                
          }
        }

        if (!isNotVoted &&
            (this.PageContext.ForumVoteAccess ||
             (this.PageContext.BoardVoteAccess && (this.BoardId > 0 || this.EditBoardId > 0))))
        {
          notificationString += " {0}".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "POLL_VOTED"));
        }

        // Poll has expiration date
        if (daystorun > 0)
        {
          if (!soon)
          {
            notificationString += " {0}".FormatWith(this.PageContext.Localization.GetTextFormatted("POLL_WILLEXPIRE", daystorun));
          }
          else
          {
            notificationString += " {0}".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS"));
          }
          
        }
        else if (daystorun == 0)
        {
         
          notificationString += " {0}".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "POLL_EXPIRED"));
        }

        pollChoiceList.CanVote = cvote;
        pollChoiceList.DaysToRun = daystorun;
       
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
      if (item.ItemType == ListItemType.Footer)
      {
          if (_groupNotificationString.IsSet())
          {
              // we don't display warnings row if no info
              item.FindControlRecursiveAs<HtmlTableRow>("PollInfoTr").Visible = true;
              var pgn = item.FindControlRecursiveAs<Label>("PollGroupNotification");
              pgn.Text = _groupNotificationString;
              pgn.Visible = true;
          }
          AddPollGroupButtonConfirmations(item);
      }
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
    protected void RemovePollCompletely_Load(object sender, EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
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
    protected void RemovePoll_Load(object sender, EventArgs e)
    {
      ((ThemeButton)sender).Attributes["onclick"] =
        "return confirm('{0}');".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE"));
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
    private static decimal GetImageAspect(object mimeType)
    {
      if (!mimeType.IsNullOrEmptyDBField())
      {
        string[] attrs = mimeType.ToString().Split('!')[1].Split(';');
        decimal width = Convert.ToDecimal(attrs[0]);
        return width / Convert.ToDecimal(attrs[1]);
      }

      return 1;
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
      this._dtPoll = DB.pollgroup_stats(this.PollGroupId);

      // if the page user can cheange the poll. Only a group owner, forum moderator  or an admin can do it.   
      this._canChange = (Convert.ToInt32(this._dtPoll.Rows[0]["GroupUserID"]) == this.PageContext.PageUserID) ||
                        this.PageContext.IsAdmin || this.PageContext.IsForumModerator;

      // check if we should hide pollgroup repeater when a message is posted
      if (this.Parent.Page.ClientQueryString.Contains("postmessage"))
      {
        this.PollGroup.Visible = ((this.EditMessageId > 0) && (!this._canChange)) || this._canChange;
      }
      else
      {
        this.PollGroup.Visible = true;
      }

      this._dtPollGroup = this._dtPoll.Copy();

      // TODO: repeating code - move to Utils
      // Remove repeating PollID values   
      var hTable = new Hashtable();
      var duplicateList = new ArrayList();

      foreach (DataRow drow in this._dtPollGroup.Rows)
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
        this._dtPollGroup.Rows.Remove(dRow);
      }

      this._dtPollGroup.AcceptChanges();

      // frequently used
      this.PollNumber = this._dtPollGroup.Rows.Count;

      if (this._dtPollGroup.Rows.Count > 0)
      {
        // Check if the user is already voted in polls in the group 
        object userId = null;
        object remoteIp = null;

        if (this.PageContext.BoardSettings.PollVoteTiedToIP)
        {
          remoteIp = IPHelper.IPStrToLong(this.Request.UserHostAddress).ToString();
        }

        if (!this.PageContext.IsGuest)
        {
          userId = this.PageContext.PageUserID;
        }

        this._dtVotes = DB.pollgroup_votecheck(this.PollGroupId, userId, remoteIp);

        this.isBound = Convert.ToInt32(this._dtPollGroup.Rows[0]["IsBound"]) == 2;

        this.PollGroup.DataSource = this._dtPollGroup;

        // we hide new poll row if a poll exist
        this.NewPollRow.Visible = false;
      }

      this.DataBind();
    }

    /// <summary>
    /// The has owner existing group access.
    /// </summary>
    /// <returns>
    /// The has owner existing group access.
    /// </returns>
    private bool HasOwnerExistingGroupAccess()
    {
      if (this.PageContext.BoardSettings.AllowedPollChoiceNumber > 0)
      {
        // if topicid > 0 it can be every member
        if (this.TopicId > 0)
        {
          return (this.topicUser == this.PageContext.PageUserID) || this.PageContext.IsAdmin ||
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
    /// A method to return parameters string. 
    /// Consrtucts parameters string to send to other forms.
    /// </summary>
    /// <returns>
    /// The params to send.
    /// </returns>
    private string ParamsToSend()
    {
      String sb = String.Empty;

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
    private bool PollHasNoVotes(object pollId)
    {
      return
        this._dtPoll.Rows.Cast<DataRow>().Where(dr => Convert.ToInt32(dr["PollID"]) == Convert.ToInt32(pollId)).All(
          dr => Convert.ToInt32(dr["Votes"]) <= 0);
    }

    /// <summary>
    /// Returns user to the original call page.
    /// </summary>
    private void ReturnToPage()
    {
      // We simply return here to the page where the control is put. It can be made other way.
      if (this.TopicId > 0)
      {
        YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.TopicId);
      }

      if (this.EditMessageId > 0)
      {
        YafBuildLink.Redirect(ForumPages.postmessage, "m={0}", this.EditMessageId);
      }

      if (this.ForumId > 0)
      {
        YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.ForumId);
      }

      if (this.EditForumId > 0)
      {
        YafBuildLink.Redirect(ForumPages.admin_editforum, "f={0}", this.ForumId);
      }

      if (this.CategoryId > 0)
      {
        YafBuildLink.Redirect(ForumPages.forum, "c={0}", this.CategoryId);
      }

      if (this.EditCategoryId > 0)
      {
        YafBuildLink.Redirect(ForumPages.admin_editcategory, "c={0}", this.EditCategoryId);
      }

      if (this.BoardId > 0)
      {
        YafBuildLink.Redirect(ForumPages.forum);
      }

      if (this.EditBoardId > 0)
      {
        YafBuildLink.Redirect(ForumPages.admin_editboard, "b={0}", this.EditBoardId);
      }

      YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
    }

    /// <summary>
    /// The method to return if a user already voted
    /// </summary>
    /// <param name="pollId">The pollId value.</param>
    /// <returns>Return True if a user has voted.</returns>
    private bool HasVoteAccess(object pollId)
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

        if (this.IsPollClosed(pollId))
        {
            return false;
        }

        return true;
    }

     private void AddPollGroupButtonConfirmations(RepeaterItem ri)
    {
        var pgcr = ri.FindControlRecursiveAs<HtmlTableRow>("PollGroupCommandRow");
        pgcr.Visible = this.HasOwnerExistingGroupAccess() && this.ShowButtons;
        // return confirmations for poll group 
        if (pgcr.Visible)
        {
            ri.FindControlRecursiveAs<ThemeButton>("RemoveGroup").Attributes["onclick"] =
                "return confirm('{0}');".FormatWith(
                    this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLGROUP_DELETE"));

            ri.FindControlRecursiveAs<ThemeButton>("RemoveGroupAll").Attributes["onclick"] =
                "return confirm('{0}');".FormatWith(
                    this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_ALL"));

            ri.FindControlRecursiveAs<ThemeButton>("RemoveGroupEverywhere").Attributes["onclick"] =
                "return confirm('{0}');".FormatWith(
                    this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_EVR"));
        }

    }

    private void AddPollButtonConfirmations(RepeaterItem ri, int pollId)
    {
        // Add confirmations to delete buttons
        var removePollAll = ri.FindControlRecursiveAs<ThemeButton>("RemovePollAll");
        removePollAll.Attributes["onclick"] =
          "return confirm('{0}');".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
      removePollAll.Visible = this.CanRemovePollCompletely(pollId);

        var removePoll = ri.FindControlRecursiveAs<ThemeButton>("RemovePoll");
        removePoll.Attributes["onclick"] =
          "return confirm('{0}');".FormatWith(this.PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE"));
        removePoll.Visible = this.CanRemovePoll(pollId);
        
    }

     private void BindPollQuestionImage(RepeaterItem item, DataRowView drowv)
{
     var questionImage = item.FindControlRecursiveAs<HtmlImage>("QuestionImage");
     var questionAnchor = item.FindControlRecursiveAs<HtmlAnchor>("QuestionAnchor");

     // The image is not from theme
     if (!drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField())
     {
         questionAnchor.Attributes["rel"] = "lightbox-group" + Guid.NewGuid().ToString().Substring(0, 5);
         questionAnchor.HRef = drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField()
                                 ? this.GetThemeContents("VOTE", "POLL_CHOICE")
                                 : this.HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());
         questionAnchor.Title = this.HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());

         questionImage.Src = questionImage.Alt = this.HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());
         if (!drowv.Row["QuestionMimeType"].IsNullOrEmptyDBField())
         {
             decimal aspect = GetImageAspect(drowv.Row["QuestionMimeType"]);

             // hardcoded - bad
             questionImage.Width = 80;
             questionImage.Height = Convert.ToInt32(questionImage.Width / aspect);
         }
     }
     else
     {
         // image from theme no need to resize it
         questionImage.Alt =  this.PageContext.Localization.GetText("POLLEDIT", "POLL_PLEASEVOTE");
         questionImage.Src = this.GetThemeContents("VOTE", "POLL_QUESTION");
         questionAnchor.HRef = string.Empty;
     }
 }


    #endregion
  }
}