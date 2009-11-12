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
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Controls
{
  /// <summary>
  /// Shows a Message Post
  /// </summary>
  public class MessagePostData : MessagePost
  {
    /// <summary>
    /// The _row.
    /// </summary>
    private DataRowView _row = null;

    /// <summary>
    /// The _show attachments.
    /// </summary>
    private bool _showAttachments = true;

    /// <summary>
    /// The _show signature.
    /// </summary>
    private bool _showSignature = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePostData"/> class.
    /// </summary>
    public MessagePostData()
      : base()
    {
    }

    /// <summary>
    /// Gets or sets DataRow.
    /// </summary>
    public DataRowView DataRow
    {
      get
      {
        return this._row;
      }

      set
      {
        this._row = value;
        if (this._row != null)
        {
          MessageFlags = new MessageFlags(this._row["Flags"]);
        }
      }
    }

    /// <summary>
    /// Gets Posted.
    /// </summary>
    public DateTime Posted
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToDateTime(DataRow["Posted"]);
        }

        return DateTime.Now;
      }
    }

    /// <summary>
    /// Gets Edited.
    /// </summary>
    public DateTime Edited
    {
      get
      {
        if (DataRow != null)
        {
          return Convert.ToDateTime(DataRow["Edited"]);
        }

        return DateTime.Now;
      }
    }

    /// <summary>
    /// Gets Signature.
    /// </summary>
    public override string Signature
    {
      get
      {
        if (DataRow != null && ShowSignature && PageContext.BoardSettings.AllowSignatures && DataRow["Signature"] != DBNull.Value &&
            DataRow["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" && DataRow["Signature"].ToString().Trim().Length > 0)
        {
          return DataRow["Signature"].ToString();
        }

        return null;
      }
    }

    /// <summary>
    /// Gets Message.
    /// </summary>
    public override string Message
    {
      get
      {
        if (DataRow != null)
        {
          string message = DataRow["Message"].ToString();

          return TruncateMessage(message);
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowAttachments.
    /// </summary>
    public bool ShowAttachments
    {
      get
      {
        return this._showAttachments;
      }

      set
      {
        this._showAttachments = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether ShowSignature.
    /// </summary>
    public bool ShowSignature
    {
      get
      {
        return this._showSignature;
      }

      set
      {
        this._showSignature = value;
      }
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      if (DataRow != null && !MessageFlags.IsDeleted)
      {
        // populate DisplayUserID
        if (!UserMembershipHelper.IsGuestUser(DataRow["UserID"]))
        {
          DisplayUserID = Convert.ToInt32(DataRow["UserID"]);
        }

        if (ShowAttachments && long.Parse(DataRow["HasAttachments"].ToString()) > 0)
        {
          // add attached files control...
          var attached = new MessageAttached();
          attached.MessageID = Convert.ToInt32(DataRow["MessageID"]);
          attached.UserName = DataRow["UserName"].ToString();
          Controls.Add(attached);
        }
      }

      base.OnPreRender(e);
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void RenderMessage(HtmlTextWriter writer)
    {
      if (DataRow != null)
      {
        if (MessageFlags.IsDeleted)
        {
          if (DataRow.Row.Table.Columns.Contains("IsModeratorChanged"))
          {
            IsModeratorChanged = Convert.ToBoolean(DataRow["IsModeratorChanged"]);
          }

          // deleted message text...
          RenderDeletedMessage(writer);
        }
        else if (MessageFlags.NotFormatted)
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

          if (MessageFlags.IsBBCode)
          {
            RenderModulesInBBCode(writer, FormatMsg.FormatMessage(Message, MessageFlags, false, editedMessage), MessageFlags, DisplayUserID);
          }
          else
          {
            writer.Write(FormatMsg.FormatMessage(Message, MessageFlags, false, editedMessage));
          }
        }
        else
        {
          // render standard using bbcode or html...
          if (MessageFlags.IsBBCode)
          {
            RenderModulesInBBCode(writer, FormatMsg.FormatMessage(Message, MessageFlags), MessageFlags, DisplayUserID);
          }
          else
          {
            writer.Write(FormatMsg.FormatMessage(Message, MessageFlags));
          }
        }
      }
    }

    /// <summary>
    /// The truncate message.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <returns>
    /// The truncate message.
    /// </returns>
    public static string TruncateMessage(string message)
    {
      // validate the size...
      if (YafContext.Current.BoardSettings.MaxPostSize < 0)
      {
        return message;
      }

      if (message.Length < YafContext.Current.BoardSettings.MaxPostSize)
      {
        return message;
      }

      // truncate... 
      message = message.Substring(0, YafContext.Current.BoardSettings.MaxPostSize);
      int lastSpaceIndex = message.LastIndexOf(" ");
      if (lastSpaceIndex > 0)
      {
        return message.Substring(0, lastSpaceIndex) + "...";
      }

      return message.Substring(0, message.Length - 3) + "...";
    }
  }
}