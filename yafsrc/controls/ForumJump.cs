using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class ForumJump : BaseControl, System.Web.UI.IPostBackEventHandler
	{
		public void RaisePostBackEvent(String eventArgument)
		{
			int nForumID = int.Parse(Page.Request.Form[this.UniqueID]);
			if(nForumID>0)
				Page.Response.Redirect(String.Format("topics.aspx?f={0}",nForumID));
			else
				Page.Response.Redirect(String.Format("{0}?c={1}",Page.BaseDir,-nForumID));
		}

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
