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
using System.Data;
using System.Web;
using System.Web.UI;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseControl : System.Web.UI.Control
	{
		public YafContext PageContext
		{
			get 
			{
				if ( this.Site != null && this.Site.DesignMode == true )
				{
					// design-time, return null...
					return null;
				}
				return YafContext.Current;
			}
		}

		public string HtmlEncode( object data )
		{
			return HttpContext.Current.Server.HtmlEncode( data.ToString() );
		}

		public void RenderAnchor( HtmlTextWriter writer, string href, string cssClass, string innerText )
		{
			writer.WriteBeginTag( "a" );
			writer.WriteAttribute( "href", href );
			if ( !String.IsNullOrEmpty( cssClass ) ) writer.WriteAttribute( "class", cssClass );
			writer.Write( HtmlTextWriter.TagRightChar );
			writer.Write( innerText );
			writer.WriteEndTag( "a" );
		}

		/// <summary>
		/// Creates a Unique ID
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public string GetUniqueID( string prefix )
		{
			if ( !String.IsNullOrEmpty( prefix ) )
			{
				return prefix + System.Guid.NewGuid().ToString().Substring( 0, 5 );
			}
			else
			{
				return System.Guid.NewGuid().ToString().Substring( 0, 10 );
			}
		} 

		#region Render Anchor Begin Functions
		public void RenderAnchorBegin( HtmlTextWriter writer, string href )
		{
			this.RenderAnchorBegin( writer, href, null, null );
		}
		public void RenderAnchorBegin( HtmlTextWriter writer, string href, string cssClass )
		{
			this.RenderAnchorBegin( writer, href, cssClass, null, null, null );
		}
		public void RenderAnchorBegin( HtmlTextWriter writer, string href, string cssClass, string title )
		{
			this.RenderAnchorBegin( writer, href, cssClass, title, null, null );
		}
		public void RenderAnchorBegin( HtmlTextWriter writer, string href, string cssClass, string title, string onclick, string id )
		{
			writer.WriteBeginTag( "a" );
			writer.WriteAttribute( "href", href );
			if ( !String.IsNullOrEmpty( cssClass ) ) writer.WriteAttribute( "class", cssClass );
			if ( !String.IsNullOrEmpty( title ) ) writer.WriteAttribute( "title", HtmlEncode(title) );
			if ( !String.IsNullOrEmpty( onclick ) ) writer.WriteAttribute( "onclick", onclick );
			if ( !String.IsNullOrEmpty( id ) ) writer.WriteAttribute( "id", id );
			writer.Write( HtmlTextWriter.TagRightChar );
		}
		#endregion

		public void RenderImgTag( HtmlTextWriter writer, string src, string alt, string title )
		{
			//this will output the start of the img element - <img
			writer.WriteBeginTag( "img" );

			writer.WriteAttribute( "src", src );
			writer.WriteAttribute( "alt", alt );

			if ( !String.IsNullOrEmpty( title ) ) writer.WriteAttribute( "title", title );

			writer.Write( HtmlTextWriter.SelfClosingTagEnd );
		}
	}
}
