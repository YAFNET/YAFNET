/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for movetopic.
	/// </summary>
	public partial class movetopic : YAF.Classes.Base.ForumPage
	{
	
		public movetopic() : base("MOVETOPIC")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.QueryString["t"] == null || !PageContext.ForumModeratorAccess)
				YafBuildLink.AccessDenied();

			if(!IsPostBack)
			{
				if(PageContext.Settings.LockedForum==0)
				{
					PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
					PageLinks.AddLink(PageContext.PageCategoryName,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum,"c={0}",PageContext.PageCategoryID));
				}
				PageLinks.AddForumLinks(PageContext.PageForumID);
				PageLinks.AddLink(PageContext.PageTopicName,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts,"t={0}",PageContext.PageTopicID));

				Move.Text = GetText("move");
				// Ederon : 7/14/2007 - by default, leave pointer is set on value defined on host level
				LeavePointer.Checked = PageContext.BoardSettings.ShowMoved;

				ForumList.DataSource = YAF.Classes.Data.DB.forum_listall_sorted( PageContext.PageBoardID, PageContext.PageUserID );
				
				DataBind();
				
				System.Web.UI.WebControls.ListItem pageItem = ForumList.Items.FindByValue(PageContext.PageForumID.ToString());
				if (pageItem != null) pageItem.Selected = true;
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

		}
		#endregion

		protected void Move_Click(object sender, System.EventArgs e)
		{
            if (Convert.ToInt32(ForumList.SelectedValue) <= 0)
            {
                PageContext.AddLoadMessage(GetText("CANNOT_MOVE_TO_CATEGORY"));
                return;
            }
            // only move if it's a destination is a different forum.
            if (Convert.ToInt32(ForumList.SelectedValue) != PageContext.PageForumID)
            {
				// Ederon : 7/14/2007
                YAF.Classes.Data.DB.topic_move(PageContext.PageTopicID, ForumList.SelectedValue, LeavePointer.Checked);
            }
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.topics,"f={0}",PageContext.PageForumID);
		}
	}
}
