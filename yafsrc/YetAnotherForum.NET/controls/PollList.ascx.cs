

using System.Linq;

namespace YAF.controls
{
    // YAF.Pages
    #region Using

    using System;
    using System.Collections;
    using System.Data;
    using System.Text;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using Classes;
    using Classes.Core;
    using Classes.Data;
    using Classes.Utils;
    using Controls;


    #endregion

    ///<summary>
    /// PollList Class
    ///</summary>
    public partial class PollList : BaseUserControl
    {
        /// <summary>
        ///   The _data bound.
        /// </summary>
        private bool _dataBound;
  

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
        ///   The topic User.
        /// </summary>
        private int? topicUser;

        /// <summary>
        ///   The _canChange.
        /// </summary>
        private bool _canChange;

        /// <summary>
        ///   The _showResults. Used to store data from parent repeater.
        /// </summary>
        private bool _showResults;

        /// <summary>
        ///   The _canVote. Used to store data from parent repeater.
        /// </summary>
        private bool _canVote;

        /// <summary>
        ///   The isBound.
        /// </summary>
        private bool isBound;

        /// <summary>
        ///   The isClosedBound.
        /// </summary>
        private bool isClosedBound;

        /// <summary>
        /// Returns PollGroupID
        /// </summary>
        public int? PollGroupId { get; set; }

        /// <summary>
        /// Returns TopicId
        /// </summary>
        public int TopicId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditMessageId.
        /// </summary>
        public int EditMessageId
        {
            get;
            set;
        }
        
        /// <summary>
        /// Returns CategoryId
        /// </summary>
        public int CategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditCategoryId
        /// </summary>
        public int EditCategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns ForumId
        /// </summary>
        public int ForumId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditForumId
        /// </summary>
        public int EditForumId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns BoardId
        /// </summary>
        public int BoardId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns EditBoardId.
        /// Used to return to edit board page.
        /// Currently is not implemented.
        /// </summary>
        public int EditBoardId
        {
            get;
            set;
        }

        /// <summary>
        /// Returns PollNumber
        /// </summary>
        public int PollNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Returns MaxImageAspect. Stores max aspect to get rows of equal height.
        /// </summary>
        public decimal MaxImageAspect
        {
            get;
            set;
        }


        /// <summary>
        /// Returns IsLocked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Returns If we are editing a post
        /// </summary>
        public bool PostEdit { get; set; }

        /// <summary>
        /// Returns ShowButtons
        /// </summary>
        public bool ShowButtons { get; set; }
       

        /// <summary>
        ///   Gets VotingCookieName.
        /// </summary>
        protected string VotingCookieName(int pollId)
        {
          
                return String.Format("poll#{0}", pollId);
           
        }

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowButtons = true;
           // if (!IsPostBack)
          //  {
               if (TopicId > 0)
                {
                    topicUser = Convert.ToInt32(DB.topic_info(TopicId)["UserID"]);
                }

            bool existingPoll = (PollGroupId > 0) && ((TopicId > 0) || (ForumId > 0) || (BoardId > 0));
           
            bool topicPoll = (this.EditMessageId > 0 || (this.TopicId > 0 && ShowButtons));
            bool forumPoll = (this.EditForumId > 0 || (this.ForumId > 0 && ShowButtons));
            bool categoryPoll = (this.EditCategoryId > 0 || (this.CategoryId > 0 && ShowButtons));
            bool boardPoll = (this.EditBoardId > 0 || (this.BoardId > 0 && ShowButtons));

            NewPollRow.Visible = HasOwnerExistingGroupAccess() && (!existingPoll) && (topicPoll || forumPoll || categoryPoll || boardPoll);
                if (PollGroupId > 0)
                {
                    BindData();
                }
                else
                {
                    if (NewPollRow.Visible)
                    {
                        BindCreateNewPollRow();
                    }
                }
        //    }

        }

        private bool HasOwnerExistingGroupAccess()
        { 

            if ((PageContext.BoardSettings.AllowedPollChoiceNumber > 0) && ShowButtons )
            {
                // it topicid > 0 it can be every member
                if (TopicId > 0)
                {
                     return (topicUser == PageContext.PageUserID) || PageContext.IsAdmin || PageContext.IsForumModerator;
                   
                }

                // only admins can edit this
                if (CategoryId > 0 || BoardId > 0)
                {
                    return PageContext.IsAdmin;
                }

                // in other places only admins and forum moderators can have access
                return PageContext.IsAdmin || PageContext.IsForumModerator;
            }
            return false;
           
        }

