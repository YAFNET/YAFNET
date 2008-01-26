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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for bannedip_edit.
	/// </summary>
	public partial class extensions_edit : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			string strAddEdit = ( Request.QueryString ["i"] == null ) ? "Add" : "Edit";

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( strAddEdit + " File Extensions", "" );

				BindData();
			}

			extension.Attributes.Add( "style", "width:250px" );
		}

		private void BindData()
		{
			if ( !String.IsNullOrEmpty( Request.QueryString ["i"] ) )
			{
				DataRow row = YAF.Classes.Data.DB.extension_edit( Security.StringToLongOrRedirect( Request.QueryString ["i"] ) ).Rows [0];
				extension.Text = ( string ) row ["Extension"];
			}
		}

		private void Add_Click( object sender, EventArgs e )
		{
			string ext = extension.Text.Trim();

			if ( !IsValidExtension( ext ) )
			{
				BindData();
			}
			else
			{
				YAF.Classes.Data.DB.extension_save( Request.QueryString ["i"], PageContext.PageBoardID, ext );
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_extensions );
			}
		}

		protected bool IsValidExtension( string newExtension )
		{
			if ( String.IsNullOrEmpty( newExtension ) )
			{
				PageContext.AddLoadMessage( "You must enter something." );
				return false;
			}

			if ( newExtension.IndexOf( '.' ) != -1 )
			{
				PageContext.AddLoadMessage( "Remove the period in the extension." );
				return false;
			}

			// TODO: maybe check for duplicate?

			return true;
		}

		private void Cancel_Click( object sender, EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_extensions );
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			save.Click += new EventHandler( Add_Click );
			cancel.Click += new EventHandler( Cancel_Click );
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

