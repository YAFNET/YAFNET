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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	/// Shows a Message Post
	/// </summary>
	public class MessagePost : MessageBase
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
				sig.DisplayUserID = this.DisplayUserID;
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
			if ( this.MessageFlags.IsDeleted )
			{
				// deleted message text...
				RenderDeletedMessage( writer );
			}
			else if ( this.MessageFlags.NotFormatted )
			{
				writer.Write( this.Message );
			}
			else
			{
				string formattedMessage = FormatMsg.FormatMessage( this.Message, this.MessageFlags );

				if ( this.MessageFlags.IsBBCode )
					RenderModulesInBBCode( writer, formattedMessage, this.MessageFlags, this.DisplayUserID );
				else
					writer.Write( formattedMessage );
			}
		}

		virtual protected void RenderDeletedMessage( HtmlTextWriter writer )
		{
			// if message was deleted then write that instead of real body
			if ( MessageFlags.IsDeleted )
			{
				if ( IsModeratorChanged )
				{
					// deleted by mod
					writer.Write( PageContext.Localization.GetText( "POSTS", "MESSAGEDELETED_MOD" ) );
				}
				else
				{
					// deleted by user
					writer.Write( PageContext.Localization.GetText( "POSTS", "MESSAGEDELETED_USER" ) );
				}
			}
		}

		virtual public int? DisplayUserID
		{
			get
			{
				if ( ViewState ["DisplayUserID"] != null )
					return Convert.ToInt32( ViewState ["DisplayUserID"] );

				return null;
			}
			set { ViewState ["DisplayUserID"] = value; }
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

		virtual public bool IsModeratorChanged
		{
			get
			{
				if ( ViewState ["IsModeratorChanged"] != null )
					return Convert.ToBoolean(ViewState ["IsModeratorChanged"]);

				return false;
			}
			set { ViewState ["IsModeratorChanged"] = value; }
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