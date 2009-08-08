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
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Pager.
	/// </summary>
	public class Pager : BaseControl, System.Web.UI.IPostBackEventHandler
	{
		private PopupControlExtender _popupControlExt = new PopupControlExtender();
		private Label _pageLabel = new Label();
		private GotoPageForm _gotoPageForm = new GotoPageForm();
		private bool _usePostBack = true;
		private bool _ignorePageIndex = false;

		protected Pager CurrentLinkedPager
		{
			get
			{
				if ( LinkedPager != null )
				{
					Pager linkedPager = ( Pager )Parent.FindControl( LinkedPager );

					if ( linkedPager == null )
					{
						throw new Exception( string.Format( "Failed to link pager to '{0}'.", LinkedPager ) );
					}
					return linkedPager;
				}

				return null;
			}
		}

		public Pager()
		{
			this.Init += new EventHandler( Pager_Init );
		}

		void Pager_Init( object sender, EventArgs e )
		{
			if ( !_ignorePageIndex && System.Web.HttpContext.Current.Request.QueryString ["p"] != null )
			{
				// set a new page...
				CurrentPageIndex = ( int )Security.StringToLongOrRedirect( System.Web.HttpContext.Current.Request.QueryString ["p"] ) - 1;
			}

			_pageLabel.ID = GetExtendedID( "PageLabel" );
			_gotoPageForm.ID = GetExtendedID( "GotoPageForm" );
			_popupControlExt.ID = GetExtendedID( "PopupControlExt" );

			this.Controls.Add( _pageLabel );
			this.Controls.Add( _popupControlExt );
			this.Controls.Add( _gotoPageForm );

			_popupControlExt.TargetControlID = _pageLabel.ID;
			_popupControlExt.PopupControlID = _gotoPageForm.ID;
			_popupControlExt.Position = PopupControlPopupPosition.Bottom;

			// init the necessary js...
			ScriptManager.RegisterClientScriptInclude( this, typeof( Pager ), "yafjs", YAF.Classes.Utils.YafForumInfo.GetURLToResource( "js/yaf.js" ) );

			// change the cursor to hand when over link...
			_pageLabel.Attributes.Add( "onmouseover", @"yaf_mouseover()" );
			_popupControlExt.Animations = @"<OnHide><Sequence><FadeOut duration=""0.3"" fps=""30"" minimumOpacity=""0""></FadeOut><HideAction Visible=""false"" /></Sequence></OnHide><OnShow><Sequence><HideAction Visible=""true"" /><FadeIn duration=""0.3"" fps=""30""></FadeIn></Sequence></OnShow>";

			// hook up events...
			_gotoPageForm.GotoPageClick += new EventHandler<GotoPageForumEventArgs>( _gotoPageForm_GotoPageClick );
		}

		void _gotoPageForm_GotoPageClick( object sender, GotoPageForumEventArgs e )
		{
			int newPage = e.GotoPage-1;

			if ( newPage >= 0 && newPage < PageCount )
			{
				// set a new page index...
				CurrentPageIndex = newPage;
				_ignorePageIndex = true;
			}

			if ( LinkedPager != null )
			{
				// raise post back event on the linked pager...
				CurrentLinkedPager._gotoPageForm_GotoPageClick( sender, e );
			}
			else if ( PageChange != null )
			{
				PageChange( this, new EventArgs() );
			}
		}

		protected void CopyPagerSettings( Pager toPager )
		{
			toPager.Count = this.Count;
			toPager.CurrentPageIndex = this.CurrentPageIndex;
			toPager.PageSize = this.PageSize;
		}

		protected override void Render( HtmlTextWriter output )
		{
			if ( LinkedPager != null )
			{
				// just copy the linked pager settings but still render in this function...
				CurrentLinkedPager.CopyPagerSettings( this );
			}

			if ( PageCount < 2 ) return;

			output.WriteLine( String.Format( @"<div class=""yafpager"" id=""{0}"">", this.ClientID ) );

			_popupControlExt.RenderControl( output );

			output.WriteLine( @"<span class=""pagecount"">" );

			// have to be careful about localization because the pager is used in the admin pages...
			string pagesText = "Pages";
			if ( !String.IsNullOrEmpty( PageContext.Localization.TransPage ) )
			{
				pagesText = PageContext.Localization.GetText( "COMMON", "PAGES" );
			}
			
			_pageLabel.Text = String.Format( @"{0:N0} {1}", PageCount, pagesText );

			// render this control...
			_pageLabel.RenderControl( output );

			output.WriteLine( @"</span>" );

			OutputLinks( output, UsePostBack );

			_gotoPageForm.RenderControl( output );

			output.WriteLine( "</div>" );

			//base.Render( output );
		}

		private string GetLinkUrl( int pageNum, bool postBack )
		{
			if ( postBack )
			{
				return Page.ClientScript.GetPostBackClientHyperlink( this, pageNum.ToString() );
			}
			return GetPageURL( pageNum );
		}

		private void OutputLinks( HtmlTextWriter output, bool postBack )
		{
			int iStart = CurrentPageIndex - 2;
			int iEnd = CurrentPageIndex + 3;
			if ( iStart < 0 ) iStart = 0;
			if ( iEnd > PageCount ) iEnd = PageCount;

			if ( iStart > 0 )
			{
				output.WriteBeginTag( "span" );
				output.WriteAttribute( "class", "pagelinkfirst" );
				output.Write( HtmlTextWriter.TagRightChar );

				this.RenderAnchorBegin( output, GetLinkUrl( 1, postBack ), null, "Go to First Page" );

				output.Write( "&laquo;" );
				output.WriteEndTag( "a" );
				output.WriteEndTag( "span" );
			}

			if ( CurrentPageIndex > iStart )
			{
				output.WriteBeginTag( "span" );
				output.WriteAttribute( "class", "pagelink" );
				output.Write( HtmlTextWriter.TagRightChar );

				this.RenderAnchorBegin( output, GetLinkUrl( CurrentPageIndex, postBack ), null, "Prev Page" );

				output.Write( "&lt;" );
				output.WriteEndTag( "a" );
				output.WriteEndTag( "span" );
			}

			for ( int i = iStart; i < iEnd; i++ )
			{
				if ( i == CurrentPageIndex )
				{
					output.WriteBeginTag( "span" );
					output.WriteAttribute( "class", "pagecurrent" );
					output.Write( HtmlTextWriter.TagRightChar );
					output.Write( i + 1 );
					output.WriteEndTag( "span" );
				}
				else
				{
					string page = ( i + 1 ).ToString();

					output.WriteBeginTag( "span" );
					output.WriteAttribute( "class", "pagelink" );
					output.Write( HtmlTextWriter.TagRightChar );

					this.RenderAnchorBegin( output, GetLinkUrl( i + 1, postBack ), null, page );

					output.Write( page );
					output.WriteEndTag( "a" );
					output.WriteEndTag( "span" );
				}
			}

			if ( CurrentPageIndex < ( PageCount - 1 ) )
			{
				output.WriteBeginTag( "span" );
				output.WriteAttribute( "class", "pagelink" );
				output.Write( HtmlTextWriter.TagRightChar );

				this.RenderAnchorBegin( output, GetLinkUrl( CurrentPageIndex + 2, postBack ), null, "Next Page" );

				output.Write( "&gt;" );
				output.WriteEndTag( "a" );
				output.WriteEndTag( "span" );
			}

			if ( iEnd < PageCount )
			{
				output.WriteBeginTag( "span" );
				output.WriteAttribute( "class", "pagelinklast" );
				output.Write( HtmlTextWriter.TagRightChar );

				this.RenderAnchorBegin( output, GetLinkUrl( PageCount, postBack ), null, "Go to Last Page" );

				output.Write( "&raquo;" );
				output.WriteEndTag( "a" );
				output.WriteEndTag( "span" );
			}
		}

		protected string GetPageURL( int page )
		{
			string url = "";

			// create proper query string...
			SimpleURLParameterParser parser = new SimpleURLParameterParser( System.Web.HttpContext.Current.Request.QueryString.ToString() );

			// get the current page
			ForumPages currentPage = ( ForumPages )Enum.Parse( typeof( ForumPages ), parser ["g"], true );

			if ( parser ["m"] != null )
			{
				// must be converted to by topic...
				parser.Parameters.Remove( "m" );
				parser.Parameters.Add( "t", YafContext.Current.PageTopicID.ToString() );
			}

			if ( page > 1 )
			{
				string tmp = parser.CreateQueryString( new string [] { "g", "p", "tabid", "find" } );
				if ( tmp.Length > 0 ) tmp += "&";

				tmp += "p={0}";

				url = YafBuildLink.GetLink( currentPage, tmp, page );
			}
			else
			{
				url = YafBuildLink.GetLink( currentPage, parser.CreateQueryString( new string [] { "g", "p", "tabid", "find" } ) );
			}

			return url;
		}

		public bool UsePostBack
		{
			get
			{
				return _usePostBack;
			}
			set
			{
				_usePostBack = value;
			}
		}

		public int Count
		{
			get
			{
				if ( ViewState ["Count"] != null )
					return ( int )ViewState ["Count"];
				else
					return 0;
			}
			set
			{
				ViewState ["Count"] = value;
			}
		}

		public int CurrentPageIndex
		{
			get
			{
				if ( ViewState ["CurrentPageIndex"] != null )
					return ( int )ViewState ["CurrentPageIndex"];
				else
					return 0;
			}
			set
			{
				ViewState ["CurrentPageIndex"] = value;
			}
		}

		public int PageSize
		{
			get
			{
				if ( ViewState ["PageSize"] != null )
					return ( int )ViewState ["PageSize"];
				else
					return 20;
			}
			set
			{
				ViewState ["PageSize"] = value;
			}
		}

		public int PageCount
		{
			get
			{
				return ( int )Math.Ceiling( ( double )Count / PageSize );
			}
		}

		public string LinkedPager
		{
			get
			{
				return ( string )ViewState ["LinkedPager"];
			}
			set
			{
				ViewState ["LinkedPager"] = value;
			}
		}

		#region IPostBackEventHandler
		public event EventHandler PageChange;

		public void RaisePostBackEvent( string eventArgument )
		{
			if ( LinkedPager != null )
			{
				// raise post back event on the linked pager...
				CurrentLinkedPager.RaisePostBackEvent( eventArgument );
			}
			else if ( PageChange != null )
			{
				CurrentPageIndex = ( int.Parse( eventArgument ) - 1 );
				_ignorePageIndex = true;
				PageChange( this, new EventArgs() );
			}
		}
		#endregion
	}
}
