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
using System.Collections;

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Drawing;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Web;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The polledit.
  /// </summary>
  public partial class polledit : ForumPage
  {
    #region Constants and Fields

    /// <summary>
    ///   Table with choices
    /// </summary>
    protected DataRow _topicInfo;

    /// <summary>
    /// The board id.
    /// </summary>
    protected int? boardId;

    /// <summary>
    /// The category id.
    /// </summary>
    protected int? categoryId;

    /// <summary>
    ///   Table with choices
    /// </summary>
    protected DataTable choices;

    /// <summary>
    /// The date poll expire.
    /// </summary>
    protected DateTime? datePollExpire;

    // protected int? pollID;

    /// <summary>
    /// The days poll expire.
    /// </summary>
    protected int daysPollExpire;

    /// <summary>
    /// The edit board id.
    /// </summary>
    protected int? editBoardId;

    /// <summary>
    /// The edit category id.
    /// </summary>
    protected int? editCategoryId;

    /// <summary>
    /// The edit forum id.
    /// </summary>
    protected int? editForumId;

    /// <summary>
    /// The edit message id.
    /// </summary>
    protected int? editMessageId;

    /// <summary>
    /// The edit topic id.
    /// </summary>
    protected int? editTopicId;

    /// <summary>
    /// The forum id.
    /// </summary>
    protected int? forumId;

    /// <summary>
    /// The return forum.
    /// </summary>
    protected int? returnForum;

    /// <summary>
    /// The topic id.
    /// </summary>
    protected int? topicId;

    /// <summary>
    /// The topic unapproved.
    /// </summary>
    protected bool topicUnapproved;

    /// <summary>
    /// The currentSelection.
    /// </summary>
    protected int currentSelection;

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
    protected int? PollID { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// An image reader to read images on local disk.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    public Stream GetLocalData(Uri path)
    {
      return new FileStream(path.LocalPath, FileMode.Open);
    }

    /// <summary>
    /// The get remote data.
    /// </summary>
    /// <param name="url">
    /// The url.
    /// </param>
    /// <returns>
    /// </returns>
    public Stream GetRemoteData(Uri url)
    {
      string path = url.ToString();

      try
      {
        if (path.StartsWith("~/"))
        {
          path = "file://" + HttpRuntime.AppDomainAppPath + path.Substring(2, path.Length - 2);
        }

        WebRequest request = WebRequest.Create(new Uri(path));

        WebResponse response = request.GetResponse();

        return response.GetResponseStream();
      }
      catch
      {
        return new MemoryStream();
      }
 // Don't make the program crash just because we have a picture which failed downloading
    }

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
    /// The change poll show status.
    /// </summary>
    /// <param name="newStatus">
    /// The new status.
    /// </param>
    protected void ChangePollShowStatus(bool newStatus)
    {
      this.SavePoll.Visible = newStatus;
      this.Cancel.Visible = newStatus;
      this.PollRow1.Visible = newStatus;
      this.PollRowExpire.Visible = newStatus;

      /*  var pollRow = (HtmlTableRow)this.FindControl(String.Format("PollRow{0}", 1));

            if (pollRow != null)
            {
                pollRow.Visible = newStatus;
            } */
    }

    /// <summary>
    /// From a path, return a byte[] of the image.
    /// </summary>
    /// <param name="uriPath">
    /// </param>
    /// <returns>
    /// The get image parameters.
    /// </returns>
    protected string GetImageParameters(Uri uriPath)
    {
      string pseudoMime = string.Empty;
      using (Stream stream = this.GetRemoteData(uriPath))
      {
        var data = new byte[0];
        Bitmap img = null;
        try
        {
          pseudoMime += "image/png!";
          img = new Bitmap(stream);
          pseudoMime += img.Width;
          pseudoMime += ';';
          pseudoMime += img.Height;
        }
        catch
        {
          return null;
        }
        finally
        {
          if (img != null)
          {
            img.Dispose();
          }
        }

        stream.Close();
        return pseudoMime;
      }
    }

    /// <summary>
    /// The is input verified.
    /// </summary>
    /// <returns>
    /// The is input verified.
    /// </returns>
    protected bool IsInputVerified()
    {
      if (Convert.ToInt32(this.PollGroupListDropDown.SelectedIndex) <= 0)
      {
          if (this.Question.Text.Trim().Length == 0)
          {
              YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT", "NEED_QUESTION"));
              return false;
          }

          // If it's admin or moderator we don't check tags
          if (!PageContext.IsAdmin || PageContext.IsForumModerator)
          {
              string tagPoll = YafFormatMessage.CheckHtmlTags(this.Question.Text.Trim(),
                                                              PageContext.BoardSettings.AcceptedHeadersHTML, ',');

              if (tagPoll.IsSet())
              {
                  this.PageContext.AddLoadMessage(tagPoll);
                  return false;
              }
          }


          int notNullcount = 0;
          foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
          {
              string value = ((TextBox) ri.FindControl("PollChoice")).Text.Trim();

              if (!string.IsNullOrEmpty(value))
              {
                  notNullcount++;

                  // If it's admin or moderator we don't check tags
                  if (!PageContext.IsAdmin || PageContext.IsForumModerator)
                  {
                      string tagChoice = YafFormatMessage.CheckHtmlTags(value,
                                                                        PageContext.BoardSettings.AcceptedHeadersHTML,
                                                                        ',');
                      if (tagChoice.IsSet())
                      {
                          this.PageContext.AddLoadMessage(tagChoice);
                          return false;
                      }
                  }
              }
          }

          if (notNullcount < 2)
          {
              YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT", "NEED_CHOICES"));
              return false;
          }

          int dateVerified = 0;
          if (!int.TryParse(this.PollExpire.Text.Trim(), out dateVerified) &&
              (this.PollExpire.Text.Trim().IsSet()))
          {
              YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT", "EXPIRE_BAD"));
              return false;
          }
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

      this.PollObjectRow1.Visible = this.PageContext.IsAdmin && this.PageContext.ForumPollAccess;

      if (int.TryParse(this.PollExpire.Text.Trim(), out this.daysPollExpire))
      {
        this.datePollExpire = DateTime.UtcNow.AddDays(this.daysPollExpire);
      }

      if (!this.IsPostBack)
      {
          this.AddPageLinks();

          // Admin can attach an existing group if it's a new poll - this.pollID <= 0
          if (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
          {
              var pollGroup =
                  DB.PollGroupList(this.PageContext.PageUserID, null, this.PageContext.PageBoardID).Distinct(
                      new AreEqualFunc<TypedPollGroup>((id1, id2) => id1.PollGroupID == id2.PollGroupID)).ToList();

              pollGroup.Insert(0, new TypedPollGroup(String.Empty, -1));

              this.PollGroupListDropDown.Items.AddRange(
                  pollGroup.Select(x => new ListItem(x.Question, x.PollGroupID.ToString())).ToArray());
              
              this.PollGroupListDropDown.DataBind();
              this.PollGroupList.Visible = true;
          }
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
      if (this.PageContext.ForumPollAccess && this.IsInputVerified())
      {
        if (this.GetPollID() == true)
        {
          this.ReturnToPage();
        }
      }
    }

    /// <summary>
    /// Adds page links to the page
    /// </summary>
    private void AddPageLinks()
    {
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      if (this.categoryId > 0)
      {
        this.PageLinks.AddLink(
          this.PageContext.PageCategoryName, YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.categoryId));
      }

      if (this.returnForum > 0)
      {
        this.PageLinks.AddLink(
          DB.forum_list(this.PageContext.PageBoardID, this.returnForum).Rows[0]["Name"].ToString(), 
          YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.returnForum));
      }

      if (this.forumId > 0)
      {
        this.PageLinks.AddLink(
          DB.forum_list(this.PageContext.PageBoardID, this.returnForum).Rows[0]["Name"].ToString(), 
          YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.forumId));
      }

      if (this.topicId > 0)
      {
        this.PageLinks.AddLink(
          this._topicInfo["Topic"].ToString(), YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.topicId));
      }

      if (this.editMessageId > 0)
      {
        this.PageLinks.AddLink(
          this._topicInfo["Topic"].ToString(), YafBuildLink.GetLink(ForumPages.postmessage, "m={0}", this.editMessageId));
      }

      this.PageLinks.AddLink(this.GetText("POLLEDIT", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Checks access rights for the page
    /// </summary>
    private void CheckAccess()
    {
      if (this.boardId > 0 || this.categoryId > 0)
      {
        // invalid category
        bool categoryVars = this.categoryId > 0 &&
                            (this.topicId > 0 || this.editTopicId > 0 || this.editMessageId > 0 || this.editForumId > 0 ||
                             this.editBoardId > 0 || this.forumId > 0 || this.boardId > 0);

        // invalid board vars
        bool boardVars = this.boardId > 0 &&
                         (this.topicId > 0 || this.editTopicId > 0 || this.editMessageId > 0 || this.editForumId > 0 ||
                          this.editBoardId > 0 || this.forumId > 0 || this.categoryId > 0);
        if (!categoryVars || (!boardVars))
        {
          YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
        }
      }
      else if (this.forumId > 0 && (!this.PageContext.ForumPollAccess))
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
    }

    /// <summary>
    /// The get poll id.
    /// </summary>
    /// <returns>
    /// The get poll id.
    /// </returns>
    private bool? GetPollID()
    {
      if (int.TryParse(this.PollExpire.Text.Trim(), out this.daysPollExpire))
      {
        this.datePollExpire = DateTime.UtcNow.AddDays(this.daysPollExpire);
      }

      string questionPath = this.QuestionObjectPath.Text.Trim();
      string questionMime = string.Empty;

      if (questionPath.IsSet())
      {
        questionMime = this.GetImageParameters(new Uri(questionPath));
        if (questionMime == null)
        {
          YafContext.Current.AddLoadMessage(
            YafContext.Current.Localization.GetTextFormatted("POLLIMAGE_INVALID", this.QuestionObjectPath.Text.Trim()));
          return false;
        }
      }

      // we are just using existing poll
      if (this.PollID != null)
      {
        DB.poll_update(
          this.PollID, 
          this.Question.Text, 
          this.datePollExpire, 
          this.IsBoundCheckBox.Checked, 
          this.IsClosedBoundCheckBox.Checked, 
          questionPath, 
          questionMime);

        foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
        {
          string choice = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
          string chid = ((HiddenField)ri.FindControl("PollChoiceID")).Value;
          string objectPath = ((TextBox)ri.FindControl("ObjectPath")).Text.Trim();

          string parametrs = string.Empty;

          // update choice
          if (objectPath.IsSet())
          {
            parametrs = this.GetImageParameters(new Uri(objectPath));
            if (parametrs == null)
            {
              YafContext.Current.AddLoadMessage(
                YafContext.Current.Localization.GetTextFormatted(
                  "POLLIMAGE_INVALID", ri.FindControlRecursiveAs<TextBox>("ObjectPath").Text));
              return false;
            }
          }

          if (string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
          {
            // add choice
            DB.choice_add(this.PollID, choice, objectPath, parametrs);
          }
          else if (!string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
          {
            DB.choice_update(chid, choice, objectPath, parametrs);
          }
          else if (!string.IsNullOrEmpty(chid) && string.IsNullOrEmpty(choice))
          {
            // remove choice
            DB.choice_delete(chid);
          }
        }

        return true;
      }
      else if (this.PollID == null)
      {
        // User wishes to create a poll  
        // The value was selected, we attach an existing poll
        if (Convert.ToInt32(this.PollGroupListDropDown.SelectedIndex) > 0)
        {
          int result = DB.pollgroup_attach(
            Convert.ToInt32(this.PollGroupListDropDown.SelectedValue), 
            this.topicId, 
            this.forumId, 
            this.categoryId, 
            this.boardId);
          if (result == 1)
          {
            this.PageContext.LoadMessage.Add(this.GetText("POLLEDIT", "POLLGROUP_ATTACHED"));
          }

          return true;
        }
        else
        {
          // vzrus: always one in the current code - a number of  polls for a topic
          int questionsTotal = 1;

          var pollList = new List<PollSaveList>(questionsTotal);

          var rawChoices = new string[3, this.ChoiceRepeater.Items.Count];
          int j = 0;
          foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
          {
            rawChoices[0, j] = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
            rawChoices[1, j] = questionPath;
            rawChoices[2, j] = questionMime;
            j++;
          }

          int? realTopic = this.topicId;

          if (this.topicId == null)
          {
            realTopic = this.editTopicId;
          }

          if (this.datePollExpire == null && (this.PollExpire.Text.Trim().IsSet()))
          {
            this.datePollExpire = DateTime.UtcNow.AddDays(Convert.ToInt32(this.PollExpire.Text.Trim()));
          }

          pollList.Add(
            new PollSaveList(
              this.Question.Text, 
              rawChoices, 
              this.datePollExpire, 
              this.PageContext.PageUserID, 
              realTopic, 
              this.forumId, 
              this.categoryId, 
              this.boardId, 
              questionPath, 
              questionMime, 
              this.IsBoundCheckBox.Checked, 
              this.IsClosedBoundCheckBox.Checked));
          DB.poll_save(pollList);
          return true;
        }
      }

      return false; // A poll was not created for this topic.
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
      this.choices = DB.poll_stats(pollID);
      this.choices.Columns.Add("ChoiceOrderID", typeof(int));

      // First existing values alway 1!
      int existingRowsCount = 1;
      int allExistingRowsCount = this.choices.Rows.Count;

      // we edit existing poll 
      if (this.choices.Rows.Count > 0)
      {
        if ((Convert.ToInt32(this.choices.Rows[0]["UserID"]) != this.PageContext.PageUserID) &&
            (!this.PageContext.IsAdmin) && (!this.PageContext.IsForumModerator))
        {
          YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
        }

        if (Convert.ToInt32(this.choices.Rows[0]["IsBound"]) == 2)
        {
          this.IsBoundCheckBox.Checked = true;
        }

        if (Convert.ToInt32(this.choices.Rows[0]["IsClosedBound"]) == 4)
        {
          this.IsClosedBoundCheckBox.Checked = true;
        }

        this.Question.Text = this.choices.Rows[0]["Question"].ToString();
        this.QuestionObjectPath.Text = this.choices.Rows[0]["QuestionObjectPath"].ToString();

        if (this.choices.Rows[0]["Closes"] != DBNull.Value)
        {
          TimeSpan closing = (DateTime)this.choices.Rows[0]["Closes"] - DateTime.UtcNow;

          this.PollExpire.Text = SqlDataLayerConverter.VerifyInt32(closing.TotalDays + 1).ToString();
        }
        else
        {
          this.PollExpire.Text = null;
        }

        foreach (DataRow choiceRow in this.choices.Rows)
        {
          choiceRow["ChoiceOrderID"] = existingRowsCount;

          existingRowsCount++;
        }

        // Get isBound value directly
        if (Convert.ToInt32(this.choices.Rows[0]["IsBound"]) == 2)
        {
          this.IsBoundCheckBox.Checked = true;
        }

        if (Convert.ToInt32(this.choices.Rows[0]["IsClosedBound"]) == 4)
        {
          this.IsClosedBoundCheckBox.Checked = true;
        }
      }
      else
      {
         // A new topic is created
         // below check currently if works for topics only, but will do as some things are not enabled 
         if (!CanCreatePoll())
         {
             YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
         }
        // Get isBound value using page variables. They are initialized here.
        int pgidt = 0;

        // If a topic poll is edited or new topic created
        if (this.topicId > 0 && this._topicInfo != null)
        {
          // topicid should not be null here 
          if (this._topicInfo["PollID"] != DBNull.Value)
          {
            pgidt = (int)this._topicInfo["PollID"];

            DataTable pollGroupData = DB.pollgroup_stats(pgidt);

            if (Convert.ToInt32(pollGroupData.Rows[0]["IsBound"]) == 2)
            {
              this.IsBoundCheckBox.Checked = true;
            }

            if (Convert.ToInt32(DB.pollgroup_stats(pgidt).Rows[0]["IsClosedBound"]) == 4)
            {
              this.IsClosedBoundCheckBox.Checked = true;
            }
          }
        }
        else if (this.forumId > 0 && (!(this.topicId > 0) || (!(this.editTopicId > 0))))
        {
          // forumid should not be null here
          pgidt = (int)DB.forum_list(this.PageContext.PageBoardID, this.forumId).Rows[0]["PollGroupID"];
        }
        else if (this.categoryId > 0)
        {
          // categoryid should not be null here
          pgidt =
            (int)
            DB.category_listread(this.PageContext.PageBoardID, this.PageContext.PageUserID, this.categoryId).Rows[0][
              "PollGroupID"];
        }

        if (pgidt > 0)
        {
          if (Convert.ToInt32(DB.pollgroup_stats(pgidt).Rows[0]["IsBound"]) == 2)
          {
            this.IsBoundCheckBox.Checked = true;
          }

          if (Convert.ToInt32(DB.pollgroup_stats(pgidt).Rows[0]["IsClosedBound"]) == 4)
          {
            this.IsClosedBoundCheckBox.Checked = true;
          }
        }

        // clear the fields...
        this.PollExpire.Text = string.Empty;
        this.Question.Text = string.Empty;
      }

      int dummyRowsCount = this.PageContext.BoardSettings.AllowedPollChoiceNumber - allExistingRowsCount - 1;
      for (int i = 0; i <= dummyRowsCount; i++)
      {
        DataRow drow = this.choices.NewRow();
        drow["ChoiceOrderID"] = existingRowsCount + i;
        this.choices.Rows.Add(drow);
      }

      this.ChoiceRepeater.DataSource = this.choices;
      this.ChoiceRepeater.DataBind();
      this.ChoiceRepeater.Visible = true;

      this.ChangePollShowStatus(true);
      this.IsBound.Visible = PageContext.BoardSettings.AllowUsersHidePollResults || PageContext.IsAdmin || PageContext.IsForumModerator;
      this.IsClosedBound.Visible = PageContext.BoardSettings.AllowUsersHidePollResults || PageContext.IsAdmin || PageContext.IsForumModerator;
    }

    /// <summary>
    /// Initializes page context query variables.
    /// </summary>
    private void InitializeVariables()
    {
      this.PageContext.QueryIDs =
        new QueryStringIDHelper(
          new[] { "p", "ra", "ntp", "t", "e", "em", "m", "f", "ef", "c", "ec", "b", "eb", "rf" });

      if (this.PageContext.QueryIDs != null)
      {
        // we return to a specific place, general token 
        if (this.PageContext.QueryIDs.ContainsKey("ra"))
        {
          this.topicUnapproved = true;
        }

        // we return to a forum (used when a topic should be approved)
        if (this.PageContext.QueryIDs.ContainsKey("f"))
        {
          this.forumId = this.returnForum = (int)this.PageContext.QueryIDs["f"];
        }

        if (this.PageContext.QueryIDs.ContainsKey("t"))
        {
          this.topicId = (int)this.PageContext.QueryIDs["t"];
          this._topicInfo = DB.topic_info(this.topicId);
        }

        if (this.PageContext.QueryIDs.ContainsKey("m"))
        {
          this.editMessageId = (int)this.PageContext.QueryIDs["m"];
        }

        if (this.editMessageId == null)
        {
          if (this.PageContext.QueryIDs.ContainsKey("ef"))
          {
            this.categoryId = (int)this.PageContext.QueryIDs["ef"];
          }

          if (this.editForumId == null)
          {
            if (this.PageContext.QueryIDs.ContainsKey("c"))
            {
              this.categoryId = (int)this.PageContext.QueryIDs["c"];
            }

            if (this.categoryId == null)
            {
              if (this.PageContext.QueryIDs.ContainsKey("ec"))
              {
                this.editCategoryId = (int)this.PageContext.QueryIDs["ec"];
              }

              if (this.editCategoryId == null)
              {
                if (this.PageContext.QueryIDs.ContainsKey("b"))
                {
                  this.boardId = (int)this.PageContext.QueryIDs["b"];
                }

                if (this.boardId == null)
                {
                  if (this.PageContext.QueryIDs.ContainsKey("eb"))
                  {
                    this.editBoardId = (int)this.PageContext.QueryIDs["eb"];
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
          this.PollID = (int)this.PageContext.QueryIDs["p"];
          this.InitPollUI(this.PollID);
        }
        else
        {
          // new poll
          this.PollRow1.Visible = true;
          this.InitPollUI(null);
        }
      }
      else
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
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
      string ars = null;
      int vl = 0;

      if (this.topicId > 0)
      {
        retliterals = "t";
        retvalue = this.topicId;
      }
      else if (this.editMessageId > 0)
      {
        retliterals = "em";
        retvalue = this.editMessageId;
      }
      else if (this.forumId > 0)
      {
        retliterals = "f";
        retvalue = this.forumId;
      }
      else if (this.editForumId > 0)
      {
        retliterals = "ef";
        retvalue = this.editForumId;
      }
      else if (this.categoryId > 0)
      {
        retliterals = "c";
        retvalue = this.categoryId;
      }
      else if (this.editCategoryId > 0)
      {
        retliterals = "ec";
        retvalue = this.editCategoryId;
      }
      else if (this.boardId > 0)
      {
        retliterals = "b";
        retvalue = this.boardId;
      }
      else if (this.editBoardId > 0)
      {
        retliterals = "eb";
        retvalue = this.editBoardId;
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
      if (this.topicUnapproved)
      {
        // Tell user that his message will have to be approved by a moderator
        string url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.returnForum);

        if (Config.IsRainbow)
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1");
        }
        else
        {
          YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
        }
      }

      // YafBuildLink.Redirect(ForumPages.posts, "m={0}#{0}", this.Request.QueryString.GetFirstOrDefault("m"));      
      string retliterals = string.Empty;
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
        if (this.topicId > 0)
        {
            // admins can add any number of polls
            if (PageContext.IsAdmin || PageContext.IsForumModerator)
            {
                return true;
            }

            int? pollGroupId = null;
            if (!_topicInfo["PollID"].IsNullOrEmptyDBField())
            {
                pollGroupId = Convert.ToInt32(_topicInfo["PollID"]);
            }

            if (pollGroupId == null && PageContext.BoardSettings.AllowedPollNumber > 0 && PageContext.ForumPollAccess)
            {
                return true;
            }
            else
            {
                // TODO: repeating code
                // Remove repeating PollID values   
                var hTable = new Hashtable();
                var duplicateList = new ArrayList();
                DataTable dtPollGroup = DB.pollgroup_stats(pollGroupId);

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
                int pollNumber = dtPollGroup.Rows.Count;

                return (pollNumber < this.PageContext.BoardSettings.AllowedPollNumber) &&
                       (this.PageContext.BoardSettings.AllowedPollChoiceNumber > 0);
            }
        }
        return true;
    }

    #endregion
  }
}