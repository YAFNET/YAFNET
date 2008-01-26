/* Yet Another Forum.NET
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
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Pager.
	/// </summary>
	public class Pager : BaseControl, System.Web.UI.IPostBackEventHandler
	{
		private bool _usePostBack = true;
		private bool _ignorePageIndex = false;

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
		}

		protected override void Render( HtmlTextWriter output )
		{
			if ( LinkedPager != null )
			{
				Pager linkedPager = ( Pager ) Parent.FindControl( LinkedPager );
				if ( linkedPager == null )
					throw new Exception( string.Format( "Failed to link pager to '{0}'.", LinkedPager ) );
				linkedPager.Render( output );
				return;
			}

			if ( PageCount < 2 ) return;

			output.WriteLine( @"<div class=""yafpager"">" );
			output.WriteLine( @"<span class=""pagecount"">{0:N0} Pages</span>", PageCount );

			OutputLinks( output, UsePostBack );

			output.WriteLine( "</div>" );
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
				output.Write( HtmlTextWriter.SelfClosingTagEnd );

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

			if ( CurrentPageIndex < (PageCount-1) )
			{
				output.WriteBeginTag( "span" );
				output.WriteAttribute( "class", "pagelink" );
				output.Write( HtmlTextWriter.TagRightChar );

				this.RenderAnchorBegin( output, GetLinkUrl( CurrentPageIndex + 2, postBack ), null, "Next Page" );

				output.Write( "&gt;");
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

		protected string GetPageURL(int page)
		{
			string url = "";

			// create proper query string...
			SimpleURLParameterParser parser = new SimpleURLParameterParser( System.Web.HttpContext.Current.Request.QueryString.ToString() );

			// get the current page
      YAF.Classes.Utils.ForumPages currentPage = ( YAF.Classes.Utils.ForumPages ) Enum.Parse( typeof( YAF.Classes.Utils.ForumPages ), parser ["g"], true );

			if ( parser ["m"] != null )
			{
				// must be converted to by topic...
				parser.Parameters.Remove( "m" );
				parser.Parameters.Add( "t", YafContext.Current.PageTopicID.ToString() );
			}

			if ( page > 1 )
			{
        string tmp = parser.CreateQueryString( new string [] { "g", "p", "tabid", "find" } );
				if (tmp.Length > 0) tmp += "&";

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
					return ( int ) ViewState ["Count"];
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
					return ( int ) ViewState ["CurrentPageIndex"];
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
					return ( int ) ViewState ["PageSize"];
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
				return ( int ) Math.Ceiling( ( double ) Count / PageSize );
			}
		}

		public string LinkedPager
		{
			get
			{
				return ( string ) ViewState ["LinkedPager"];
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
			if ( PageChange != null )
			{
				CurrentPageIndex = (int.Parse( eventArgument ) - 1);
				_ignorePageIndex = true;
				PageChange( this, new EventArgs() );
			}
		}
		#endregion
	}
}
