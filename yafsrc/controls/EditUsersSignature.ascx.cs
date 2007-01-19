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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class EditUsersSignature : YAF.Classes.Base.BaseUserControl
	{
		protected YAF.Editor.ForumEditor sig;
		private int CurrentUserID;
		private bool AdminEditMode = false;

		protected void Page_Load( object sender, EventArgs e )
		{
			if ( AdminEditMode && PageContext.IsAdmin && Request.QueryString ["u"] != null )
			{
				CurrentUserID = Convert.ToInt32( Request.QueryString ["u"] );
			}
			else
			{
				CurrentUserID = PageContext.PageUserID;
			}

			sig.BaseDir = yaf_ForumInfo.ForumRoot + "editors";
			sig.StyleSheet = yaf_BuildLink.ThemeFile( "theme.css" );

			if ( !IsPostBack )
			{
				sig.Text = YAF.Classes.Data.DB.user_getsignature( CurrentUserID );

				save.Text = PageContext.Localization.GetText( "COMMON", "Save" );
				cancel.Text = PageContext.Localization.GetText( "COMMON", "Cancel" );
			}
		}

		private void save_Click( object sender, EventArgs e )
		{
			string body = sig.Text;
			//body = FormatMsg.RepairHtml(this,body,false);

			if ( sig.Text.Length > 0 )
				YAF.Classes.Data.DB.user_savesignature( CurrentUserID, body );
			else
				YAF.Classes.Data.DB.user_savesignature( CurrentUserID, DBNull.Value );

			if ( AdminEditMode )
				yaf_BuildLink.Redirect( ForumPages.admin_users );
			else
				yaf_BuildLink.Redirect( ForumPages.cp_profile );
		}

		private void cancel_Click( object sender, EventArgs e )
		{
			if ( AdminEditMode )
				yaf_BuildLink.Redirect( ForumPages.admin_users );
			else
				yaf_BuildLink.Redirect( ForumPages.cp_profile );
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			// since signatures are so small only allow BBCode in them...
			sig = new YAF.Editor.BBCodeEditor();
			EditorLine.Controls.Add( sig );

			save.Click += new EventHandler( save_Click );
			cancel.Click += new EventHandler( cancel_Click );
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

		public bool InAdminPages
		{
			get
			{
				return AdminEditMode;
			}
			set
			{
				AdminEditMode = value;
			}
		}
	}
}