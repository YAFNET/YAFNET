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

namespace yaf
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for forumjump.
	/// </summary>
	public abstract class forumjump : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DropDownList list;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				DataTable dt;
				string cachename = String.Format("forumjump_{0}",Page.User.Identity.Name);
				if(Cache[cachename] != null) {
					dt = (DataTable)Cache[cachename];
				} else {
					dt = DB.forum_listread(((BasePage)Page).PageUserID,null);
					Cache[cachename] = dt;
				}

				int nOldCat = 0;
				list.Items.Add(new ListItem(((BasePage)Page).GetText("select_forum"),"0"));
				for(int i=0;i<dt.Rows.Count;i++) {
					DataRow row = dt.Rows[i];
					if((int)row["CategoryID"] != nOldCat) {
						nOldCat = (int)row["CategoryID"];
						list.Items.Add(new ListItem((string)row["Category"],(-(int)row["CategoryID"]).ToString()));
					}
					list.Items.Add(new ListItem(" - " + (string)row["Forum"],row["ForumID"].ToString()));
				}
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
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		protected void list_SelectedIndexChanged(object sender, System.EventArgs e) {
			int ForumID = int.Parse(list.SelectedItem.Value);
			if(ForumID>0)
				Response.Redirect(String.Format("topics.aspx?f={0}",ForumID));
			else if(ForumID<0)
				Response.Redirect(String.Format("default.aspx?c={0}",-ForumID));
			else {
				list.SelectedItem.Selected = false;
				list.Items[0].Selected = true;
			}
		}
	}
}
