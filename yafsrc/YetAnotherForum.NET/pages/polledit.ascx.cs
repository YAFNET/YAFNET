using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Controls;


namespace YAF.Pages
{

    public partial class polledit : ForumPage
    {
        /// <summary>
        /// Table with choices
        /// </summary>
        protected DataTable choices;

        /// <summary>
        /// Table with choices
        /// </summary>
        protected DataRow _topicInfo;

       // protected int? pollID;

        protected int daysPollExpire = 0;

        protected bool topicUnapproved;

        protected  int? topicId;

        protected int? returnForum;

        protected int? editCategoryId;

        protected int? editBoardId;

        protected int? editForumId;
        
        protected int? editTopicId;

        protected int? editMessageId;

        protected int? forumId;

        protected int? categoryId;

        protected int? boardId; 

        protected DateTime? datePollExpire = null;

        protected int? PollID
        { get; set;} 

    /// <summary>
    /// Initializes a new instance of the ReportPost class.
    /// </summary>
    public polledit()
      : base("POLLEDIT")
    {
    }
        private void InitializeVariables()
        {
          
            PageContext.QueryIDs = new QueryStringIDHelper(new string[14]{"p","ra","ntp","t","e","em","m","f", "ef","c","ec","b","eb","rf"});
           
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
                    this.editMessageId = (int) this.PageContext.QueryIDs["m"];
                }
                        if (this.editMessageId == null)
                        {
                                
                                if (this.PageContext.QueryIDs.ContainsKey("ef"))
                                {
                                    this.categoryId = (int) this.PageContext.QueryIDs["ef"];
                                }
                                if (this.editForumId == null)
                                {
                                    
                                    if (this.PageContext.QueryIDs.ContainsKey("c"))
                                    {
                                        this.categoryId = (int) this.PageContext.QueryIDs["c"];
                                    }
                                    if (this.categoryId == null)
                                    {
                                        
                                        if (this.PageContext.QueryIDs.ContainsKey("ec"))
                                        {
                                            this.editCategoryId = (int) this.PageContext.QueryIDs["ec"];
                                        }
                                        if (this.editCategoryId == null)
                                        {
                                            
                                            if (this.PageContext.QueryIDs.ContainsKey("b"))
                                            {
                                                this.boardId = (int) this.PageContext.QueryIDs["b"];
                                            }
                                            if (this.boardId == null)
                                            {
                                                if (this.PageContext.QueryIDs.ContainsKey("eb"))
                                                {
                                                    this.editBoardId = (int) this.PageContext.QueryIDs["eb"];
                                                }

                                            }
                                      
                               
                            }
                        }
                    }
                }

                    CheckAccess();
                    // handle poll
                    if (this.PageContext.QueryIDs.ContainsKey("p"))
                    {
                        // edit existing poll
                        PollID = (int) this.PageContext.QueryIDs["p"];
                        InitPollUI(PollID);
                    }
                    else
                    {
                        // new poll
                        this.PollRow1.Visible = true;
                        InitPollUI(null);

                    }
            }
            else
            {
                YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
            }
          
            
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            this.PollExpire.Attributes.Add("style", "width:50px");

            InitializeVariables();

