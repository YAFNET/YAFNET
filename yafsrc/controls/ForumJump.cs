using System;
using System.Data;
using yaf.pages;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class ForumJump : BaseControl, System.Web.UI.IPostBackDataHandler
	{
		private void Page_Load(object sender, System.EventArgs e) 
		{
			if(!Page.IsPostBack)
				ForumID = ForumPage.PageForumID;
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}

		private int ForumID 
		{
			get 
			{
				return (int)ViewState["ForumID"];
			}
			set 
			{
				ViewState["ForumID"] = value;
			}
		}

		#region IPostBackDataHandler
		public virtual bool LoadPostData(string postDataKey,System.Collections.Specialized.NameValueCollection postCollection) 
		{
			int nForumID;
			try 
			{
				nForumID = int.Parse(postCollection[postDataKey]);
				if(nForumID==ForumID)
					return false;
			}
			catch(Exception) 
			{
				return false;
			}

			ForumID = nForumID;
			return true;
		}

		public virtual void RaisePostDataChangedEvent() 
		{
			if(ForumID>0)
				Forum.Redirect(Pages.topics,"f={0}",ForumID);
			else
				Forum.Redirect(Pages.forum,"c={0}",-ForumID);
		}
		#endregion

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			DataTable dt;
			string cachename = String.Format("forumjump_{0}",ForumPage.User.Name);
			if(Page.Cache[cachename] != null) 
			{
				dt = (DataTable)Page.Cache[cachename];
			} 
			else 
			{
				dt = DB.forum_listread(ForumPage.PageBoardID,ForumPage.PageUserID,null);
				Page.Cache[cachename] = dt;
			}

			writer.WriteLine(String.Format("<select name=\"{0}\" onchange=\"{1}\" language=\"javascript\" id=\"{0}\">",this.UniqueID,Page.GetPostBackEventReference(this)));

			int nForumID = ForumPage.PageForumID;
			if(nForumID<=0)
				writer.WriteLine("<option/>");
			int nOldCat = 0;
			for(int i=0;i<dt.Rows.Count;i++) 
			{
				DataRow row = dt.Rows[i];
				if((int)row["CategoryID"] != nOldCat) 
				{
					nOldCat = (int)row["CategoryID"];
					writer.WriteLine(String.Format("<option style='font-weight:bold' value='{0}'>{1}</option>",-(int)row["CategoryID"],row["Category"]));
				}
				writer.WriteLine(String.Format("<option {2}value='{0}'> - {1}</option>",row["ForumID"],row["Forum"],(int)row["ForumID"]==nForumID ? "selected=\"selected\" " : ""));
			}
			writer.WriteLine("</select>");
		}
	}
}
