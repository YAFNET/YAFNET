namespace yaf.controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for ForumList.
	/// </summary>
	public class ForumList : BaseUserControl
	{
		protected Repeater forumList;

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		protected string GetForumIcon(object o) 
		{
			DataRow		row			= (DataRow)o;
			bool		locked		= (bool)row["Locked"];
			DateTime	lastRead	= ForumPage.GetForumRead((int)row["ForumID"]);
			DateTime	lastPosted	= row["LastPosted"]!=DBNull.Value ? (DateTime)row["LastPosted"] : lastRead;

			string		img, imgTitle;
			
			try 
			{
				if(locked) 
				{
					img = ForumPage.GetThemeContents("ICONS","FORUM_LOCKED");
					imgTitle = ForumPage.GetText("ICONLEGEND","Forum_Locked");
				} 
				else if(lastPosted > lastRead)
				{
					img = ForumPage.GetThemeContents("ICONS","FORUM_NEW");
					imgTitle = ForumPage.GetText("ICONLEGEND","New_Posts");
				}
				else
				{
					img = ForumPage.GetThemeContents("ICONS","FORUM");
					imgTitle = ForumPage.GetText("ICONLEGEND","No_New_Posts");
				}
			}
			catch(Exception) 
			{
				img = ForumPage.GetThemeContents("ICONS","FORUM");
				imgTitle = ForumPage.GetText("ICONLEGEND","No_New_Posts");
			}

			return String.Format("<img src=\"{0}\" title=\"{1}\"/>",img,imgTitle);
		}
		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) 
		{
			switch(e.CommandName) 
			{
				case "forum":
					if(ForumPage.DataProvider.user_access(ForumPage.PageUserID,e.CommandArgument))
						Forum.Redirect(Pages.topics,"f={0}",e.CommandArgument);

					ForumPage.AddLoadMessage(ForumPage.GetText("ERROR_NOFORUMACCESS"));
					break;
			}
		}

		protected void ModeratorList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) 
		{
			//AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}

		protected string FormatLastPost(System.Data.DataRow row) 
		{
			if(!row.IsNull("LastPosted")) 
			{
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > Mession.LastVisit)
					minipost = ForumPage.GetThemeContents("ICONS","ICON_NEWEST");
				else
					minipost = ForumPage.GetThemeContents("ICONS","ICON_LATEST");
				
				return String.Format("{0}<br/>{1}<br/>{2}&nbsp;<a title=\"{4}\" href=\"{5}\"><img src='{3}'></a>",
					ForumPage.FormatDateTime((DateTime)row["LastPosted"]),
					String.Format(ForumPage.GetText("in"),String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.posts,"t={0}",row["LastTopicID"]),row["LastTopicName"])),
					String.Format(ForumPage.GetText("by"),String.Format("<a href=\"{0}\">{1}</a>",Forum.GetLink(Pages.profile,"u={0}",row["LastUserID"]),row["LastUser"])),
					minipost,
					ForumPage.GetText("GO_LAST_POST"),
					Forum.GetLink(Pages.posts,"m={0}#{0}",row["LastMessageID"])
					);
			}
			else
				return ForumPage.GetText("NO_POSTS");
		}

		protected string GetViewing(object o) 
		{
			DataRow row = (DataRow)o;
			int nViewing = (int)row["Viewing"];
			if(nViewing>0)
				return "&nbsp;" + String.Format(ForumPage.GetText("VIEWING"),nViewing);
			else
				return "";
		}

		public System.Collections.IEnumerable DataSource
		{
			set
			{
				forumList.DataSource = value;
			}
		}
	}
}
