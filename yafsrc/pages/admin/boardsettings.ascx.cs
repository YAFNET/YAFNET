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
using System.Globalization;

namespace YAF.Pages.Admin {
	/// <summary>
	/// Summary description for settings.
	/// </summary>
	public partial class boardsettings : AdminPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e) 
		{
			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("Board Settings","");

				// create list boxes by populating datasources from Data class
				Theme.DataSource = Data.Themes();
				Theme.DataTextField = "Theme";
				Theme.DataValueField = "FileName";

				Language.DataSource = Data.Languages();
				Language.DataTextField = "Language";
				Language.DataValueField = "FileName";

				ShowTopic.DataSource = Data.TopicTimes();
				ShowTopic.DataTextField = "TopicText";
				ShowTopic.DataValueField = "TopicValue";

				BindData();
				
				Theme.Items.FindByValue(BoardSettings.Theme).Selected = true;
				Language.Items.FindByValue(BoardSettings.Language).Selected = true;
				ShowTopic.Items.FindByValue(BoardSettings.ShowTopicsDefault.ToString()).Selected = true;
                AllowThemedLogo.Checked = BoardSettings.AllowThemedLogo;
			}
		}

		private void BindData()
		{
			DataRow row;
			using(DataTable dt = YAF.Classes.Data.DB.board_list(PageBoardID))
				row = dt.Rows[0];

			DataBind();
			Name.Text = (string)row["Name"];
			AllowThreaded.Checked = (bool)row["AllowThreaded"];
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

		protected void Save_Click(object sender, System.EventArgs e)
		{
            YAF.Classes.Data.DB.board_save(PageBoardID, Name.Text, AllowThreaded.Checked);

			BoardSettings.Theme = Theme.SelectedValue;
			BoardSettings.Language = Language.SelectedValue;
			BoardSettings.ShowTopicsDefault = Convert.ToInt32(ShowTopic.SelectedValue);
            BoardSettings.AllowThemedLogo = AllowThemedLogo.Checked;
			/// save the settings to the database
			BoardSettings.SaveRegistry();

			/// Reload forum settings
			BoardSettings = null;

			Forum.Redirect( ForumPages.admin_admin);
		}
	}
}
