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
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	public class MessageSignature : BaseControl
	{
		public MessageSignature()
			: base()
		{

		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.BeginRender();
			writer.WriteBeginTag( "div" );
			writer.WriteAttribute( "id", this.ClientID );
			writer.WriteAttribute( "class", "yafsignature" );
			writer.Write( HtmlTextWriter.TagRightChar );

			if ( !String.IsNullOrEmpty( HtmlPrefix ) ) writer.Write( HtmlPrefix );
			if ( !String.IsNullOrEmpty( Signature ) ) RenderSignature( writer );			
			if ( !String.IsNullOrEmpty( HtmlSuffix ) ) writer.Write( HtmlSuffix );

			base.Render( writer );

			writer.WriteEndTag( "div" );
			writer.EndRender();
		}

		protected void RenderSignature( HtmlTextWriter writer )
		{
			// don't allow any HTML on signatures
			MessageFlags tFlags = new MessageFlags();
			tFlags.IsHtml = false;

			writer.Write( FormatMsg.FormatMessage( this.Signature, tFlags ) );
		}

		private string _signature;
		public string Signature
		{
			get { return _signature; }
			set { _signature = value; }
		}

		private string _htmlPrefix;
		public string HtmlPrefix
		{
			get { return _htmlPrefix; }
			set { _htmlPrefix = value; }
		}

		private string _htmlSuffix;
		public string HtmlSuffix
		{
			get { return _htmlSuffix; }
			set { _htmlSuffix = value; }
		}
	}
}