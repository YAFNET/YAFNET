namespace YAF.Pages
{
    using System;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;

    /// <summary>
    /// Summary description for members.
    /// </summary>
    public partial class messagehistory : ForumPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="members"/> class.
        /// </summary>
        public messagehistory()
            : base("MESSAGEHISTORY")
        {
        }

        /// <summary>
        /// To save messageid value.
        /// </summary>
        private int messageID = 0;

        /// <summary>
        /// To save forumid value.
        /// </summary>
        private int forumID = 0;


        /// <summary>
        /// To save originalRow value.
        /// </summary>
        private System.Data.DataTable originalRow;

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(Request.QueryString["m"]))
            {            

                if (!Int32.TryParse(Request.QueryString["m"], out this.messageID))
                {
                    Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
                }
                this.ReturnBtn.Visible = true;
              
            }
            if (!String.IsNullOrEmpty(Request.QueryString["f"]))
            {
                // We check here if the user have access to the option
                if (PageContext.IsGuest)
                {
                    Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=4"));
                }

                if (!Int32.TryParse(Request.QueryString["f"], out this.forumID))
                {
                    Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.error, "Incorrect forum value: {0}", this.forumID));
                }
                this.ReturnModBtn.Visible = true;
            }

            originalRow = DB.message_secdata(this.messageID, this.PageContext.PageUserID);

            if (originalRow.Rows.Count <= 0)
            {
                Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
            }

            if (!IsPostBack)
            {
                this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

                BindData();
            }

        }

        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
           // Fill revisions list repeater. We set clean-up period to 365 days.
            this.RevisionsList.DataSource = DB.messagehistory_list(this.messageID, this.PageContext.BoardSettings.MessageHistoryDaysToLog, true);
          
           // Fill current message repeater
           this.CurrentMessageRpt.Visible = true;
           this.CurrentMessageRpt.DataSource = DB.message_secdata(this.messageID, this.PageContext.PageUserID);           
          
            DataBind();
        }

        protected void ReturnBtn_OnClick(object sender, EventArgs e)
        {

            // Redirect to the changed post
            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", this.messageID));

        }
        protected void ReturnModBtn_OnClick(object sender, EventArgs e)
        {

            // Redirect to the changed post
            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.moderate_reportedposts, "f={0}", this.forumID));

        }
     
    }
}
