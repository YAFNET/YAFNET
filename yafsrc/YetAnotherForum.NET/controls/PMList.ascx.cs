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
using System.Data;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Controls
{

	public partial class PMList : YAF.Classes.Base.BaseUserControl
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( ViewState ["SortField"] == null )
				SetSort( "Created", false );

			if ( !IsPostBack )
			{
				// setup pager...
				MessagesView.AllowPaging = true;
				MessagesView.PagerSettings.Visible = false;
				MessagesView.AllowSorting = true;

				PagerTop.PageSize = 10;
				MessagesView.PageSize = 10;
			}

			BindData();
		}

		/// <summary>
		/// Gets or sets the current view for the user's private messages.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Gets or sets the current view for the user's private messages." )]
		public PMView View
		{
			get
			{
				if ( ViewState ["View"] != null )
					return ( PMView ) ViewState ["View"];
				else
					return PMView.Inbox;
			}
			set { ViewState ["View"] = value; }
		}

		protected string GetTitle()
		{
			if ( View == PMView.Outbox )
				return GetLocalizedText( "SENTITEMS" );
			else if ( View == PMView.Inbox )
				return GetLocalizedText( "INBOX" );
			else
				return GetLocalizedText( "ARCHIVE" );
		}

		protected string GetLocalizedText( string text )
		{
			return HtmlEncode( PageContext.Localization.GetText( text ) );
		}

		protected string GetMessageUserHeader()
		{
			return GetLocalizedText( View == PMView.Outbox ? "to" : "from" );
		}

		protected string GetMessageLink( object messageId )
		{
			return YafBuildLink.GetLink( ForumPages.cp_message, "pm={0}&v={1}", messageId,
										 PMViewConverter.ToQueryStringParam( View ) );
		}

		protected string FormatBody( object o )
		{
			DataRowView row = ( DataRowView ) o;
			return ( string ) row ["Body"];
		}

		private void BindData()
		{
			object toUserID = null;
			object fromUserID = null;
			if ( View == PMView.Outbox )
				fromUserID = PageContext.PageUserID;
			else
				toUserID = PageContext.PageUserID;
			using ( DataView dv = DB.pmessage_list( toUserID, fromUserID, null ).DefaultView )
			{
				if ( View == PMView.Outbox )
					dv.RowFilter = "IsInOutbox = True";
				else if ( View == PMView.Archive )
					dv.RowFilter = "IsArchived = True";
				else
					dv.RowFilter = "IsArchived = False";

				dv.Sort = String.Format( "{0} {1}", ViewState ["SortField"], ( bool ) ViewState ["SortAsc"] ? "asc" : "desc" );

				PagerTop.Count = dv.Count;

				MessagesView.PageIndex = PagerTop.CurrentPageIndex;
				MessagesView.DataSource = dv;
				MessagesView.DataBind();
			}
		}

		protected void ArchiveSelected_Click( object source, EventArgs e )
		{
			if ( this.View != PMView.Inbox )
				return;

			long archivedCount = 0;
			foreach ( GridViewRow item in MessagesView.Rows )
			{
				if ( ( ( CheckBox ) item.FindControl( "ItemCheck" ) ).Checked )
				{
					DB.pmessage_archive( MessagesView.DataKeys [item.RowIndex].Value );
					archivedCount++;
				}
			}

			BindData();

			if ( archivedCount == 1 )
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "MSG_ARCHIVED" ) );
			else
				PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "MSG_ARCHIVED+" ), archivedCount ) );
		}

		protected void DeleteSelected_Click( object source, EventArgs e )
		{
			long nItemCount = 0;
			foreach ( GridViewRow item in MessagesView.Rows )
			{
				if ( ( ( CheckBox ) item.FindControl( "ItemCheck" ) ).Checked )
				{
					if ( View == PMView.Outbox )
						DB.pmessage_delete( MessagesView.DataKeys [item.RowIndex].Value, true );
					else
						DB.pmessage_delete( MessagesView.DataKeys [item.RowIndex].Value );
					nItemCount++;
				}
			}

			BindData();
			if ( nItemCount == 1 )
				PageContext.AddLoadMessage( PageContext.Localization.GetText( "msgdeleted1" ) );
			else
				PageContext.AddLoadMessage(
					String.Format( PageContext.Localization.GetText( "msgdeleted2" ), nItemCount ) );
		}

		protected string GetImage( object o )
		{
			if (SqlDataLayerConverter.VerifyBool( ( ( DataRowView ) o ) ["IsRead"] ))
				return PageContext.Theme.GetItem( "ICONS", "TOPIC" );
			else
				return PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW" );
		}

		private void SetSort( string field, bool asc )
		{
			if ( ViewState ["SortField"] != null && ( string ) ViewState ["SortField"] == field )
			{
				ViewState ["SortAsc"] = !( bool ) ViewState ["SortAsc"];
			}
			else
			{
				ViewState ["SortField"] = field;
				ViewState ["SortAsc"] = asc;
			}
		}

		protected void SubjectLink_Click( object sender, EventArgs e )
		{
			SetSort( "Subject", true );
			BindData();
		}

		protected void FromLink_Click( object sender, EventArgs e )
		{
			if ( View == PMView.Outbox )
				SetSort( "ToUser", true );
			else
				SetSort( "FromUser", true );
			BindData();
		}

		protected void DateLink_Click( object sender, EventArgs e )
		{
			SetSort( "Created", false );
			BindData();
		}

		protected void DeleteSelected_Load( object sender, EventArgs e )
		{
			( ( ThemeButton ) sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", PageContext.Localization.GetText( "confirm_delete" ) );
		}

		protected void MessagesView_RowCreated( object sender, GridViewRowEventArgs e )
		{
			if ( e.Row.RowType == DataControlRowType.Header )
			{
				GridView oGridView = ( GridView ) sender;
				GridViewRow oGridViewRow = new GridViewRow( 0, 0, DataControlRowType.Header, DataControlRowState.Insert );
				TableCell oTableCell = new TableCell();

				// Add Header to top with column span of 5... no need for two tables.
				oTableCell.Text = GetTitle();
				oTableCell.CssClass = "header1";
				oTableCell.ColumnSpan = 5;
				oGridViewRow.Cells.Add( oTableCell );
				oGridView.Controls [0].Controls.AddAt( 0, oGridViewRow );

				Image SortFrom = ( Image ) e.Row.FindControl( "SortFrom" );
				Image SortSubject = ( Image ) e.Row.FindControl( "SortSubject" );
				Image SortDate = ( Image ) e.Row.FindControl( "SortDate" );

				SortFrom.Visible = ( View == PMView.Outbox )
									? ( string ) ViewState ["SortField"] == "ToUser"
									: ( string ) ViewState ["SortField"] == "FromUser";
				SortFrom.ImageUrl =
					PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );

				SortSubject.Visible = ( string ) ViewState ["SortField"] == "Subject";
				SortSubject.ImageUrl =
					PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );

				SortDate.Visible = ( string ) ViewState ["SortField"] == "Created";
				SortDate.ImageUrl =
					PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );
			}
		}

		protected void PagerTop_PageChange( object sender, EventArgs e )
		{
			// rebind
			BindData();
		}
	}

	/// <summary>
	/// Indicates the mode of the PMList.
	/// </summary>  
	public enum PMView
	{
		Inbox = 0,
		Outbox,
		Archive
	}

	/// <summary>
	/// Converts <see cref="PMView" />s to and from their URL query string representations.
	/// </summary>
	public static class PMViewConverter
	{
		/// <summary>
		/// Converts a <see cref="PMView" /> to a string representation appropriate for inclusion in a URL query string.
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public static string ToQueryStringParam( PMView view )
		{
			if ( view == PMView.Outbox )
				return "out";
			else if ( view == PMView.Inbox )
				return "in";
			else if ( view == PMView.Archive )
				return "arch";
			else
				return null;
		}

		/// <summary>
		/// Returns a <see cref="PMView" /> based on its URL query string value.
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public static PMView FromQueryString( string param )
		{
			if ( String.IsNullOrEmpty( param ) )
				return PMView.Inbox;

			switch ( param.ToLower() )
			{
				case "out":
					return PMView.Outbox;
				case "in":
					return PMView.Inbox;
				case "arch":
					return PMView.Archive;
				default:    // Inbox by default
					return PMView.Inbox;
			}
		}
	}
}
