/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.UI;

  #endregion

  /// <summary>
  /// Shows a Message Post
  /// </summary>
  public class MessagePostData : MessagePost
  {
    #region Constants and Fields

    /// <summary>
    /// The _row.
    /// </summary>
    private DataRow _row = null;

    /// <summary>
    /// The _show attachments.
    /// </summary>
    private bool _showAttachments = true;

    /// <summary>
    /// The _show signature.
    /// </summary>
    private bool _showSignature = true;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePostData"/> class.
    /// </summary>
    public MessagePostData()
      : base()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets DataRow.
    /// </summary>
    public DataRow DataRow
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
          this.MessageFlags = new MessageFlags(this._row["Flags"]);
        }
      }
    }

    /// <summary>
    /// Gets Edited.
    /// </summary>
    public DateTime Edited
    {
      get
      {
        if (this.DataRow != null)
        {
          return Convert.ToDateTime(this.DataRow["Edited"]);
        }

        return DateTime.UtcNow;
      }
    }

    /// <summary>
    /// Gets Message.
    /// </summary>
    public override string Message
    {
      get
      {
        if (this.DataRow != null)
        {
          string message = this.DataRow["Message"].ToString();

          return TruncateMessage(message);
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Posted.
    /// </summary>
    public DateTime Posted
    {
      get
      {
        if (this.DataRow != null)
        {
          return Convert.ToDateTime(this.DataRow["Posted"]);
        }

        return DateTime.UtcNow;
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
    /// Gets Signature.
    /// </summary>
    public override string Signature
    {
      get
      {
        if (this.DataRow != null && this.ShowSignature && this.PageContext.BoardSettings.AllowSignatures &&
            this.DataRow["Signature"] != DBNull.Value &&
            this.DataRow["Signature"].ToString().ToLower() != "<p>&nbsp;</p>" &&
            this.DataRow["Signature"].ToString().Trim().Length > 0)
        {
          return this.DataRow["Signature"].ToString();
        }

        return null;
      }
    }

    #endregion

    #region Public Methods

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

    #endregion

    #region Methods

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      if (this.DataRow != null && !this.MessageFlags.IsDeleted)
      {
        // populate DisplayUserID
        if (!UserMembershipHelper.IsGuestUser(this.DataRow["UserID"]))
        {
          this.DisplayUserID = Convert.ToInt32(this.DataRow["UserID"]);
        }

        if (this.ShowAttachments && long.Parse(this.DataRow["HasAttachments"].ToString()) > 0)
        {
          // add attached files control...
          var attached = new MessageAttached();
          attached.MessageID = Convert.ToInt32(this.DataRow["MessageID"]);

          if  (this.DataRow["UserID"] != DBNull.Value && PageContext.BoardSettings.EnableDisplayName)
          {
             attached.UserName = UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(this.DataRow["UserID"]));
          }
          else
          {
              attached.UserName = this.DataRow["UserName"].ToString();
          }

            this.Controls.Add(attached);
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
      if (this.DataRow != null)
      {
        if (this.MessageFlags.IsDeleted)
        {
          if (this.DataRow.Table.Columns.Contains("IsModeratorChanged"))
          {
            this.IsModeratorChanged = Convert.ToBoolean(this.DataRow["IsModeratorChanged"]);
          }

          // deleted message text...
          this.RenderDeletedMessage(writer);
        }
        else if (this.MessageFlags.NotFormatted)
        {
          // just write out the message with no formatting...
          writer.Write(this.Message);
        }
        else if (this.DataRow.Table.Columns.Contains("Edited"))
        {
          // handle a message that's been edited...
          DateTime editedMessage = this.Posted;

          if (this.Edited > this.Posted)
          {
            editedMessage = this.Edited;
          }

          if (this.MessageFlags.IsBBCode)
          {
            this.RenderModulesInBBCode(
              writer, 
              this.HighlightMessage(YafFormatMessage.FormatMessage(this.Message, this.MessageFlags, false, editedMessage)), 
              this.MessageFlags, 
              this.DisplayUserID);
          }
          else
          {
            writer.Write(HighlightMessage(YafFormatMessage.FormatMessage(this.Message, this.MessageFlags, false, editedMessage)));
          }
        }
        else
        {
          // render standard using bbcode or html...
          if (this.MessageFlags.IsBBCode)
          {
            this.RenderModulesInBBCode(
              writer, this.HighlightMessage(YafFormatMessage.FormatMessage(this.Message, this.MessageFlags)), this.MessageFlags, this.DisplayUserID);
          }
          else
          {
            writer.Write(this.HighlightMessage(YafFormatMessage.FormatMessage(this.Message, this.MessageFlags)));
          }
        }
      }
    }

    #endregion
  }
}