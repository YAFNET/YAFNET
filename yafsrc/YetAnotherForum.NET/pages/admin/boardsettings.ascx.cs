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
using System.Globalization;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public partial class boardsettings : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Board Settings", "" );

				// create list boxes by populating datasources from Data class
				Theme.DataSource = YafStaticData.Themes();
				Theme.DataTextField = "Theme";
				Theme.DataValueField = "FileName";

				Language.DataSource = YafStaticData.Languages();
				Language.DataTextField = "Language";
				Language.DataValueField = "FileName";

				ShowTopic.DataSource = YafStaticData.TopicTimes();
				ShowTopic.DataTextField = "TopicText";
				ShowTopic.DataValueField = "TopicValue";

				FileExtensionAllow.DataSource = YafStaticData.AllowDisallow();
				FileExtensionAllow.DataTextField = "Text";
				FileExtensionAllow.DataValueField = "Value";

				BindData();

				SetSelectedOnList( ref Theme, PageContext.BoardSettings.Theme );
				SetSelectedOnList( ref Language, PageContext.BoardSettings.Language );
				SetSelectedOnList( ref ShowTopic, PageContext.BoardSettings.ShowTopicsDefault.ToString() );
				SetSelectedOnList( ref FileExtensionAllow, PageContext.BoardSettings.FileExtensionAreAllowed ? "0" : "1" );

				AllowThemedLogo.Checked = PageContext.BoardSettings.AllowThemedLogo;
			}
		}

		private void SetSelectedOnList( ref DropDownList list, string value )
		{
			ListItem selItem = list.Items.FindByValue( value );

			if ( selItem != null )
			{
				selItem.Selected = true;
			}
			else if ( list.Items.Count > 0 ) 
			{
				// select the first...
				list.SelectedIndex = 0;
			}
		}

		private void BindData()
		{
			DataRow row;
			using ( DataTable dt = YAF.Classes.Data.DB.board_list( PageContext.PageBoardID ) )
				row = dt.Rows [0];

			DataBind();
			Name.Text = ( string ) row ["Name"];
			AllowThreaded.Checked = ( bool ) row ["AllowThreaded"];
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Data.DB.board_save( PageContext.PageBoardID, Name.Text, AllowThreaded.Checked );

			PageContext.BoardSettings.Theme = Theme.SelectedValue;
			PageContext.BoardSettings.Language = Language.SelectedValue;
			PageContext.BoardSettings.ShowTopicsDefault = Convert.ToInt32( ShowTopic.SelectedValue );
			PageContext.BoardSettings.AllowThemedLogo = AllowThemedLogo.Checked;
			PageContext.BoardSettings.FileExtensionAreAllowed = ( Convert.ToInt32( FileExtensionAllow.SelectedValue ) == 0 ? true : false );

			/// save the settings to the database
			PageContext.BoardSettings.SaveRegistry();

			/// Reload forum settings
			PageContext.BoardSettings = null;

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_admin );
		}
	}
}
