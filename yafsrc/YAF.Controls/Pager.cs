/* Yet Another Forum.NET
 * Copyright (C) 2006-2007 Jaben Cargman
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

			output.WriteLine( "<span>" );
			output.WriteLine( "{0:N0} pages:", PageCount );

			if ( UsePostBack ) OutputPostback( output );
			else OutputLinked( output );

			output.WriteLine( "</span>" );
		}

		private void OutputPostback( HtmlTextWriter output )
		{
			int iStart = CurrentPageIndex - 6;
			int iEnd = CurrentPageIndex + 7;
			if ( iStart < 0 ) iStart = 0;
			if ( iEnd > PageCount ) iEnd = PageCount;

			if ( iStart > 0 )
				output.WriteLine( "<a href=\"{0}\">First</a> ...", Page.ClientScript.GetPostBackClientHyperlink( this, "0" ) );

			for ( int i = iStart; i < iEnd; i++ )
			{
				if ( i == CurrentPageIndex )
					output.WriteLine( "[{0}]", i + 1 );
				else
					output.WriteLine( "<a href=\"{0}\">{1}</a>", Page.ClientScript.GetPostBackClientHyperlink( this, i.ToString() ), i + 1 );
			}

			if ( iEnd < PageCount )
				output.WriteLine( "... <a href=\"{0}\">Last</a>", Page.ClientScript.GetPostBackClientHyperlink( this, ( PageCount - 1 ).ToString() ) );
		}

		private void OutputLinked( HtmlTextWriter output )
		{
			int iStart = CurrentPageIndex - 6;
			int iEnd = CurrentPageIndex + 7;
			if ( iStart < 0 ) iStart = 0;
			if ( iEnd > PageCount ) iEnd = PageCount;

			if ( iStart > 0 )
				output.WriteLine( "<a href=\"{0}\">First</a> ...", GetPageURL(0) );

			for ( int i = iStart; i < iEnd; i++ )
			{
				if ( i == CurrentPageIndex )
					output.WriteLine( "[{0}]", i + 1 );
				else
					output.WriteLine( "<a href=\"{0}\">{1}</a>", GetPageURL( i+1 ), i + 1 );
			}

			if ( iEnd < PageCount )
				output.WriteLine( "... <a href=\"{0}\">Last</a>", GetPageURL(PageCount) );
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
        string tmp = parser.CreateQueryString( new string [] { "g", "p", "tabid" } );
				if (tmp.Length > 0) tmp += "&";

				tmp += "p={0}";

				url = YafBuildLink.GetLink( currentPage, tmp, page );
			}
			else
			{
        url = YafBuildLink.GetLink( currentPage, parser.CreateQueryString( new string [] { "g", "p", "tabid" } ) );
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
				CurrentPageIndex = int.Parse( eventArgument );
				_ignorePageIndex = true;
				PageChange( this, new EventArgs() );
			}
		}
		#endregion
	}
}
