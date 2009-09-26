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
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for register.
	/// </summary>
	public partial class version : YAF.Classes.Core.AdminPage
	{
		private long _lastVersion;
		private DateTime _lastVersionDate;

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Version Check", "" );
			}

			try
			{
				using ( RegisterForum.Register reg = new RegisterForum.Register() )
				{
					_lastVersion = reg.LatestVersion();
					_lastVersionDate = reg.LatestVersionDate();
				}
			}
			catch ( Exception )
			{
				_lastVersion = 0;
			}
			Upgrade.Visible = _lastVersion > YafForumInfo.AppVersionCode;


			DataBind();
		}

		protected string LastVersion
		{
			get
			{
				return YafForumInfo.AppVersionNameFromCode( _lastVersion );
			}
		}
		protected string LastVersionDate
		{
			get
			{
				return YafServices.DateTime.FormatDateShort( _lastVersionDate );
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
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
