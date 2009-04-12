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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
	/// <summary>
	/// Provides a image with themed src
	/// </summary>
	public class ThemeImage : BaseControl
	{
		protected string _alt = string.Empty;
		protected string _themePage = "ICONS";
		protected string _themeTag = string.Empty;
		protected string _style = string.Empty;
		protected string _localizedTitlePage = string.Empty;
		protected string _localizedTitleTag = string.Empty;
		protected bool _useTitleForEmptyAlt = true;

		public ThemeImage() : base()
		{

		}

		protected override void Render( HtmlTextWriter output )
		{
			string src = this.GetCurrentThemeItem();
			string title = this.GetCurrentTitleItem();

			// might not be needed...
			if ( String.IsNullOrEmpty( src ) ) return;

			if ( UseTitleForEmptyAlt && String.IsNullOrEmpty( Alt ) && !String.IsNullOrEmpty(title) )
			{
				Alt = title;
			}

			output.BeginRender();
			output.WriteBeginTag( "img" );
			output.WriteAttribute( "id", this.ClientID );

			// this will output the src and alt attributes
			output.WriteAttribute( "src", src );
			output.WriteAttribute( "alt", Alt );

			if ( !String.IsNullOrEmpty( Style ) ) output.WriteAttribute( "style", Style );
			if ( !String.IsNullOrEmpty( title ) ) output.WriteAttribute( "title", title );

			// self closing end tag "/>"
			output.Write( HtmlTextWriter.SelfClosingTagEnd );

			output.EndRender();
		}

		protected string GetCurrentTitleItem()
		{
			if ( !String.IsNullOrEmpty( _localizedTitlePage ) && !String.IsNullOrEmpty( _localizedTitleTag ) )
			{
				return PageContext.Localization.GetText( _localizedTitlePage, _localizedTitleTag );
			}
			else if ( !String.IsNullOrEmpty( _localizedTitleTag ) )
			{
				return PageContext.Localization.GetText( _localizedTitleTag );
			}

			return null;
		}

		protected string GetCurrentThemeItem()
		{
			if ( !String.IsNullOrEmpty( _themePage ) && !String.IsNullOrEmpty( _themeTag ) )
			{
				return PageContext.Theme.GetItem( ThemePage, ThemeTag, null );
			}

			return null;
		}

		/// <summary>
		/// Set/Get the ThemePage -- Defaults to "ICONS"
		/// </summary>
		public string ThemePage
		{
			get { return _themePage; }
			set { _themePage = value; }
		}

		/// <summary>
		/// Set/Get the actual theme item
		/// </summary>
		public string ThemeTag
		{
			get { return _themeTag; }
			set { _themeTag = value; }
		}

		public string Alt
		{
			get { return _alt; }
			set { _alt = value; }
		}

		public bool UseTitleForEmptyAlt
		{
			get { return _useTitleForEmptyAlt; }
			set { _useTitleForEmptyAlt = value; }
		}

		public string LocalizedTitlePage
		{
			get { return _localizedTitlePage; }
			set { _localizedTitlePage = value; }
		}

		public string LocalizedTitleTag
		{
			get { return _localizedTitleTag; }
			set { _localizedTitleTag = value; }
		}

		public string Style
		{
			get { return _style; }
			set { _style = value; }
		}

	}
}
