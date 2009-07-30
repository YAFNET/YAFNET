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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class active : YAF.Classes.Base.ForumPage
	{
		protected string _lastForumName = string.Empty;

		public active()
			: base( "ACTIVE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( PageContext.IsPrivate && User == null )
			{
				// Ederon : guess we don't need this if anymore
				//if ( CanLogin )
					RedirectNoAccess();
				//else
				//	YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );
			}

			if ( PageContext.BoardSettings.ShowRSSLink )
			{
				RssFeed.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.rsstopic, "pg=active" );
				RssFeed.Text = GetText( "RSSFEED" );
				RssFeed.Visible = true;
			}
			else
			{
				RssFeed.Visible = false;
			}

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

				Since.Items.Add( new ListItem( GetTextFormatted( "last_visit", YafDateTime.FormatDateTime( Mession.LastVisit ) ), "0" ) );
				Since.Items.Add( new ListItem( GetText( "last_hour" ), "-1" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_hours" ), "-2" ) );
				Since.Items.Add( new ListItem( GetText( "last_day" ), "1" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_days" ), "2" ) );
				Since.Items.Add( new ListItem( GetText( "last_week" ), "7" ) );
				Since.Items.Add( new ListItem( GetText( "last_two_weeks" ), "14" ) );
				Since.Items.Add( new ListItem( GetText( "last_month" ), "31" ) );
                if (Session["ActiveTopicSince"] != null)
                { Since.SelectedIndex = Convert.ToInt32(Session["ActiveTopicSince"]); }
                else
                    Since.SelectedIndex = 0;
			}
			BindData();
		}

		protected void Pager_PageChange( object sender, EventArgs e )
		{
			BindData();
		}

		private void BindData()
		{
			DateTime SinceDate = DateTime.Now;
			int SinceValue = 0;

			if ( Since.SelectedItem != null )
			{
				SinceValue = int.Parse( Since.SelectedItem.Value );
				SinceDate = DateTime.Now;
				if ( SinceValue > 0 )
					SinceDate = DateTime.Now - TimeSpan.FromDays( SinceValue );
				else if ( SinceValue < 0 )
					SinceDate = DateTime.Now + TimeSpan.FromHours( SinceValue );
			}
			if ( SinceValue == 0 )
				SinceDate = Mession.LastVisit;


			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;

			object categoryIDObject = null;

			if ( PageContext.Settings.CategoryID != 0 ) categoryIDObject = PageContext.Settings.CategoryID;

			DataView dv = YAF.Classes.Data.DB.topic_active( PageContext.PageBoardID, PageContext.PageUserID, SinceDate, categoryIDObject ).DefaultView;
			pds.DataSource = dv;
			Pager.Count = dv.Count;
			Pager.PageSize = 15;
			pds.PageSize = Pager.PageSize;

			pds.CurrentPageIndex = Pager.CurrentPageIndex;
			TopicList.DataSource = pds;

			DataBind();
		}

		protected string PrintForumName( DataRowView row )
		{
			string forumName = ( string )row ["ForumName"];
			string html = "";
			if ( forumName != _lastForumName )
			{
				html = String.Format( @"<tr><td class=""header2"" colspan=""6""><a href=""{1}"">{0}</a></td></tr>", forumName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", row ["ForumID"] ) );
				_lastForumName = forumName;
			}
			return html;
		}

		protected void Since_SelectedIndexChanged( object sender, System.EventArgs e )
		{
            Pager.CurrentPageIndex = 0;
            Session["ActiveTopicSince"] = Since.SelectedIndex;
			BindData();
		}
	}
}
