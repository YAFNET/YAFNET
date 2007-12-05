using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.moderate
{
    /// <summary>
    /// Summary description for _default.
    /// </summary>
    public partial class reportedposts : YAF.Classes.Base.ForumPage
    {

        public reportedposts()
            : base("MODERATE_FORUM")
        {
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!PageContext.IsModerator || !PageContext.ForumModeratorAccess)
                YafBuildLink.AccessDenied();

            if (!IsPostBack)
            {  
                PageLinks.AddLink(PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum));
                PageLinks.AddLink(GetText("MODERATE_DEFAULT", "TITLE"), YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.moderate_index));
                PageLinks.AddLink(PageContext.PageForumName);
                BindData();
            }
        }

        protected void Delete_Load(object sender, System.EventArgs e)
        {
            ((LinkButton)sender).Attributes["onclick"] = String.Format("return confirm('{0}')", GetText("MODERATE_FORUM", "ASK_DELETE"));
        }

        private void BindData()
        {
            List.DataSource = YAF.Classes.Data.DB.message_listreported(7, PageContext.PageForumID);
            DataBind();
        }

        protected string FormatMessage(DataRowView row)
        {
            string msg = row["Message"].ToString();

			if (msg.IndexOf('<') >= 0)
				return Server.HtmlEncode(msg);

			return msg;
        }

        protected bool CompareMessage(Object originalMessage, Object newMessage)
        {
            return ((String)originalMessage != (String)newMessage);
        }

        private void List_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "delete":
                    YAF.Classes.Data.DB.message_delete(e.CommandArgument, true, "", 1, true);
                    BindData();
                    PageContext.AddLoadMessage(GetText("MODERATE_FORUM", "DELETED"));
                    break;
                case "view":
                    YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.posts, "m={0}", e.CommandArgument);
                    break;
                case "copyover":
                    BindData();
                    YAF.Classes.Data.DB.message_reportcopyover(e.CommandArgument);
                    break;
                case "resolved":
                    YAF.Classes.Data.DB.message_reportresolve(7, e.CommandArgument, PageContext.PageUserID);
                    BindData();
                    PageContext.AddLoadMessage(GetText("MODERATE_FORUM", "RESOLVEDFEEDBACK"));
                    break;
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            List.ItemCommand += new RepeaterCommandEventHandler(List_ItemCommand);
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
