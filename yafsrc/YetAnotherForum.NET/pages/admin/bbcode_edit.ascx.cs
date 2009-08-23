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
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	public partial class bbcode_edit : YAF.Classes.Core.AdminPage
	{
		private int? _bbcodeId = null;
		protected int? BBCodeID
		{
			get
			{
				if ( _bbcodeId != null ) return _bbcodeId;
				else if ( Request.QueryString ["b"] != null )
				{
					int id;
					if ( int.TryParse( Request.QueryString ["b"], out id ) )
					{
						_bbcodeId = id;
						return id;
					}
				}
				return null;
			}
		}

		protected void Page_Load( object sender, EventArgs e )
		{
			string strAddEdit = ( BBCodeID == null ) ? "Add" : "Edit";

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( strAddEdit + " YafBBCode Extensions", "" );

				BindData();
			}

			txtName.Attributes.Add( "style", "width:100%" );
			txtDescription.Attributes.Add( "style", "width:100%;height:75px;" );
			txtOnClickJS.Attributes.Add( "style", "width:100%;height:75px;" );
			txtDisplayJS.Attributes.Add( "style", "width:100%;height:75px;" );
			txtEditJS.Attributes.Add( "style", "width:100%;height:75px;" );
			txtDisplayCSS.Attributes.Add( "style", "width:100%;height:75px;" );
			txtSearchRegEx.Attributes.Add( "style", "width:100%;height:75px;" );
			txtReplaceRegEx.Attributes.Add( "style", "width:100%;height:75px;" );
			txtVariables.Attributes.Add( "style", "width:100%;height:75px;" );
			txtModuleClass.Attributes.Add( "style", "width:100%" );
		}

		protected void BindData()
		{
			if ( BBCodeID != null )
			{
				DataRow row = DB.bbcode_list( PageContext.PageBoardID, BBCodeID.Value ).Rows [0];
				// fill the control values...
				txtName.Text = row ["Name"].ToString();
				txtExecOrder.Text = row ["ExecOrder"].ToString();
				txtDescription.Text = row ["Description"].ToString();
				txtOnClickJS.Text = row ["OnClickJS"].ToString();
				txtDisplayJS.Text = row ["DisplayJS"].ToString();
				txtEditJS.Text = row ["EditJS"].ToString();
				txtDisplayCSS.Text = row ["DisplayCSS"].ToString();
				txtSearchRegEx.Text = row ["SearchRegex"].ToString();
				txtReplaceRegEx.Text = row ["ReplaceRegex"].ToString();
				txtVariables.Text = row ["Variables"].ToString();
				txtModuleClass.Text = row ["ModuleClass"].ToString();
				chkUseModule.Checked = Convert.ToBoolean( (row ["UseModule"] == DBNull.Value) ? false : row ["UseModule"] );
			}
		}

		protected void Add_Click( object sender, EventArgs e )
		{
			DB.bbcode_save(
				BBCodeID,
				PageContext.PageBoardID,
				txtName.Text.Trim(),
				txtDescription.Text,
				txtOnClickJS.Text,
				txtDisplayJS.Text,
				txtEditJS.Text,
				txtDisplayCSS.Text,
				txtSearchRegEx.Text,
				txtReplaceRegEx.Text,
				txtVariables.Text,
				chkUseModule.Checked,
				txtModuleClass.Text,
				int.Parse( txtExecOrder.Text )
			);
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.CustomBBCode ) );
			YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
			YafBuildLink.Redirect( ForumPages.admin_bbcode );
		}

		protected void Cancel_Click( object sender, EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_bbcode );
		}
	}
}