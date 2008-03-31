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
	/// <summary>
	/// Shows a Message Post
	/// </summary>
	public class MessagePost : BaseControl
	{
		public MessagePost()
			: base()
		{

		}

		protected override void OnPreRender( EventArgs e )
		{
			if ( !String.IsNullOrEmpty( this.Signature ) )
			{
				MessageSignature sig = new MessageSignature();
				sig.Signature = this.Signature;
				this.Controls.Add( sig );
			}

			base.OnPreRender( e );
		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.BeginRender();
			writer.WriteBeginTag( "div" );
			writer.WriteAttribute( "id", this.ClientID );
			writer.Write( HtmlTextWriter.TagRightChar );

			RenderMessage( writer );

			// render controls...
			base.Render( writer );

			writer.WriteEndTag( "div" );
			writer.EndRender();
		}

		virtual protected void RenderMessage( HtmlTextWriter writer )
		{
			if ( this.MessageFlags.NotFormatted )
			{
				writer.Write( this.Message );
			}
			else
			{
				writer.Write( FormatMsg.FormatMessage( this.Message, this.MessageFlags ) );
			}
		}

		virtual public string Signature
		{
			get
			{
				if ( ViewState ["Signature"] != null )
					return ViewState ["Signature"].ToString();

				return null;
			}
			set { ViewState ["Signature"] = value; }
		}

		virtual public string Message
		{
			get
			{
				if ( ViewState ["Message"] != null )
					return ViewState ["Message"].ToString();

				return null;
			}
			set { ViewState ["Message"] = value; }
		}

		virtual public MessageFlags MessageFlags
		{
			get
			{
				if ( ViewState ["MessageFlags"] == null )
				{
					ViewState ["MessageFlags"] = new MessageFlags( 0 );
				}
				
				return ViewState ["MessageFlags"] as MessageFlags;
			}
			set { ViewState ["MessageFlags"] = value; }
		}
	}
}