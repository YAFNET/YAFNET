/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for _default.
	/// </summary>
	public class _default : BasePage
	{
		protected System.Web.UI.WebControls.Label TimeNow;
		protected System.Web.UI.WebControls.Label Stats;
		protected System.Web.UI.WebControls.Repeater CategoryList;
		protected System.Web.UI.WebControls.Label TimeLastVisit;
		protected System.Web.UI.WebControls.Repeater ForumList;
		protected System.Web.UI.WebControls.Repeater ActiveList;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink HomeLink2;
		protected System.Web.UI.HtmlControls.HtmlGenericControl NavLinks;
		protected System.Web.UI.HtmlControls.HtmlGenericControl NavLinks2;
		protected System.Web.UI.WebControls.HyperLink CategoryLink;
		protected System.Web.UI.HtmlControls.HtmlGenericControl Welcome;
		protected System.Web.UI.WebControls.Label activeinfo;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				DataSet ds = new DataSet();

				TimeNow.Text = String.Format(CustomCulture,"Current time: {0}.",FormatTime(DateTime.Now));
				TimeLastVisit.Text = String.Format(CustomCulture,"Your last visit: {0}.",FormatDateTime(DateTime.Parse(Session["lastvisit"].ToString())));

				if(PageCategoryID!=0) {
					HomeLink.NavigateUrl = BaseDir;
					HomeLink.Text = ForumName;
					CategoryLink.NavigateUrl = String.Format("default.aspx?c={0}",PageCategoryID);
					CategoryLink.Text = PageCategoryName;
					NavLinks2.Visible = false;
					Welcome.Visible = false;
				} else {
					HomeLink2.NavigateUrl = BaseDir;
					HomeLink2.Text = ForumName;
					NavLinks.Visible = false;
				}

				SqlDataAdapter da = new SqlDataAdapter("yaf_forum_moderators",yaf.DataManager.GetConnection());
				da.SelectCommand.CommandType = CommandType.StoredProcedure;
				da.Fill(ds,"Moderators");
				da.SelectCommand.Parameters.Add("@UserID",PageUserID);
				if(PageCategoryID!=0)
					da.SelectCommand.Parameters.Add("@CategoryID",PageCategoryID);
				da.SelectCommand.CommandText = "yaf_category_listread";
				da.Fill(ds,"yaf_Category");
				da.SelectCommand.CommandText = "yaf_forum_listread";
				da.Fill(ds,"yaf_Forum");

				CategoryList.DataSource = ds.Tables["yaf_Category"];

				ds.Relations.Add("myrelation",ds.Tables["yaf_Category"].Columns["CategoryID"],ds.Tables["yaf_Forum"].Columns["CategoryID"]);
				ds.Relations.Add("rel2",ds.Tables["yaf_Forum"].Columns["ForumID"],ds.Tables["Moderators"].Columns["ForumID"],false);

				// Active users
				// Call this before yaf_forum_stats to clean up active users
				ActiveList.DataSource = DataManager.GetData("yaf_active_list",CommandType.StoredProcedure);

				// Forum statistics
				DataTable dtStats = yaf.DataManager.GetData("yaf_forum_stats",CommandType.StoredProcedure);
				DataRow stats = dtStats.Rows[0];
				
				Stats.Text = String.Format(CustomCulture,"There are {0:N0} posts in {1:N0} topics in {2:N0} forums.<br/>",stats["posts"],stats["topics"],stats["forums"]);
				
				if(!stats.IsNull("LastPost")) 
					Stats.Text += String.Format(CustomCulture,"Last post on {0} by <a href=\"profile.aspx?u={1}\">{2}</a>.<br/>",FormatDateTime((DateTime)stats["LastPost"]),stats["LastUserID"],stats["LastUser"]);
				
				Stats.Text += String.Format(CustomCulture,"We have {0:N0} registered members.<br/>",stats["members"]);

				Stats.Text += String.Format("The newest member is <a href=\"profile.aspx?u={0}\">{1}</a><br/>",stats["LastMemberID"],stats["LastMember"]);

				activeinfo.Text = String.Format(CustomCulture,"{0:N0} <a href=\"activeusers.aspx\">active users</a> - {1:N0} members and {2:N0} guests.",stats["ActiveUsers"],stats["ActiveMembers"],stats["ActiveGuests"]);

				DataBind();
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		protected string FormatLastPost(System.Data.DataRow row) {
			if(!row.IsNull("LastPosted")) {
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > (DateTime)Session["lastvisit"])
					minipost = ThemeFile("icon_newest_reply.gif");
				else
					minipost = ThemeFile("icon_latest_reply.gif");
				return String.Format(CustomCulture,"{0}<br/>in <a href=\"posts.aspx?t={5}\">{6}</a><br/>by <a href=\"profile.aspx?u={1}\">{2}</a>&nbsp;<a href=\"posts.aspx?m={4}#{4}\"><img src='{3}'></a>", 
					FormatDateTime((DateTime)row["LastPosted"]), 
					row["LastUserID"], 
					row["LastUser"], 
					minipost, 
					row["LastMessageID"],
					row["LastTopicID"],
					row["LastTopicName"]
				);
			}
			else
				return "No Posts";
		}

		protected string GetForumIcon(object lastPosted,object Locked,object oPostAccess,object oReplyAccess,object oReadAccess) 
		{
			try 
			{
				if((bool)Locked)
					return ThemeFile("topic_lock.png");

				if(DateTime.Parse(lastPosted.ToString()) > (DateTime)Session["lastvisit"])
					return ThemeFile("topic_new.png");
				else
					return ThemeFile("topic.png");
			}
			catch(Exception) 
			{
				return ThemeFile("topic.png");
			}
		}
		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			switch(e.CommandName) {
				case "forum":
					if(Data.ForumReadAccess(PageUserID,e.CommandArgument))
						Response.Redirect(String.Format("topics.aspx?f={0}",e.CommandArgument));

					AddLoadMessage("You can't access that forum.");
					break;
			}
		}

		protected void ModeratorList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			//AddLoadMessage("TODO: Fix this");
			//TODO: Show moderators
		}
	}
}
