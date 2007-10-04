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

		public ThemeImage() : base()
		{

		}

		protected override void Render( HtmlTextWriter output )
		{
			string src = this.GetCurrentItem();

			output.BeginRender();
			output.WriteBeginTag( "img" );

			// this will output the src and alt attributes
			output.WriteAttribute( "src", src );
			output.WriteAttribute( "alt", Alt );

			if ( !String.IsNullOrEmpty( Style ) ) output.WriteAttribute( "style", Style );

			// self closing end tag "/>"
			output.Write( HtmlTextWriter.SelfClosingTagEnd );

			output.EndRender();
		}

		protected string GetCurrentItem()
		{
			if ( !String.IsNullOrEmpty( _themePage ) && !String.IsNullOrEmpty( _themeTag ) )
			{
				return PageContext.Theme.GetItem( ThemePage, ThemeTag );
			}

			return null;
		}

		public string ThemePage
		{
			get { return _themePage; }
			set { _themePage = value; }
		}

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

		public string Style
		{
			get { return _style; }
			set { _style = value; }
		}

	}
}
