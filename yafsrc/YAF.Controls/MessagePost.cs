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
using System.Web.UI;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Controls
{
  /// <summary>
  /// Shows a Message Post
  /// </summary>
  public class MessagePost : MessageBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MessagePost"/> class.
    /// </summary>
    public MessagePost()
      : base()
    {
    }

    /// <summary>
    /// Gets or sets DisplayUserID.
    /// </summary>
    public virtual int? DisplayUserID
    {
      get
      {
        if (ViewState["DisplayUserID"] != null)
        {
          return Convert.ToInt32(ViewState["DisplayUserID"]);
        }

        return null;
      }

      set
      {
        ViewState["DisplayUserID"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Signature.
    /// </summary>
    public virtual string Signature
    {
      get
      {
        if (ViewState["Signature"] != null)
        {
          return ViewState["Signature"].ToString();
        }

        return null;
      }

      set
      {
        ViewState["Signature"] = value;
      }
    }

    /// <summary>
    /// Gets or sets Message.
    /// </summary>
    public virtual string Message
    {
      get
      {
        if (ViewState["Message"] != null)
        {
          return ViewState["Message"].ToString();
        }

        return null;
      }

      set
      {
        ViewState["Message"] = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether IsModeratorChanged.
    /// </summary>
    public virtual bool IsModeratorChanged
    {
      get
      {
        if (ViewState["IsModeratorChanged"] != null)
        {
          return Convert.ToBoolean(ViewState["IsModeratorChanged"]);
        }

        return false;
      }

      set
      {
        ViewState["IsModeratorChanged"] = value;
      }
    }

    /// <summary>
    /// Gets or sets MessageFlags.
    /// </summary>
    public virtual MessageFlags MessageFlags
    {
      get
      {
        if (ViewState["MessageFlags"] == null)
        {
          ViewState["MessageFlags"] = new MessageFlags(0);
        }

        return ViewState["MessageFlags"] as MessageFlags;
      }

      set
      {
        ViewState["MessageFlags"] = value;
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
      if (!String.IsNullOrEmpty(Signature))
      {
        var sig = new MessageSignature();
        sig.Signature = Signature;
        sig.DisplayUserID = DisplayUserID;
        Controls.Add(sig);
      }

      base.OnPreRender(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      writer.BeginRender();
      writer.WriteBeginTag("div");
      writer.WriteAttribute("id", ClientID);
      writer.Write(HtmlTextWriter.TagRightChar);

      RenderMessage(writer);

      // render controls...
      base.Render(writer);

      writer.WriteEndTag("div");
      writer.EndRender();
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected virtual void RenderMessage(HtmlTextWriter writer)
    {
      if (MessageFlags.IsDeleted)
      {
        // deleted message text...
        RenderDeletedMessage(writer);
      }
      else if (MessageFlags.NotFormatted)
      {
        writer.Write(Message);
      }
      else
      {
        string formattedMessage = FormatMsg.FormatMessage(Message, MessageFlags);

        if (MessageFlags.IsBBCode)
        {
          RenderModulesInBBCode(writer, formattedMessage, MessageFlags, DisplayUserID);
        }
        else
        {
          writer.Write(formattedMessage);
        }
      }
    }

    /// <summary>
    /// The render deleted message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected virtual void RenderDeletedMessage(HtmlTextWriter writer)
    {
      // if message was deleted then write that instead of real body
      if (MessageFlags.IsDeleted)
      {
        if (IsModeratorChanged)
        {
          // deleted by mod
          writer.Write(PageContext.Localization.GetText("POSTS", "MESSAGEDELETED_MOD"));
        }
        else
        {
          // deleted by user
          writer.Write(PageContext.Localization.GetText("POSTS", "MESSAGEDELETED_USER"));
        }
      }
    }
  }
}