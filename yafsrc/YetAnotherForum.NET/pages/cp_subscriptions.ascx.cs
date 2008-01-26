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

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for cp_subscriptions.
	/// </summary>
	public partial class cp_subscriptions : YAF.Classes.Base.ForumPage
	{

		public cp_subscriptions()
			: base( "CP_SUBSCRIPTIONS" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( User == null )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
			}

			if ( !IsPostBack )
			{
				BindData();

				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( PageContext.PageUserName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_profile ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				UnsubscribeForums.Text = GetText( "unsubscribe" );
				UnsubscribeTopics.Text = GetText( "unsubscribe" );
			}
		}

		private void BindData()
		{
			ForumList.DataSource = YAF.Classes.Data.DB.watchforum_list( PageContext.PageUserID );
			TopicList.DataSource = YAF.Classes.Data.DB.watchtopic_list( PageContext.PageUserID );
			DataBind();
		}

		protected string FormatForumReplies( object o )
		{
			DataRowView row = ( DataRowView ) o;
			return String.Format( "{0}", ( int ) row ["Messages"] - ( int ) row ["Topics"] );
		}

		protected string FormatLastPosted( object o )
		{
			DataRowView row = ( DataRowView ) o;

			if ( row ["LastPosted"].ToString().Length == 0 )
				return "&nbsp;";

			string link = String.Format( "<a href=\"{0}\">{1}</a>",
				YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", row ["LastUserID"] ),
				row ["LastUserName"]
			);
			string by = String.Format( GetText( "lastpostlink" ),
				YafDateTime.FormatDateTime( ( DateTime ) row ["LastPosted"] ),
				link );

			string html = String.Format( "{0} <a href=\"{1}\"><img src=\"{2}\"'></a>",
				by,
				YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", row ["LastMessageID"] ),
				GetThemeContents( "ICONS", "ICON_LATEST" )
				);
			return html;
		}

		protected void UnsubscribeTopics_Click( object sender, System.EventArgs e )
		{
			bool NoneChecked = true;
			for ( int i = 0; i < TopicList.Items.Count; i++ )
			{
				CheckBox ctrl = ( CheckBox ) TopicList.Items [i].FindControl( "unsubx" );
				Label lbl = ( Label ) TopicList.Items [i].FindControl( "ttid" );
				if ( ctrl.Checked )
				{
					YAF.Classes.Data.DB.watchtopic_delete( lbl.Text );
					NoneChecked = false;
				}
			}
			if ( NoneChecked )
				PageContext.AddLoadMessage( GetText( "WARN_SELECTTOPICS" ) );
			else
				BindData();
		}

		protected void UnsubscribeForums_Click( object sender, System.EventArgs e )
		{
			bool NoneChecked = true;
			for ( int i = 0; i < ForumList.Items.Count; i++ )
			{
				CheckBox ctrl = ( CheckBox ) ForumList.Items [i].FindControl( "unsubf" );
				Label lbl = ( Label ) ForumList.Items [i].FindControl( "tfid" );
				if ( ctrl.Checked )
				{
					YAF.Classes.Data.DB.watchforum_delete( lbl.Text );
					NoneChecked = false;
				}
			}
			if ( NoneChecked )
				PageContext.AddLoadMessage( GetText( "WARN_SELECTFORUMS" ) );
			else
				BindData();
		}
	}
}