        ///<summary>
        /// Get Theme Contents
        ///</summary>
        ///<param name="page">The Page</param>
        ///<param name="tag">Tag</param>
        ///<returns>Content</returns>
        public string GetThemeContents(string page, string tag)
        {
            return PageContext.Theme.GetItem(page, tag);
        }


        private  void BindCreateNewPollRow()
        {
           
                var cpr = CreatePoll1;
                // ChangePollShowStatus(true);
                cpr.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.polledit,
                    "{0}",
                    ParamsToSend());
                cpr.DataBind();
            cpr.Visible = true;
            NewPollRow.Visible = true;
        }

        /// <summary>
        /// A method to return parameters string. It should be implemented in other way.
        /// </summary>
        /// <returns></returns>
        private string ParamsToSend()
        {
           StringBuilder sb = new StringBuilder();

            if (TopicId > 0)
            {
               sb.AppendFormat("t={0}", TopicId);
            }

            if (EditMessageId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("m={0}", EditMessageId);

            }

            if (ForumId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("f={0}", ForumId);
            }

            if (EditForumId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("ef={0}", EditForumId);
            }

            if (CategoryId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("c={0}", CategoryId);
            }

            if (EditCategoryId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("ec={0}", EditCategoryId);
            }

            if (BoardId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("b={0}", BoardId);
            }

            if (EditBoardId > 0)
            {
                if (!String.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append('&');
                }
                sb.AppendFormat("eb={0}", EditBoardId);
            }

            return sb.ToString();
          
        }


        protected bool IsNotVoted(object pollId)
        {

            // check for voting cookie
            if (Request.Cookies[VotingCookieName(Convert.ToInt32(pollId))] != null)
            {
                return false;
            }

            // voting is not tied to IP and they are a guest...
            if (PageContext.IsGuest && !PageContext.BoardSettings.PollVoteTiedToIP)
            {
                return true;
            }

            // Check if a user already voted
            return _dtVotes.Rows.Cast<DataRow>().All(dr => Convert.ToInt32(dr["PollID"]) != Convert.ToInt32(pollId));
        }

        /// <summary>
        ///   Property to verify if the current user can vote in this poll.
        /// </summary>
        protected bool CanVote(object pollId)
        {
       
           // rule out users without voting rights
           // If you come here from topics or edit TopicId should be > 0
           if (!PageContext.ForumVoteAccess && TopicId > 0)
           {
               return false;
           }
           if (!PageContext.BoardVoteAccess && BoardId > 0)
           {
               return false;
           }
           if (IsPollClosed(pollId))
           {
               return false;
           }

            return IsNotVoted(pollId);
            
        }

        /// <summary>
        /// The get poll is closed.
        /// </summary>
        /// <returns>
        /// The get poll is closed.
        /// </returns>
        protected string GetPollIsClosed(object pollId)
        {
            string strPollClosed = string.Empty;
            if (IsPollClosed(pollId))
            {
                strPollClosed = PageContext.Localization.GetText("POLL_CLOSED");
            }

            return strPollClosed;
        }

        /// <summary>
        /// The get poll question.
        /// </summary>
        /// <returns>
        /// The get poll question.
        /// </returns>
        protected string GetPollQuestion(object pollId)
        {
            foreach (DataRow dr in _dtPollGroup.Rows)
            {
                if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    return HtmlEncode(YafServices.BadWordReplace.Replace(dr["Question"].ToString()));
                }
                
            }
            return string.Empty;
           
        }

        /// <summary>
        /// The get total.
        /// </summary>
        /// <returns>
        /// The get total.
        /// </returns>
        protected string GetTotal(object  pollId)
        {
            foreach (DataRow dr in _dtPollGroup.Rows)
            {
                if (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    return HtmlEncode(dr["Total"].ToString());
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// The is poll closed.
        /// </summary>
        /// <returns>
        /// The is poll closed.
        /// </returns>
        protected bool IsPollClosed(object pollId)
        {
            return (from DataRow dr in _dtPollGroup.Rows
                    where (dr["Closes"] != DBNull.Value) && (Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                    select Convert.ToDateTime(dr["Closes"])).Any(tCloses => tCloses < DateTime.UtcNow);
        }

        /// <summary>
        /// The days to run.
        /// </summary>
        /// <returns>
        /// The days to run.
        /// </returns>
        protected int? DaysToRun(object pollId, out bool soon)
        {
            soon = false;
            foreach (DataRow dr in _dtPollGroup.Rows)
            {
                if (dr["Closes"] != DBNull.Value && Convert.ToInt32(pollId) == Convert.ToInt32(dr["PollID"]))
                {
                    DateTime tCloses = Convert.ToDateTime(dr["Closes"]).Date; 
                    if (tCloses > DateTime.UtcNow.Date)
                    {
                        int days = (tCloses - DateTime.UtcNow).Days;

                        return days == 0 ? 1 : days;
                    }

                    return 0;
                }

            }
            return null;
        }


     
        /// <summary>
        /// The vote width.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <returns>
        /// The vote width.
        /// </returns>
        protected int VoteWidth(object o)
        {
            var row = (DataRowView)o;
            return (int)row.Row["Stats"] * 80 / 100;
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
            DataRowView drowv = (DataRowView)e.Item.DataItem;
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
               //clear the value after choiced are bounded
               MaxImageAspect = 1;
               item.FindControlRecursiveAs<HtmlTableRow>("PollCommandRow").Visible = HasOwnerExistingGroupAccess() && ShowButtons;

                Repeater polloll = item.FindControlRecursiveAs<Repeater>("Poll");

                string pollId = drowv.Row["PollID"].ToString();
                polloll.Visible = !CanVote(pollId) && !PageContext.BoardSettings.AllowGuestsViewPollOptions && PageContext.IsGuest
                                    ? false
                                    : true;
                // Poll Choice image
                HtmlImage questionImage = item.FindControlRecursiveAs<HtmlImage>("QuestionImage");
                HtmlAnchor questionAnchor = item.FindControlRecursiveAs<HtmlAnchor>("QuestionAnchor");

                // Don't render if it's a standard image
                if (!drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField())
                {
                    questionAnchor.Attributes["rel"] = "lightbox-group" + Guid.NewGuid().ToString().Substring(0, 5);
                    questionAnchor.HRef = drowv.Row["QuestionObjectPath"].IsNullOrEmptyDBField()
                                            ? GetThemeContents("VOTE", "POLL_CHOICE")
                                            : HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());
                    questionAnchor.Title = HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());

                    questionImage.Alt = HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());
                    questionImage.Src = HtmlEncode(drowv.Row["QuestionObjectPath"].ToString());


                    if (!(drowv.Row["QuestionMimeType"]).IsNullOrEmptyDBField())
                    {
                        decimal aspect = GetImageAspect(drowv.Row["QuestionMimeType"]);

                        // hardcoded - bad
                        questionImage.Width = 80;
                        questionImage.Height = Convert.ToInt32(questionImage.Width / aspect);
                    }


                }
                else
                {
                    questionImage.Alt = PageContext.Localization.GetText("POLLEDIT", "POLL_PLEASEVOTE");
                    questionImage.Src = GetThemeContents("VOTE", "POLL_QUESTION");
                    questionAnchor.HRef = "";
                }


                DataTable _choiceRow = _dtPoll.Copy();
                foreach (DataRow drr in _choiceRow.Rows)
                {
                    if (Convert.ToInt32(drr["PollID"]) != Convert.ToInt32(pollId))
                    {
                        drr.Delete();
                    }
                    else
                    {
                        if (!drr["MimeType"].IsNullOrEmptyDBField())
                        {
                            decimal currentAspect = GetImageAspect(drr["MimeType"]);
                            if (currentAspect > MaxImageAspect)
                            {
                                MaxImageAspect = currentAspect;
                            }
                        }
                    }
                }
                

                polloll.DataSource = _choiceRow;
                _canVote = CanVote(pollId);
                bool isPollClosed = IsPollClosed(pollId);
                bool isNotVoted = IsNotVoted(pollId);
                // Poll voting is bounded - you can't see results before voting in each poll
                if (isBound)
                {
                    int voteCount = _dtPollGroup.Rows.Cast<DataRow>().Count(dr => !IsNotVoted(dr["PollID"]) && !IsPollClosed(dr["PollID"]));

                    if (!isPollClosed && voteCount >= PollNumber)
                    {
                        _showResults = true;
                    }
                }
                else
                {
                    if (!isClosedBound && PageContext.BoardSettings.AllowUsersViewPollVotesBefore)
                    {
                        _showResults = true;
                    }
                }

                // The poll expired. We show results  
                if (isClosedBound && isPollClosed)
                {
                    _showResults = true;
                }
      
                polloll.DataBind();

                // Clear the fields after the child repeater is bound
                _showResults = false;
                _canVote = false;

                // Add confirmations to delete buttons
                ThemeButton removePollAll = item.FindControlRecursiveAs<ThemeButton>("RemovePollAll");
                removePollAll.Attributes["onclick"] = String.Format(
               "return confirm('{0}');", PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
                removePollAll.Visible = CanRemovePollCompletely(pollId);
               
                ThemeButton removePoll = item.FindControlRecursiveAs<ThemeButton>("RemovePoll");
                removePoll.Attributes["onclick"] = String.Format(
                         "return confirm('{0}');", PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE"));
                removePoll.Visible = CanRemovePoll(pollId);

                // Poll warnings section
                bool soon;
                bool showWarningsRow = false; 
                int? daystorun = DaysToRun(pollId, out soon);
               
                Label pollVotesLabel = item.FindControlRecursiveAs<Label>("PollVotesLabel");
                bool cvote = CanVote(pollId);
                if (cvote)
                {
                    if (isBound && PollNumber > 1 && PollNumber >= _dtVotes.Rows.Count)
                    {
                        pollVotesLabel.Text = PageContext.Localization.GetText("POLLEDIT", "POLLGROUP_BOUNDWARN");
                    }
                    if (!PageContext.BoardSettings.AllowUsersViewPollVotesBefore)
                    {

                        if (!PageContext.IsGuest)
                        {
                            pollVotesLabel.Text += PageContext.Localization.GetText("POLLEDIT", "POLLRESULTSHIDDEN");
                        }
                        else
                        {
                            pollVotesLabel.Text += PageContext.Localization.GetText("POLLEDIT",
                                                                                         "POLLRESULTSHIDDEN_GUEST");
                        }
                    }
                }

                if (PageContext.IsGuest)
                {
                    Label guestOptionsHidden = item.FindControlRecursiveAs<Label>("GuestOptionsHidden");
                    if (!cvote &&  (!PageContext.BoardSettings.AllowGuestsViewPollOptions))
                    {
                        guestOptionsHidden.Text = PageContext.Localization.GetText("POLLEDIT",
                         "POLLOPTIONSHIDDEN_GUEST");
                        guestOptionsHidden.Visible = true;
                        showWarningsRow = true;
                    }
                    if (!PageContext.ForumPollAccess)
                    {
                        guestOptionsHidden.Text += PageContext.Localization.GetText("POLLEDIT",
                                                                                    "POLL_NOPERM_GUEST");
                        guestOptionsHidden.Visible = true;
                        showWarningsRow = true;
                    }
                }

                pollVotesLabel.Visible = isBound || (PageContext.BoardSettings.AllowUsersViewPollVotesBefore
                                             ? false
                                             : (isNotVoted || (daystorun == null)));
                if (pollVotesLabel.Visible)
                {
                    showWarningsRow = true;
                }

                if (!isNotVoted && (PageContext.ForumPollAccess || (PageContext.BoardVoteAccess && (BoardId > 0 || EditBoardId > 0))))
                 {
                     Label alreadyVotedLabel = item.FindControlRecursiveAs<Label>("AlreadyVotedLabel");
                     alreadyVotedLabel.Text = PageContext.Localization.GetText("POLLEDIT", "POLL_VOTED");
                     showWarningsRow = alreadyVotedLabel.Visible = true;
                 }
                if (daystorun > 0)
                {
                    Label pollWillExpire = item.FindControlRecursiveAs<Label>("PollWillExpire");
                    if (!soon)
                    {
                        pollWillExpire.Text = PageContext.Localization.GetTextFormatted("POLL_WILLEXPIRE",
                                                                                             daystorun);
                    }
                    else
                    {
                        pollWillExpire.Text = PageContext.Localization.GetText("POLLEDIT", "POLL_WILLEXPIRE_HOURS");
                    }
                    showWarningsRow = pollWillExpire.Visible = true;
                }
                else if (daystorun == 0)
                {
                    Label pollExpired = item.FindControlRecursiveAs<Label>("PollExpired");
                    pollExpired.Text = PageContext.Localization.GetText("POLLEDIT", "POLL_EXPIRED");
                    showWarningsRow = pollExpired.Visible = true;
                }
             
                item.FindControlRecursiveAs<HtmlTableRow>("PollInfoTr").Visible = showWarningsRow;
                
                DisplayButtons();
           }

            if (item.ItemType == ListItemType.Footer)
            {
                var pgcr = item.FindControlRecursiveAs<HtmlTableRow>("PollGroupCommandRow");
                pgcr.Visible = HasOwnerExistingGroupAccess() && ShowButtons;
               
                if (pgcr.Visible)
                {
                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroup").Attributes["onclick"] = String.Format(
                        "return confirm('{0}');",
                        PageContext.Localization.GetText("POLLEDIT", "ASK_POLLGROUP_DELETE"));

                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroupAll").Attributes["onclick"] = String.Format(
                        "return confirm('{0}');",
                        PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_ALL"));

                    item.FindControlRecursiveAs<ThemeButton>("RemoveGroupEverywhere").Attributes["onclick"] = String.
                        Format(
                            "return confirm('{0}');",
                            PageContext.Localization.GetText("POLLEDIT", "ASK_POLLROUP_DELETE_EVR"));
                }
            }
        }


        private static void DisplayButtons()
        {

           // PollGroup.FindControlRecursiveAs<HtmlTableRow>("PollCommandRow").Visible = ShowButtons;

        }

        /// <summary>
        /// PollGroup_ItemCommand
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void PollGroup_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName == "new" && PageContext.ForumVoteAccess)
            {
                YafBuildLink.Redirect(
                   ForumPages.polledit,
                   "{0}",
                  ParamsToSend()
                   );
            }
            if (e.CommandName == "edit" && PageContext.ForumVoteAccess)
            {
                YafBuildLink.Redirect(
                    ForumPages.polledit,
                    "{0}&p={1}",
                     ParamsToSend(),
                    e.CommandArgument.ToString());
                
            }
            if (e.CommandName == "remove" && PageContext.ForumVoteAccess)
            {
               // ChangePollShowStatus(false);

                if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
                {
                    DB.poll_remove(PollGroupId, e.CommandArgument,BoardId, false,false);
                    ReturnToPage();
                    // BindData();
                }

            }
            if (e.CommandName == "removeall" && PageContext.ForumVoteAccess)
            {
                if (e.CommandArgument != null && e.CommandArgument.ToString() != string.Empty)
                {
                    DB.poll_remove(PollGroupId, e.CommandArgument, BoardId, true, false);
                    ReturnToPage();
                    // BindData();
                }
            }
    
            if (e.CommandName == "removegroup" && PageContext.ForumVoteAccess)
            {
              
                    DB.pollgroup_remove(PollGroupId, TopicId, ForumId, CategoryId, BoardId, false, false);
                    ReturnToPage();
                    // BindData();
              
            }
            if (e.CommandName == "removegroupall" && PageContext.ForumVoteAccess)
            {
             
                    DB.pollgroup_remove(PollGroupId, TopicId, ForumId, CategoryId, BoardId, true, false);
                    ReturnToPage();
                    //BindData();
              
            }
            if (e.CommandName == "removegroupevery" && PageContext.ForumVoteAccess)
            {

                    DB.pollgroup_remove(PollGroupId, TopicId, ForumId, CategoryId, BoardId, false, true);
                    ReturnToPage();
                // BindData();
            }
        }
        /// <summary>
        /// Returns an image width|height ratio. 
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
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

        protected int GetImageHeight(object mimeType)
        {
            string[] attrs = mimeType.ToString().Split('!')[1].Split(';');
             return Convert.ToInt32(attrs[1]);
        
        }


        protected void Poll_OnItemDataBound(object source, RepeaterItemEventArgs e)
        {

            RepeaterItem item = e.Item;
            DataRowView drowv = (DataRowView)e.Item.DataItem;
            HtmlTableRow trow = item.FindControlRecursiveAs<HtmlTableRow>("VoteTr");
           
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                // Voting link 
                MyLinkButton myLinkButton = item.FindControlRecursiveAs<MyLinkButton>("MyLinkButton1");
                string pollId = drowv.Row["PollID"].ToString();

                myLinkButton.Enabled = _canVote;
                myLinkButton.ToolTip = PageContext.Localization.GetText("POLLEDIT", "POLL_PLEASEVOTE");
                myLinkButton.Visible = true;

               // Poll Choice image
               HtmlImage choiceImage = item.FindControlRecursiveAs<HtmlImage>("ChoiceImage");
               HtmlAnchor choiceAnchor = item.FindControlRecursiveAs<HtmlAnchor>("ChoiceAnchor");
              
               // Don't render if it's a standard image
               if (!drowv.Row["ObjectPath"].IsNullOrEmptyDBField())
                {
                    choiceAnchor.Attributes["rel"] = "lightbox-group" + Guid.NewGuid().ToString().Substring(0, 5);
                    choiceAnchor.HRef = drowv.Row["ObjectPath"].IsNullOrEmptyDBField()
                                            ? GetThemeContents("VOTE", "POLL_CHOICE")
                                            : HtmlEncode(drowv.Row["ObjectPath"].ToString());
                    choiceAnchor.Title = drowv.Row["ObjectPath"].ToString();

                    choiceImage.Alt = HtmlEncode(drowv.Row["ObjectPath"].ToString());
                    choiceImage.Src = HtmlEncode(drowv.Row["ObjectPath"].ToString());
                
                  
                    if (!(drowv.Row["MimeType"]).IsNullOrEmptyDBField())
                    {
                        decimal aspect = GetImageAspect(drowv.Row["MimeType"]);
                       
                        
                        // hardcoded - bad
                        const int imageWidth = 80;
                        choiceImage.Attributes["style"] = String.Format("width:{0}px; height:{1}px;", imageWidth, choiceImage.Width / aspect);
                        // reserved to get equal row heights
                        String height = (MaxImageAspect * choiceImage.Width).ToString();
                        trow.Attributes["style"] = String.Format("height:{0}px;", height);
                      
                    }
                   

                }
               else
               {
                   choiceImage.Alt = PageContext.Localization.GetText("POLLEDIT", "POLL_PLEASEVOTE");
                   choiceImage.Src =  GetThemeContents("VOTE", "POLL_CHOICE");
                   choiceAnchor.HRef = "";
               }

               item.FindControlRecursiveAs<Panel>("MaskSpan").Visible = !_showResults; 
               item.FindControlRecursiveAs<Panel>("resultsSpan").Visible =
               item.FindControlRecursiveAs<Panel>("VoteSpan").Visible = _showResults;
               
            }
        }


        /// <summary>
        /// The poll_ item command.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Poll_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "vote" &&  e.CommandArgument != null && ((PageContext.ForumVoteAccess && TopicId > 0) || (PageContext.BoardVoteAccess && BoardId > 0)))
            {
                if (!CanVote(Convert.ToInt32(e.CommandArgument)))
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("WARN_ALREADY_VOTED"));
                    return;
                }

                if (IsLocked)
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("WARN_TOPIC_LOCKED"));
                    return;
                }
               
                    foreach (DataRow drow  in _dtPoll.Rows)
                    {
                        if ((int)drow["ChoiceID"] == Convert.ToInt32(e.CommandArgument))
                        {
                            if (IsPollClosed(Convert.ToInt32(drow["PollID"])))
                            {
                                PageContext.AddLoadMessage(PageContext.Localization.GetText("WARN_POLL_CLOSED"));
                                return;
                            }
                            break;
                        }
                    }


                object userID = null;
                object remoteIP = null;

                if (PageContext.BoardSettings.PollVoteTiedToIP)
                {
                    remoteIP = IPHelper.IPStrToLong(Request.ServerVariables["REMOTE_ADDR"]).ToString();
                }

                if (!PageContext.IsGuest)
                { 
                    userID = PageContext.PageUserID;
                }

                DB.choice_vote(e.CommandArgument, userID, remoteIP);

                // save the voting cookie...
                var c = new HttpCookie(VotingCookieName(Convert.ToInt32(e.CommandArgument)), e.CommandArgument.ToString())
                            {Expires = DateTime.UtcNow.AddYears(1)};
                Response.Cookies.Add(c);
                string msg = PageContext.Localization.GetText("INFO_VOTED");

                if (isBound && PollNumber > 1 && PollNumber >= _dtVotes.Rows.Count && (!PageContext.BoardSettings.AllowUsersViewPollVotesBefore))
                {
                    msg += PageContext.Localization.GetText("POLLGROUP_BOUNDWARN");
                }

                PageContext.AddLoadMessage(msg);
                BindData();
            }
        }

        /// <summary>
        /// Returns user to the original call page. 
        /// </summary>
        private void ReturnToPage()
        {
            // We simply return here to the page where is the control. It can be made other way.
            if (TopicId > 0)
            {
                YafBuildLink.Redirect(
                          ForumPages.posts,
                          "t={0}",
                         TopicId);
            }
            
            if (EditMessageId > 0)
            {
                YafBuildLink.Redirect(
                    ForumPages.postmessage,
                    "m={0}",
                    EditMessageId);
            }

            if (ForumId > 0)
            {
                YafBuildLink.Redirect(
                    ForumPages.topics,
                    "f={0}",
                    ForumId);
            }
            
            if (EditForumId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.admin_editforum,
                    "f={0}",
                    ForumId);
            }
            
            if (CategoryId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.forum,
                    "c={0}",
                    CategoryId);
            }
            
            if (EditCategoryId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.admin_editcategory,
                    "c={0}",
                    EditCategoryId);
            }
            
            if (BoardId > 0)
            {

                YafBuildLink.Redirect(
                    ForumPages.forum);
            }
            
            if (EditBoardId > 0)
            {

               YafBuildLink.Redirect(
                    ForumPages.admin_editboard,
                    "b={0}",
                    EditBoardId);
            }
         
                YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
           
        }

        /// <summary>
        /// The change poll show status.
        /// </summary>
        /// <param name="newStatus">
        /// The new status.
        /// </param>
        protected void ChangePollShowStatus(bool newStatus)
        {
          /*  var pollRow = (HtmlTableRow)FindControl(String.Format("PollRow{0}", 1));

            if (pollRow != null)
            {
                pollRow.Visible = newStatus;
            }*/
        }

        /// <summary>
        /// Checks if a user can create poll.
        /// </summary>
        /// <returns></returns>
        protected bool CanCreatePoll()
        {
            return (PollNumber < PageContext.BoardSettings.AllowedPollNumber) && 
                (PageContext.BoardSettings.AllowedPollChoiceNumber > 0)  && 
                HasOwnerExistingGroupAccess() 
                && (PollGroupId >= 0);
        }

        /// <summary>
        /// Checks if user can edit a poll
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        protected bool CanEditPoll(object pollId)
        {
            if (!PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return ShowButtons &&
                       (PageContext.IsAdmin || PageContext.IsForumModerator ||
                        (PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                         PollHasNoVotes(pollId) || (!IsPollClosed(pollId))));
            }

            return ShowButtons &&
                   (PageContext.IsAdmin || PageContext.IsForumModerator ||
                    PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) && (!IsPollClosed(pollId)));
        }

        /// <summary>
        /// Checks if a user can delete poll without deleting it from database
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        protected bool CanRemovePoll(object pollId)
        {
            return ShowButtons && 
                (PageContext.IsAdmin || 
                PageContext.IsForumModerator ||
                (PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"])));
        }

        /// <summary>
        /// Checks if a user can delete poll completely
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        protected bool CanRemovePollCompletely(object pollId)
        {
            if (!PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return ShowButtons &&
                      (PageContext.IsAdmin || PageContext.IsModerator ||
                      ((PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                        PollHasNoVotes(pollId))));
            }

            return PollHasNoVotes(pollId) && ShowButtons &&
                   (PageContext.IsAdmin ||
                    PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]));
        }

        /// <summary>
        /// Checks if a user can remove all polls in a group 
        /// </summary>
        /// <returns></returns>
        protected bool CanRemoveGroup()
        {
            bool hasNoVotes = true;

            foreach (DataRow dr in _dtPoll.Rows)
            {
                if (Convert.ToInt32(dr["Votes"]) > 0)
                {
                    hasNoVotes = false;
                }
            }

            if (!PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return ShowButtons &&
                       (PageContext.IsAdmin || PageContext.IsForumModerator ||
                        ((PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                          hasNoVotes)));
            }

            return ShowButtons &&
                   (PageContext.IsAdmin || PageContext.IsForumModerator ||
                    ((PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]))));
        }

        /// <summary>
        /// Checks if  a user can remove all polls in a group completely.
        /// </summary>
        /// <returns></returns>
        protected bool CanRemoveGroupCompletely()
        {
            bool hasNoVotes = true;
            foreach (DataRow dr in _dtPoll.Rows)
            {
                if (Convert.ToInt32(dr["Votes"]) > 0)
                {
                    hasNoVotes = false;
                }
            }
           
            if (!PageContext.BoardSettings.AllowPollChangesAfterFirstVote)
            {
                return ShowButtons &&
                      (PageContext.IsAdmin || 
                       ((PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]) &&
                        hasNoVotes)));
            }

            return ShowButtons &&
                   (PageContext.IsAdmin ||
                    PageContext.PageUserID == Convert.ToInt32(_dtPollGroup.Rows[0]["GroupUserID"]));
        }

        /// <summary>
        /// Checks if a user can delete group from all places but not completely
        /// </summary>
        /// <returns></returns>
        protected bool CanRemoveGroupEverywhere()
        {
            return ShowButtons && (PageContext.IsAdmin);
        }

        /// <summary>
        /// Checks if a poll has votes.
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        private bool PollHasNoVotes(object pollId)
        {
            return _dtPoll.Rows.Cast<DataRow>().Where(dr => Convert.ToInt32(dr["PollID"]) == Convert.ToInt32(pollId)).All(dr => Convert.ToInt32(dr["Votes"]) <= 0);
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
            ((ThemeButton)sender).Attributes["onclick"] = String.Format(
              "return confirm('{0}');", PageContext.Localization.GetText("POLLEDIT","ASK_POLL_DELETE"));
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
            ((ThemeButton)sender).Attributes["onclick"] = String.Format(
              "return confirm('{0}');", PageContext.Localization.GetText("POLLEDIT", "ASK_POLL_DELETE_ALL"));
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {   
               _dataBound = true;
               PollNumber = 0;
               _dtPoll = DB.pollgroup_stats(PollGroupId);

               // if the page user can cheange the poll. Only a group owner, forum moderator  or an admin can do it.   
               _canChange = (Convert.ToInt32(_dtPoll.Rows[0]["GroupUserID"]) == PageContext.PageUserID) ||
                       PageContext.IsAdmin || PageContext.IsForumModerator;

            // check if we should hide pollgroup repeater when a message is posted
            if (Parent.Page.ClientQueryString.Contains("postmessage"))
            {
                PollGroup.Visible = (((EditMessageId > 0)) && (!_canChange)) || _canChange;
            }
            else
            {
                PollGroup.Visible = true;
            }

            _dtPollGroup = _dtPoll.Copy();

            // TODO: repeating code - move to Utils
            // Remove repeating PollID values   
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            foreach (DataRow drow in _dtPollGroup.Rows)
            {
                if (hTable.Contains(drow["PollID"]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow["PollID"], string.Empty);
            }
            foreach (DataRow dRow in duplicateList)
                _dtPollGroup.Rows.Remove(dRow);
                _dtPollGroup.AcceptChanges();

                 // frequently used
                 PollNumber = _dtPollGroup.Rows.Count;

                if (_dtPollGroup.Rows.Count > 0)
                {
                    // Check if the user is already voted in polls in the group 
                    object userId = null;
                    object remoteIp = null;

                   if (PageContext.BoardSettings.PollVoteTiedToIP)
                   {
                       remoteIp = IPHelper.IPStrToLong(Request.UserHostAddress).ToString();
                   }

                    if (!PageContext.IsGuest)
                    {
                        userId = PageContext.PageUserID;
                    }

                    _dtVotes = DB.pollgroup_votecheck(PollGroupId, userId, remoteIp);

                    isBound = (Convert.ToInt32(_dtPollGroup.Rows[0]["IsBound"]) == 2);
                    isClosedBound = (Convert.ToInt32(_dtPollGroup.Rows[0]["IsClosedBound"]) == 4);

                    PollGroup.DataSource = _dtPollGroup;

                    // we hide new poll row if a poll exist
                    NewPollRow.Visible = false;
                    ChangePollShowStatus(true);

                }

                DataBind();
        }

    }
}
