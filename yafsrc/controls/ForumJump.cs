using System;
using System.Data;

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
				ForumID = Page.PageForumID;
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
				Page.Response.Redirect(String.Format("topics.aspx?f={0}",ForumID));
			else
				Page.Response.Redirect(String.Format("{0}?c={1}",Page.BaseDir,-ForumID));
		}
		#endregion

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			DataTable dt;
			string cachename = String.Format("forumjump_{0}",Page.User.Identity.Name);
			if(Page.Cache[cachename] != null) 
			{
				dt = (DataTable)Page.Cache[cachename];
			} 
			else 
			{
				dt = DB.forum_listread(((BasePage)Page).PageUserID,null);
				Page.Cache[cachename] = dt;
			}

			writer.WriteLine(String.Format("<select name=\"{0}\" onchange=\"{1}\" language=\"javascript\" id=\"{0}\">",this.UniqueID,Page.GetPostBackEventReference(this)));

			int nForumID = ((BasePage)Page).PageForumID;
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
