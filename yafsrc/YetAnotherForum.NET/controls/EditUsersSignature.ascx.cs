/* Yet Another Forum.NET
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
		protected YAF.Editor.ForumEditor _sig;

		public bool ShowHeader
		{
			get
			{
				if ( ViewState ["ShowHeader"] != null )
				{
					return Convert.ToBoolean( ViewState ["ShowHeader"] );
				}

				return true;
			}
			set
			{
				ViewState ["ShowHeader"] = value;
			}
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			PageContext.QueryIDs = new QueryStringIDHelper( "u" );

			_sig.BaseDir = YafForumInfo.ForumRoot + "editors";
			_sig.StyleSheet = YafBuildLink.ThemeFile( "theme.css" );

			if ( !IsPostBack )
			{
				save.Text = PageContext.Localization.GetText( "COMMON", "Save" );
				cancel.Text = PageContext.Localization.GetText( "COMMON", "Cancel" );

				BindData();
			}
		}

		protected void Page_PreRender( object sender, EventArgs e )
		{
			trHeader.Visible = ShowHeader;
		}

		protected void BindData()
		{
			_sig.Text = YAF.Classes.Data.DB.user_getsignature( CurrentUserID );
		}

		private void save_Click( object sender, EventArgs e )
		{
			string body = _sig.Text;
			//body = FormatMsg.RepairHtml(this,body,false);

			if ( _sig.Text.Length > 0 )
			{
				YAF.Classes.Data.DB.user_savesignature( CurrentUserID, body );
			}
			else
			{
				YAF.Classes.Data.DB.user_savesignature( CurrentUserID, DBNull.Value );
			}

			if ( InAdminPages )
			{
				BindData();
			}
			else
			{
				DoRedirect();
			}
		}

		private void cancel_Click( object sender, EventArgs e )
		{
			DoRedirect();
		}

		private void DoRedirect()
		{
			if ( InModeratorMode )
			{
				YafBuildLink.Redirect( ForumPages.profile, "u={0}", CurrentUserID );
			}
			else
			{
				YafBuildLink.Redirect( ForumPages.cp_profile );
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			// since signatures are so small only allow BBCode in them...
			_sig = new YAF.Editor.BBCodeEditor();
			EditorLine.Controls.Add( _sig );

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

		private int CurrentUserID
		{
			get
			{
				if ( InAdminPages && PageContext.IsAdmin && PageContext.QueryIDs.ContainsKey( "u" ) )
				{
					return (int)PageContext.QueryIDs ["u"];
				}
				else if ( InModeratorMode && ( PageContext.IsAdmin || PageContext.IsModerator ) && PageContext.QueryIDs.ContainsKey( "u" ) )
				{
					return (int)PageContext.QueryIDs ["u"];
				}
				else
				{
					return PageContext.PageUserID;
				}
			}
		}

		protected bool _adminEditMode = false;
		public bool InAdminPages
		{
			get
			{
				return _adminEditMode;
			}
			set
			{
				_adminEditMode = value;
			}
		}

		protected bool _moderatorEditMode = false;
		public bool InModeratorMode
		{
			get
			{
				return _moderatorEditMode;
			}
			set
			{
				_moderatorEditMode = value;
			}
		}
	}
}