            if (int.TryParse(this.PollExpire.Text.Trim(), out daysPollExpire))
            {
                datePollExpire = DateTime.UtcNow.AddDays(daysPollExpire);
            }
            if (!IsPostBack)
            {
                this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                if (this.categoryId > 0)
                {
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.categoryId));
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
                        this._topicInfo["Topic"].ToString(),
                        YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.topicId));
                }
                if (this.editMessageId > 0)
                {
                    this.PageLinks.AddLink(
                        this._topicInfo["Topic"].ToString(),
                        YafBuildLink.GetLink(ForumPages.postmessage, "m={0}", this.editMessageId));
                }
               
                this.PageLinks.AddLink(this.GetText("POLLEDIT","TITLE"), string.Empty);


            }            // Admin can attach existing group if it's a new poll - this.pollID <= 0
            // The functionality is temporarily disabled
            if (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
            {
                DataTable dt = DB.pollgroup_list(this.PageContext.PageUserID, null, this.PageContext.PageBoardID);
                DataRow ndr = dt.NewRow();
                ndr["PollGroupID"] = -1;
                ndr["Question"] = string.Empty;
                dt.Rows.InsertAt(ndr, 0);
                dt.AcceptChanges();
                this.PollGroupListDropDown.DataSource = dt;
                this.PollGroupListDropDown.DataValueField = "PollGroupID";
                this.PollGroupListDropDown.DataTextField = "Question";

                this.PollGroupListDropDown.DataBind();
                this.PollGroupList.Visible = true;


            }



        }


        /// <summary>
        /// The init poll ui.
        /// </summary>
        /// <param name="PollID">
        /// The PollID.
        /// </param>
        private void InitPollUI(int? pollID)
        {
        //  this.RemovePoll.CommandArgument = currentRow["PollID"].ToString();
                 
                // we should get the schema anyway
                choices = DB.poll_stats(pollID);
                choices.Columns.Add("ChoiceOrderID", typeof(int));
                
                // First existing values
                int existingRowsCount = 1;
                int allExistingRowsCount = choices.Rows.Count;

                // we edit existing poll 
                if (choices.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(choices.Rows[0]["UserID"]) != this.PageContext.PageUserID) && (!PageContext.IsAdmin) && (!PageContext.IsForumModerator))
                    {
                        YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
                    }

                     if (Convert.ToInt32(choices.Rows[0]["IsBound"]) > 0 )
                     {
                         IsBoundCheckBox.Checked = true;
                     }

                    this.Question.Text = choices.Rows[0]["Question"].ToString();

                    if (choices.Rows[0]["Closes"] != DBNull.Value)
                    {
                        TimeSpan closing = (DateTime) choices.Rows[0]["Closes"] - DateTime.UtcNow;

                        this.PollExpire.Text = SqlDataLayerConverter.VerifyInt32(closing.TotalDays + 1).ToString();
                    }
                    else
                    {
                        this.PollExpire.Text = null;
                    }

                   
                    foreach (DataRow choiceRow in choices.Rows)
                    {
                        choiceRow["ChoiceOrderID"] = existingRowsCount;
                        
                        existingRowsCount++;
                    }

                    // Get isBound value directly
                    if (Convert.ToInt32(choices.Rows[0]["IsBound"]) == 2)
                    {
                        IsBoundCheckBox.Checked = true;
                        

                    }
                }
                else
                {
                    // Get isBound value using page variables. They are initialized here.

                    int pgidt = 0;
                    // If a topic poll is edited or new topic created
                    if (this.topicId > 0 && _topicInfo !=null)
                    {
                       // topicid should not be null here 
                       if (_topicInfo["PollID"] != DBNull.Value)
                        {
                            pgidt = (int)_topicInfo["PollID"];
                            int ff = Convert.ToInt32(DB.pollgroup_stats((int) pgidt).Rows[0]["IsBound"]);
                            if (Convert.ToInt32(DB.pollgroup_stats((int)pgidt).Rows[0]["IsBound"]) == 2)
                            {
                                IsBoundCheckBox.Checked = true;
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
                        pgidt = (int)DB.category_listread(this.PageContext.PageBoardID, this.PageContext.PageUserID, this.categoryId).Rows[0]["PollGroupID"];

                    }

                    if (pgidt > 0)
                    {
                        if (Convert.ToInt32(DB.pollgroup_stats(pgidt).Rows[0]["IsBound"]) == 2)
                        {
                            IsBoundCheckBox.Checked = true;
                        }
                    }

                    // clear the fields...
                    this.PollExpire.Text = string.Empty;
                    this.Question.Text = string.Empty;
                }


                int dummyRowsCount = PageContext.BoardSettings.AllowedPollChoiceNumber - allExistingRowsCount - 1;
                for (int i = 0; i <= dummyRowsCount; i++)
                {
                    DataRow drow = choices.NewRow();
                    drow["ChoiceOrderID"] = existingRowsCount + i;
                    choices.Rows.Add(drow);
                }
                this.ChoiceRepeater.DataSource = choices;
                this.ChoiceRepeater.DataBind();
                this.ChoiceRepeater.Visible = true;
           
                this.ChangePollShowStatus(true);
                IsBound.Visible = true;

           
        }

        private void CheckAccess()
        {
            if (this.boardId > 0 || this.categoryId > 0)
            {
                // invalid category
                bool categoryVars = this.categoryId > 0 &&
                                    (this.topicId > 0 || this.editTopicId > 0 || this.editMessageId > 0 ||
                                     this.editForumId > 0 || this.editBoardId > 0 || this.forumId > 0 ||
                                     this.boardId > 0);
                // invalid board vars
                bool boardVars = this.boardId > 0 &&
                                 (this.topicId > 0 || this.editTopicId > 0 || this.editMessageId > 0 ||
                                  this.editForumId > 0 || this.editBoardId > 0 || this.forumId > 0 ||
                                  this.categoryId > 0);
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

          /*  this.PollGroup.FindControlRecursiveAs<ThemeButton>("CreatePollRow").Visible = !newStatus;
            this.PollGroup.FindControlRecursiveAs<ThemeButton>("RemovePollRow").Visible = newStatus;
            this.PollGroup.FindControlRecursiveAs<ThemeButton>("PollRowExpire").Visible = newStatus;
            this.PollGroup.FindControlRecursiveAs<Repeater>("Poll").Visible = newStatus; */

          /*  var pollRow = (HtmlTableRow)this.FindControl(String.Format("PollRow{0}", 1));

            if (pollRow != null)
            {
                pollRow.Visible = newStatus;
            } */
        }

        protected bool IsInputVerified()
        {
           // if (this.PollRow1.Visible)
           // {
       
             if (Convert.ToInt32(this.PollGroupListDropDown.SelectedIndex) <= 0)
                {
                if (this.Question.Text.Trim().Length == 0)
                {
                    YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT", "NEED_QUESTION"));
                    return false;
                }

                int notNullcount = 0;
                foreach (RepeaterItem ri in this.ChoiceRepeater.Items)
                {
                    if (!string.IsNullOrEmpty(((TextBox)ri.FindControl("PollChoice")).Text.Trim()))
                    {
                        notNullcount++;
                    }
                }

                if (notNullcount < 2)
                {
                    YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT", "NEED_CHOICES"));
                    return false;
                }

               int dateVerified = 0;
               if (!int.TryParse(this.PollExpire.Text.Trim(), out dateVerified) && (!String.IsNullOrEmpty(this.PollExpire.Text.Trim())))
               {
                   YafContext.Current.AddLoadMessage(YafContext.Current.Localization.GetText("POLLEDIT","EXPIRE_BAD"));
                   return false;
               }
                
            }
            return true;
        }

        /// <summary>
        /// The get poll id.
        /// </summary>
        /// <returns>
        /// The get poll id.
        /// </returns>
        private bool? GetPollID()
        {

            if (int.TryParse(this.PollExpire.Text.Trim(), out daysPollExpire))
            {
                datePollExpire = DateTime.UtcNow.AddDays(daysPollExpire);
            }
            // this.PollGroup.FindControlRecursiveAs<ThemeButton>("RemovePoll").Visible =
            // we are just using existing poll
            if (this.PollID != null)
             {

                // int pollID = Convert.ToInt32(this.RemovePoll.CommandArgument);
                 DB.poll_update(PollID, this.Question.Text, datePollExpire, this.IsBoundCheckBox.Checked);

                 foreach (RepeaterItem ri in ChoiceRepeater.Items)
                 {
                     string choice = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
                     string chid = ((HiddenField)ri.FindControl("PollChoiceID")).Value;

                     if (string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
                     {
                         // add choice
                         DB.choice_add(PollID, choice);
                     }
                     else if (!string.IsNullOrEmpty(chid) && !string.IsNullOrEmpty(choice))
                     {
                         // update choice
                         DB.choice_update(chid, choice);
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
                    int result = DB.pollgroup_attach((int?)Convert.ToInt32(this.PollGroupListDropDown.SelectedValue),
                                                     this.topicId, this.forumId, this.categoryId, this.boardId);
                    if (result == 1)
                    {
                        PageContext.LoadMessage.Add(GetText("POLLEDIT", "POLLGROUP_ATTACHED"));
                    }
                    return true;
                }
                
                 else
                 {


                     // vzrus: always one in the current code - a number of  polls for a topic
                     int questionsTotal = 1;

                     System.Collections.Generic.List<PollSaveList> pollList =
                         new System.Collections.Generic.List<PollSaveList>(questionsTotal);
                     string[,] rawChoices = new string[3,ChoiceRepeater.Items.Count];
                     int j = 0;
                     foreach (RepeaterItem ri in ChoiceRepeater.Items)
                     {
                         rawChoices[0,j] = ((TextBox)ri.FindControl("PollChoice")).Text.Trim();
                         rawChoices[1,j] = null;
                         rawChoices[2,j] = null;
                         j++;
                     }

                     int? realTopic = this.topicId;

                     if (this.topicId == null)
                     {
                         realTopic = this.editTopicId;
                     }


                     if (datePollExpire == null && (!String.IsNullOrEmpty(this.PollExpire.Text.Trim())))
                     {

                         datePollExpire = DateTime.UtcNow.AddDays(Convert.ToInt32(this.PollExpire.Text.Trim()));
                     }

                     pollList.Add(new PollSaveList(this.Question.Text,
                                                   rawChoices,
                                                   (DateTime?)datePollExpire, this.PageContext.PageUserID, realTopic,
                                                   this.forumId, this.categoryId, this.boardId, null, null, IsBoundCheckBox.Checked));
                     DB.poll_save(pollList);
                     return true;
                 }

             }
           

            return false; // A poll was not created for this topic.
        }

        protected void SavePoll_Click(object  sender, EventArgs eventArgs)
        {

            if (PageContext.ForumPollAccess && IsInputVerified())
            {
              
                if (GetPollID() == true)
                {
                    ReturnToPage();
                }
            }

        }


        protected void Cancel_Click(object sender, EventArgs eventArgs)
        {
            ReturnToPage();
        }

        private  void ReturnToPage()
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

            ParamsToSend(out retliterals, out retvalue);

            switch (retliterals)
            {
                case "t":
                    YafBuildLink.Redirect(
                        ForumPages.posts,
                        "t={0}",
                       retvalue);
                    break;

                case "em":

                    YafBuildLink.Redirect(
                        ForumPages.postmessage,
                        "m={0}",
                        retvalue);
                    break;

                case "f":

                    YafBuildLink.Redirect(
                        ForumPages.topics,
                        "f={0}",
                        retvalue);
                    break;
                case "ef":
                    YafBuildLink.Redirect(
                        ForumPages.admin_editforum,
                        "f={0}",
                        retvalue);
                    break;
                case "c":
                    YafBuildLink.Redirect(
                        ForumPages.forum,
                        "c={0}",
                       retvalue);
                    break;
                case "ec":
                    YafBuildLink.Redirect(
                        ForumPages.admin_editcategory,
                        "c={0}",
                       retvalue);
                    break;
                case "b":
                    YafBuildLink.Redirect(
                        ForumPages.forum);
                    break;
                case "eb":
                    YafBuildLink.Redirect(
                           ForumPages.admin_editboard,
                           "b={0}",
                          retvalue);
                    break;
                default:
                    YafBuildLink.RedirectInfoPage(InfoMessage.Invalid);
                    break;


            }


        }

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
 




    }
}
