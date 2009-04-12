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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for bannedip.
	/// </summary>
	public partial class replacewords : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Replace Words", "" );

				BindData();
			}
		}

		private void BindData()
		{
			list.DataSource = YAF.Classes.Data.DB.replace_words_list( PageContext.PageBoardID, null );
			DataBind();
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton )sender ).Attributes ["onclick"] = "return confirm('Delete this Word Replacement?')";
		}

		private void list_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "add" )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_replacewords_edit );
			}
			else if ( e.CommandName == "edit" )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_replacewords_edit, "i={0}", e.CommandArgument );
			}
			else if ( e.CommandName == "delete" )
			{
				YAF.Classes.Data.DB.replace_words_delete( e.CommandArgument );
				YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ReplaceWords ) );
				BindData();
			}
			else if ( e.CommandName == "export" )
			{
				DataTable replaceDT = YAF.Classes.Data.DB.replace_words_list( PageContext.PageBoardID, null );
				replaceDT.DataSet.DataSetName = "YafReplaceWordsList";
				replaceDT.TableName = "YafReplaceWords";
				replaceDT.Columns.Remove( "ID" );
				replaceDT.Columns.Remove( "BoardID" );

				Response.ContentType = "text/xml";
				Response.AppendHeader( "Content-Disposition", "attachment; filename=YafReplaceWordsExport.xml" );
				replaceDT.DataSet.WriteXml( Response.OutputStream );
				Response.End();
			}
			else if ( e.CommandName == "import" )
			{
				YafBuildLink.Redirect( ForumPages.admin_replacewords_import );
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			list.ItemCommand += new RepeaterCommandEventHandler( list_ItemCommand );
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
