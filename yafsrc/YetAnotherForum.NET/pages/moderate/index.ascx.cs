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
using System.Drawing;
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
	/// Base root control for moderating, linking to other moderating controls/pages.
	/// </summary>
	public partial class index : YAF.Classes.Base.ForumPage
	{
		#region Construcotrs & Overridden Methods

		/// <summary>
		/// Default constructor.
		/// </summary>
		public index() : base("MODERATE_DEFAULT") { }


		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			// forum index
			PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// moderation index
			PageLinks.AddLink(GetText("TITLE"));
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load(object sender, EventArgs e)
		{
			// Only moderators are allowed here
			if (!PageContext.IsModerator) YafBuildLink.AccessDenied();

			// this needs to be done just once, not during postbacks
			if (!IsPostBack)
			{
				// create page links
				CreatePageLinks();

				// bind data
				BindData();
			}
		}

		
		/// <summary>
		/// Handles event of item commands for each forum.
		/// </summary>
		protected void ForumList_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			// which command are we handling
			switch (e.CommandName.ToLower())
			{
				case "viewunapprovedposts":
					// go to unapproved posts for selected forum
					YafBuildLink.Redirect(ForumPages.moderate_unapprovedposts, "f={0}", e.CommandArgument);
					break;
				case "viewreportedposts":
					// go to reported abuses for selected forum
					YafBuildLink.Redirect(ForumPages.moderate_reportedposts, "f={0}", e.CommandArgument);
					break;
				case "viewreportedspam":
					// go to spam reports for selected forum
					YafBuildLink.Redirect(ForumPages.moderate_reportedspam, "f={0}", e.CommandArgument);
					break;
			}
		}

		#endregion


		#region Data Binding & Formatting

		/// <summary>
		/// Bind data for this control.
		/// </summary>
		private void BindData()
		{
			// get list of forums and their moderating data
			using (DataSet ds = DB.forum_moderatelist(PageContext.PageUserID, PageContext.PageBoardID))
				CategoryList.DataSource = ds.Tables[DBAccess.GetObjectName("Category")];

			// bind data to controls
			DataBind();
		}

		#endregion


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
	}
}
