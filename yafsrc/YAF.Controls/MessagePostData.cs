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
using System.Data;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
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

		protected override void OnPreRender(EventArgs e)
		{
			if (DataRow != null && !this.MessageFlags.IsDeleted)
			{
				// populate DisplayUserID
				if (!UserMembershipHelper.IsGuestUser(DataRow["UserID"])) DisplayUserID = Convert.ToInt32(DataRow["UserID"]);

				if (ShowAttachments && long.Parse(DataRow["HasAttachments"].ToString()) > 0)
				{
					// add attached files control...
					MessageAttached attached = new MessageAttached();
					attached.MessageID = Convert.ToInt32(DataRow["MessageID"]);
					attached.UserName = DataRow["UserName"].ToString();
					this.Controls.Add(attached);
				}
			}

			base.OnPreRender(e);
		}

		protected override void RenderMessage(HtmlTextWriter writer)
		{
			if (DataRow != null)
			{
				if (this.MessageFlags.IsDeleted)
				{
					if (DataRow.Row.Table.Columns.Contains("IsModeratorChanged"))
					{
						this.IsModeratorChanged = Convert.ToBoolean(DataRow["IsModeratorChanged"]);
					}
					// deleted message text...
					RenderDeletedMessage(writer);
				}
				else if (this.MessageFlags.NotFormatted)
				{
					// just write out the message with no formatting...
					writer.Write(Message);
				}
				else if (DataRow.Row.Table.Columns.Contains("Edited"))
				{
					// handle a message that's been edited...
					DateTime editedMessage = Posted;

					if (Edited > Posted)
					{
						editedMessage = Edited;
					}

					if (this.MessageFlags.IsBBCode)
					{
						RenderModulesInBBCode(writer, FormatMsg.FormatMessage(Message, this.MessageFlags, false, editedMessage), this.MessageFlags, this.DisplayUserID);
					}
					else
					{
						writer.Write(FormatMsg.FormatMessage(Message, this.MessageFlags, false, editedMessage));
					}
				}
				else
				{
					// render standard using bbcode or html...
					if (this.MessageFlags.IsBBCode)
					{
						RenderModulesInBBCode(writer, FormatMsg.FormatMessage(Message, this.MessageFlags), this.MessageFlags, this.DisplayUserID);
					}
					else
					{
						writer.Write(FormatMsg.FormatMessage(Message, this.MessageFlags));
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
				if (_row != null)
				{
					this.MessageFlags = new MessageFlags(_row["Flags"]);
				}
			}
		}

		public DateTime Posted
		{
			get
			{
				if ( DataRow != null ) return Convert.ToDateTime(DataRow["Posted"]);
				return DateTime.Now;
			}
		}

		public DateTime Edited
		{
			get
			{
				if ( DataRow != null ) return Convert.ToDateTime(DataRow["Edited"]);
				return DateTime.Now;
			}
		}

		public override string Signature
		{
			get
			{
				if ( DataRow != null && ShowSignature && PageContext.BoardSettings.AllowSignatures && DataRow["Signature"] != DBNull.Value && DataRow["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && DataRow["Signature"].ToString().Trim().Length > 0 )
				{
					return DataRow["Signature"].ToString();
				}

				return null;
			}
		}

		public override string Message
		{
			get
			{
				if ( DataRow != null )
				{
					string message = DataRow["Message"].ToString();

					return TruncateMessage(message);
				}

				return string.Empty;
			}
		}
		public static string TruncateMessage(string message)
		{
			// validate the size...
			if (YafContext.Current.BoardSettings.MaxPostSize < 0)
				return message;

			if (message.Length < YafContext.Current.BoardSettings.MaxPostSize)
				return message;

			// truncate... 
			message = message.Substring(0, YafContext.Current.BoardSettings.MaxPostSize);
			int lastSpaceIndex = message.LastIndexOf(" ");
			if (lastSpaceIndex > 0)
				return message.Substring(0, lastSpaceIndex) + "...";

			return message.Substring(0, message.Length - 3) + "...";
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