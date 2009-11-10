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
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class active : YAF.Classes.Core.ForumPage
	{
		protected string _lastForumName = string.Empty;

		public active()
			: base( "ACTIVE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( PageContext.BoardSettings.ShowRSSLink )
			{
				RssFeed.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.rsstopic, "pg=active" );
				RssFeed.Text = GetText( "RSSFEED" );
				RssFeed.Visible = true;
			}
			else
			{
				RssFeed.Visible = false;
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

				Since.Items.Add( new ListItem( GetTextFormatted( "last_visit", YafServices.DateTime.FormatDateTime( Mession.LastVisit ) ), "0" ) );
				Since.Items.Add( new ListItem( GetText( "last_hour" ), "-1" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_hours" ), "-2" ) );
				Since.Items.Add( new ListItem( GetText( "last_day" ), "1" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_days" ), "2" ) );
				Since.Items.Add( new ListItem( GetText( "last_week" ), "7" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_weeks" ), "14" ) );
				Since.Items.Add( new ListItem( GetText( "last_month" ), "31" ) );

				if ( Mession.ActiveTopicSince.HasValue )
				{
					ListItem activeTopicItem = Since.Items.FindByValue( Mession.ActiveTopicSince.Value.ToString() );
					if ( activeTopicItem != null ) activeTopicItem.Selected = true;
				}
				else
				{
					Since.SelectedIndex = 0;
				}
			}

			BindData();
		}

		protected void Pager_PageChange( object sender, EventArgs e )
		{
			BindData();
		}

		private void BindData()
		{
			DateTime sinceDate = DateTime.Now;
			int sinceValue = 0;

			if ( Since.SelectedItem != null )
			{
				sinceValue = int.Parse( Since.SelectedItem.Value );
				sinceDate = DateTime.Now;
				if ( sinceValue > 0 )
					sinceDate = DateTime.Now - TimeSpan.FromDays( sinceValue );
				else if ( sinceValue < 0 )
					sinceDate = DateTime.Now + TimeSpan.FromHours( sinceValue );
			}
			if ( sinceValue == 0 )
				sinceDate = Mession.LastVisit;


			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;

			object categoryIDObject = null;

			if ( PageContext.Settings.CategoryID != 0 ) categoryIDObject = PageContext.Settings.CategoryID;

			DataView dv = DB.topic_active( PageContext.PageBoardID, PageContext.PageUserID, sinceDate, categoryIDObject ).DefaultView;
			pds.DataSource = dv;
			PagerTop.Count = dv.Count;
			PagerTop.PageSize = 15;
			pds.PageSize = PagerTop.PageSize;

			pds.CurrentPageIndex = PagerTop.CurrentPageIndex;
			TopicList.DataSource = pds;

			DataBind();
		}

		protected string PrintForumName( DataRowView row )
		{
			string forumName = (string)row["ForumName"];
			string html = "";
			if ( forumName != _lastForumName )
			{
				html = String.Format( @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"">{0}</a></td></tr>", forumName, YafBuildLink.GetLink( ForumPages.topics, "f={0}", row["ForumID"] ) );
				_lastForumName = forumName;
			}
			return html;
		}

		protected void Since_SelectedIndexChanged( object sender, System.EventArgs e )
		{
			PagerTop.CurrentPageIndex = 0;
			Mession.ActiveTopicSince = Convert.ToInt32( Since.SelectedValue );
			BindData();
		}
	}
}
