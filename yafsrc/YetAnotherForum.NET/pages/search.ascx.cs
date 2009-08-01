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
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;
using HistoryEventArgs=nStuff.UpdateControls.HistoryEventArgs;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for topics.
	/// </summary>
	public partial class search : YAF.Classes.Base.ForumPage
	{
		private bool _searchHandled = false;

		/// <summary>
		/// The search page constructor.
		/// </summary>
		public search()
			: base( "SEARCH" )
		{
		}

		private HtmlForm GetServerForm( ControlCollection parent )
		{
			HtmlForm tmpHtmlForm = null;

			foreach ( Control child in parent )
			{
				Type t = child.GetType();
				if ( t == typeof( System.Web.UI.HtmlControls.HtmlForm ) )
					return (HtmlForm)child;

				if ( child.HasControls() )
				{
					tmpHtmlForm = GetServerForm( child.Controls );
					if ( tmpHtmlForm != null && tmpHtmlForm.ClientID != null )
						return tmpHtmlForm;
				}
			}

			return null;
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			// check access permissions
			General.HandleRequest( PageContext, PageContext.BoardSettings.SearchPermissions );

			//Page.Reg
			//if(IsPostBack) throw new ApplicationException(Request.Form["__EVENTTARGET"]);

			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name,
				                   YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );
				btnSearch.Text = GetText( "btnsearch" );

				// Load result dropdown
				listResInPage.Items.Add( new ListItem( GetText( "result5" ), "5" ) );
				listResInPage.Items.Add( new ListItem( GetText( "result10" ), "10" ) );
				listResInPage.Items.Add( new ListItem( GetText( "result25" ), "25" ) );
				listResInPage.Items.Add( new ListItem( GetText( "result50" ), "50" ) );

				// Load searchwhere dropdown
				// listSearchWhere.Items.Add( new ListItem( GetText( "posts" ), "0" ) );
				// listSearchWhere.Items.Add( new ListItem( GetText( "postedby" ), "1" ) );

				//Load listSearchFromWho dropdown
				listSearchFromWho.Items.Add( new ListItem( GetText( "match_all" ), "0" ) );
				listSearchFromWho.Items.Add( new ListItem( GetText( "match_any" ), "1" ) );
				listSearchFromWho.Items.Add( new ListItem( GetText( "match_exact" ), "2" ) );

				// Load listSearchWhat dropdown
				listSearchWhat.Items.Add( new ListItem( GetText( "match_all" ), "0" ) );
				listSearchWhat.Items.Add( new ListItem( GetText( "match_any" ), "1" ) );
				listSearchWhat.Items.Add( new ListItem( GetText( "match_exact" ), "2" ) );


				listSearchFromWho.SelectedIndex = 0;
				listSearchWhat.SelectedIndex = 0;

				// handle custom BBCode javascript or CSS...
				BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );

				//Load forum's combo
				//listForum.Items.Add( new ListItem( GetText( "allforums" ), "-1" ) );
				//DataTable dt = YAF.Classes.Data.DB.forum_listread( PageContext.PageBoardID, PageContext.PageUserID, null, null );

				//int nOldCat = 0;
				//for ( int i = 0; i < dt.Rows.Count; i++ )
				//{
				//    DataRow row = dt.Rows [i];
				//    if ( ( int ) row ["CategoryID"] != nOldCat )
				//    {
				//        nOldCat = ( int ) row ["CategoryID"];
				//        listForum.Items.Add( new ListItem( ( string ) row ["Category"], "-1" ) );
				//    }
				//    listForum.Items.Add( new ListItem( " - " + ( string ) row ["Forum"], row ["ForumID"].ToString() ) );
				//}

				LoadingModal.HeaderText = GetText( "LOADING" );
				LoadingModal.MainText = GetText( "LOADING_SEARCH" );

				listForum.DataSource = DB.forum_listall_sorted( PageContext.PageBoardID, PageContext.PageUserID );
				listForum.DataValueField = "ForumID";
				listForum.DataTextField = "Title";
				listForum.DataBind();
				listForum.Items.Insert( 0, new ListItem( GetText( "allforums" ), "0" ) );

				bool doSearch = false;

				string searchString = Request.QueryString["search"];
				if ( !String.IsNullOrEmpty( searchString ) && searchString.Length < 50 )
				{
					txtSearchStringWhat.Text = searchString;
					doSearch = true;
				}

				string postedBy = Request.QueryString["postedby"];
				if ( !String.IsNullOrEmpty( postedBy ) && postedBy.Length < 50 )
				{
					txtSearchStringFromWho.Text = postedBy;
					doSearch = true;
				}

				if ( doSearch ) SearchBindData( true );
			}
		}

		protected void Pager_PageChange( object sender, EventArgs e )
		{
			SmartScroller1.RegisterStartupReset();
			SearchBindData( false );
		}

		private void SearchBindData( bool newSearch )
		{
			try
			{
				if ( newSearch && !CheckSearchRequest() )
				{
					return;
				}
				else if ( newSearch || Mession.SearchData == null )
				{
					SearchWhatFlags sw = (SearchWhatFlags)System.Enum.Parse( typeof( SearchWhatFlags ), listSearchWhat.SelectedValue );
					SearchWhatFlags sfw = (SearchWhatFlags)System.Enum.Parse( typeof( SearchWhatFlags ), listSearchFromWho.SelectedValue );
					int forumID = int.Parse( listForum.SelectedValue );

					DataTable searchDataTable = YAF.Classes.Data.DB.GetSearchResult( txtSearchStringWhat.Text,
					                                                                 txtSearchStringFromWho.Text, sfw, sw, forumID,
					                                                                 PageContext.PageUserID, PageContext.PageBoardID,
					                                                                 PageContext.BoardSettings.ReturnSearchMax,
					                                                                 PageContext.BoardSettings.UseFullTextSearch );
					Pager.CurrentPageIndex = 0;
					Pager.PageSize = int.Parse( listResInPage.SelectedValue );
					Pager.Count = searchDataTable.DefaultView.Count;
					Mession.SearchData = searchDataTable;

					bool bResults = (searchDataTable.DefaultView.Count > 0) ? true : false;

					SearchRes.Visible = bResults;
					NoResults.Visible = !bResults;
				}

				PagedDataSource pds = new PagedDataSource();
				pds.AllowPaging = true;
				pds.DataSource = Mession.SearchData.DefaultView;
				pds.PageSize = Pager.PageSize;
				pds.CurrentPageIndex = Pager.CurrentPageIndex;

				UpdateHistory.AddEntry( Pager.CurrentPageIndex.ToString() + "|" + Pager.PageSize );

				SearchRes.DataSource = pds;
				SearchRes.DataBind();
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				CreateMail.CreateLogEmail( x );

				if ( PageContext.IsAdmin )
				{
					PageContext.AddLoadMessage( string.Format( "{0}", x ) );
				}
				else
				{
					PageContext.AddLoadMessage( "An error occured while searching." );
				}
			}
		}

		protected void btnSearch_Click( object sender, System.EventArgs e )
		{
			SearchUpdatePanel.Visible = true;
			SearchBindData( true );
		}

		protected void SearchRes_ItemDataBound( object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e )
		{
			HtmlTableCell cell = (HtmlTableCell)e.Item.FindControl( "CounterCol" );
			if ( cell != null )
			{
				string messageID = cell.InnerText;
				int rowCount = e.Item.ItemIndex + 1 + (Pager.CurrentPageIndex * Pager.PageSize);
				cell.InnerHtml = string.Format( "<a href=\"{1}\">{0}</a>", rowCount, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", messageID ) );
			}
		}

		protected bool CheckSearchRequest()
		{
			bool whatValid = CheckSearchText( txtSearchStringWhat.Text ),
				whoValid = CheckSearchText( txtSearchStringFromWho.Text ),
				whatNull = txtSearchStringWhat.Text.Trim().Length == 0,
				whoNull = txtSearchStringFromWho.Text.Trim().Length == 0;

			if ( (!whoNull && whoValid && ((whatNull && !whatValid) || (!whatNull && whatValid))) ||
				(whoNull && !whoValid && !whatNull && whatValid) )
			{
				return true;
			}
			else
			{
				if ( !_searchHandled )
					PageContext.AddLoadMessage( GetTextFormatted( "SEARCH_CRITERIA_ERROR",
																										 PageContext.BoardSettings.SearchStringMinLength ) );
				_searchHandled = true;
				return false;
			}
		}

		protected bool CheckSearchText(string searchText)
		{
			return searchText.Trim().Length >= PageContext.BoardSettings.SearchStringMinLength &&
				Regex.IsMatch( searchText, PageContext.BoardSettings.SearchStringPattern );
		}

		protected void OnUpdateHistoryNavigate( object sender, HistoryEventArgs e )
		{
			int pageNumber, pageSize;

			string[] pagerData = e.EntryName.Split( '|' );

			if ( pagerData.Length >= 2 && int.TryParse( pagerData[0], out pageNumber ) && int.TryParse( pagerData[1], out pageSize ) && Mession.SearchData != null )
			{
				// use existing page...
				Pager.CurrentPageIndex = pageNumber;

				// and existing page size...
				Pager.PageSize = pageSize;

				// count...
				Pager.Count = Mession.SearchData.DefaultView.Count;

				// bind existing search
				SearchBindData( false );

				// use existing search data...
				SearchUpdatePanel.Update();
			}
		}
	}
}
