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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for smilies.
	/// </summary>
	public partial class smilies : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Smilies", "" );

				BindData();
			}
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = "return confirm('Delete this smiley?')";
		}

		private void Pager_PageChange( object sender, EventArgs e )
		{
			BindData();
		}

		private void BindData()
		{
			Pager.PageSize = 25;
			DataView dv = YAF.Classes.Data.DB.smiley_list( PageContext.PageBoardID, null ).DefaultView;
			Pager.Count = dv.Count;
			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = dv;
			pds.AllowPaging = true;
			pds.CurrentPageIndex = Pager.CurrentPageIndex;
			pds.PageSize = Pager.PageSize;
			List.DataSource = pds;
			DataBind();
		}

		private void List_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			switch ( e.CommandName )
			{
				case "add":
					YafBuildLink.Redirect( ForumPages.admin_smilies_edit );
					break;
				case "edit":
					YafBuildLink.Redirect( ForumPages.admin_smilies_edit, "s={0}", e.CommandArgument );
					break;
				case "moveup":
					YAF.Classes.Data.DB.smiley_resort( PageContext.PageBoardID, e.CommandArgument, -1 );
					// invalidate the cache...
					PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.Smilies ) );
					BindData();
					YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
					break;
				case "movedown":
					YAF.Classes.Data.DB.smiley_resort( PageContext.PageBoardID, e.CommandArgument, 1 );
					// invalidate the cache...
					PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.Smilies ) );
					BindData();
					YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
					break;
				case "delete":
					YAF.Classes.Data.DB.smiley_delete( e.CommandArgument );
					// invalidate the cache...
					PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.Smilies ) );
					BindData();
					YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
					break;
				case "import":
					YafBuildLink.Redirect( ForumPages.admin_smilies_import );
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			this.Pager.PageChange += new EventHandler( Pager_PageChange );
			this.List.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler( this.List_ItemCommand );
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
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
