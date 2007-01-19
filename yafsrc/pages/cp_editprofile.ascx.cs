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
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Globalization;
using System.Collections.Specialized;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for cp_editprofile.
	/// </summary>
	public partial class cp_editprofile : YAF.Classes.Base.ForumPage
	{

		public cp_editprofile()
			: base( "CP_EDITPROFILE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if(User==null)
			{
				if(CanLogin)
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
				else
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( PageContext.PageUserName, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_profile ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );
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
