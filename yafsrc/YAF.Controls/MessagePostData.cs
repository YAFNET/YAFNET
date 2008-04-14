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
	public class MessagePostData : MessagePost
	{
		public MessagePostData()
			: base()
		{

		}

		protected override void OnPreRender( EventArgs e )
		{
			if ( DataRow != null && !this.MessageFlags.IsDeleted )
			{
				if ( ShowAttachments && long.Parse( DataRow ["HasAttachments"].ToString() ) > 0 )
				{
					// add attached files control...
					MessageAttached attached = new MessageAttached();
					attached.MessageID = Convert.ToInt32( DataRow ["MessageID"] );
					attached.UserName = DataRow ["Username"].ToString();
					this.Controls.Add( attached );
				}

				// add signature control if necessary...
				if ( ShowSignature && PageContext.BoardSettings.AllowSignatures && DataRow ["Signature"] != DBNull.Value && DataRow ["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && DataRow ["Signature"].ToString().Trim().Length > 0 )
				{
					// signature control is created in base MessagePost
					Signature = DataRow ["Signature"].ToString();
				}
			}

			base.OnPreRender( e );
		}

		protected override void RenderMessage( HtmlTextWriter writer )
		{
			if ( DataRow != null )
			{
				if ( this.MessageFlags.IsDeleted )
				{
					if ( DataRow.Row.Table.Columns.Contains( "IsModeratorChanged" ) )
					{
						this.IsModeratorChanged = Convert.ToBoolean( DataRow ["IsModeratorChanged"] );
					}
					// deleted message text...
					RenderDeletedMessage( writer );
				}
				else if ( this.MessageFlags.NotFormatted )
				{
					// just write out the message with no formatting...
					writer.Write( DataRow ["Message"].ToString() );
				}
				else if ( DataRow.Row.Table.Columns.Contains( "Edited" ) )
				{
					// handle a message that's been edited...
					DateTime editedMessage = Convert.ToDateTime( DataRow ["Posted"] );

					if ( Convert.ToDateTime( DataRow ["Edited"] ) > Convert.ToDateTime( DataRow ["Posted"] ) )
					{
						editedMessage = Convert.ToDateTime( DataRow ["Edited"] );
					}

					if ( this.MessageFlags.IsBBCode )
					{
						RenderModulesInBBCode( writer, FormatMsg.FormatMessage( DataRow ["Message"].ToString(), this.MessageFlags, false, editedMessage ) );
					}
					else
					{
						writer.Write( FormatMsg.FormatMessage( DataRow ["Message"].ToString(), this.MessageFlags, false, editedMessage ) );
					}
				}
				else
				{
					// render standard using bbcode or html...
					if ( this.MessageFlags.IsBBCode )
					{
						RenderModulesInBBCode( writer, FormatMsg.FormatMessage( DataRow ["Message"].ToString(), this.MessageFlags ) );
					}
					else
					{
						writer.Write( FormatMsg.FormatMessage( DataRow ["Message"].ToString(), this.MessageFlags ) );
					}
				}
			}
		}

		private DataRowView _row = null;
		public DataRowView DataRow
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;
				if ( _row != null )
				{
					this.MessageFlags = new MessageFlags( _row ["Flags"] );
				}
			}
		}

		private bool _showAttachments = true;
		public bool ShowAttachments
		{
			get { return _showAttachments; }
			set { _showAttachments = value; }
		}

		private bool _showSignature = true;
		public bool ShowSignature
		{
			get { return _showSignature; }
			set { _showSignature = value; }
		}
	}
}