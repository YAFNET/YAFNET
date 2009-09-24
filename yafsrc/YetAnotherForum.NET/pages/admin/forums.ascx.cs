/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Data;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for forums.
	/// </summary>
	public partial class forums : YAF.Classes.Core.AdminPage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			PageContext.PageElements.RegisterJsResourceInclude( "blockUIJs", "js/jquery.blockUI.js" );

			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YafBuildLink.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",YafBuildLink.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("Forums","");

				BindData();
			}
		}

		protected void DeleteCategory_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this category?')";
		}

		protected void DeleteForum_Load(object sender, System.EventArgs e) 
		{
			( (LinkButton)sender ).Attributes["onclick"] = "return (confirm('Permanently delete this Forum including ALL topics, polls, attachments and messages associated with it?') && confirm('Are you POSITIVE?'));";
		}

		private void BindData() 
		{
			using(DataSet ds = YAF.Classes.Data.DB.ds_forumadmin(PageContext.PageBoardID))
				CategoryList.DataSource = ds.Tables[YafDBAccess.GetObjectName("Category")];
			DataBind();
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
			this.CategoryList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.CategoryList_ItemCommand);

		}
		#endregion

		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					YafBuildLink.Redirect( ForumPages.admin_editforum,"f={0}",e.CommandArgument);
					break;
				case "delete":
					// schedule...
					ForumDeleteTask.Start( PageContext.PageBoardID, Convert.ToInt32(e.CommandArgument) );
          // enable timer...
					UpdateStatusTimer.Enabled = true;
					// show blocking ui...
					PageContext.PageElements.RegisterJsBlockStartup( "BlockUIExecuteJs",
					                                                 YAF.Utilities.JavaScriptBlocks.BlockUIExecuteJs(
																														"DeleteForumMessage " ) );
					break;
			}
		}

		private void ClearCaches()
		{
			// clear moderatorss cache
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators ) );
			// clear category cache...
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumCategory ) );
			// clear active discussions cache..
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumActiveDiscussions ) );
		}

		protected void NewForum_Click(object sender, System.EventArgs e)
		{
			YafBuildLink.Redirect( ForumPages.admin_editforum);
		}

		private void CategoryList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					YafBuildLink.Redirect( ForumPages.admin_editcategory,"c={0}",e.CommandArgument);
					break;
				case "delete":
					if (YAF.Classes.Data.DB.category_delete(e.CommandArgument))
					{
						BindData();
						ClearCaches();
					}
					else
						PageContext.AddLoadMessage("You cannot delete this Category as it has at least one forum assigned to it.\nTo move forums click on \"Edit\" and change the category the forum is assigned to.");
					break;
			}
		}

		protected void NewCategory_Click(object sender, System.EventArgs e)
		{
			YafBuildLink.Redirect( ForumPages.admin_editcategory);
		}

		protected void UpdateStatusTimer_Tick( object sender, EventArgs e )
		{
			// see if the migration is done....
			if ( YafTaskModule.Current.TaskManager.ContainsKey( ForumDeleteTask.TaskName ) && YafTaskModule.Current.TaskManager[ForumDeleteTask.TaskName].IsRunning )
			{
				// continue...
				return;
			}

			UpdateStatusTimer.Enabled = false;
			// rebind...
			BindData();
			// clear caches...
			ClearCaches();
			// done here...
			YafBuildLink.Redirect( ForumPages.admin_forums );
		}
	}
}
