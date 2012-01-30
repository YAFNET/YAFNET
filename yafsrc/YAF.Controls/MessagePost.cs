/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using System.Collections.Generic;
    using System.Web.UI;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

  /// <summary>
  /// Shows a Message Post
  /// </summary>
  public class MessagePost : MessageBase
  {
    #region Properties

      /// <summary>
      ///   Gets or sets a value indicating whether IsAlt.
      /// </summary>
      public virtual bool IsAlt
      {
          get
          {
              return this.ViewState["IsAlt"] != null && Convert.ToBoolean(this.ViewState["IsAlt"]);
          }

          set
          {
              this.ViewState["IsAlt"] = value;
          }
      }

      /// <summary>
      ///   Gets or sets a value indicating whether IsAlt.
      /// </summary>
      public virtual string RowColSpan
      {
          get
          {
              return this.ViewState["RowColSpan"] != null ? this.ViewState["RowColSpan"].ToString() : null;
          }

          set
          {
              this.ViewState["RowColSpan"] = value;
          }
      }

    /// <summary>
    ///   Gets or sets DisplayUserID.
    /// </summary>
    public virtual int? DisplayUserID
    {
      get
      {
        if (this.ViewState["DisplayUserID"] != null)
        {
          return this.ViewState["DisplayUserID"].ToType<int>();
        }

        return null;
      }

      set
      {
        this.ViewState["DisplayUserID"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets MessageID.
    /// </summary>
    public virtual int? MessageID
    {
        get
        {
            if (this.ViewState["MessageID"] != null)
            {
                return this.ViewState["MessageID"].ToType<int>();
            }

            return null;
        }

        set
        {
            this.ViewState["MessageID"] = value;
        }
    }

    /// <summary>
    ///   Gets or sets the Words to highlight in this message
    /// </summary>
    [CanBeNull]
    public virtual IList<string> HighlightWords
    {
      get
      {
        if (this.ViewState["HighlightWords"] == null)
        {
          this.ViewState["HighlightWords"] = new List<string>();
        }

        return this.ViewState["HighlightWords"] as IList<string>;
      }

      set
      {
        this.ViewState["HighlightWords"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether IsModeratorChanged.
    /// </summary>
    public virtual bool IsModeratorChanged
    {
      get
      {
          return this.ViewState["IsModeratorChanged"] != null && Convert.ToBoolean(this.ViewState["IsModeratorChanged"]);
      }

        set
      {
        this.ViewState["IsModeratorChanged"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets Message.
    /// </summary>
    public virtual string Message
    {
      get
      {
          return this.ViewState["Message"] != null ? this.ViewState["Message"].ToString() : null;
      }

        set
      {
        this.ViewState["Message"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets MessageFlags.
    /// </summary>
    public virtual MessageFlags MessageFlags
    {
      get
      {
        if (this.ViewState["MessageFlags"] == null)
        {
          this.ViewState["MessageFlags"] = new MessageFlags(0);
        }

        return this.ViewState["MessageFlags"] as MessageFlags;
      }

      set
      {
        this.ViewState["MessageFlags"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets Signature.
    /// </summary>
    public virtual string Signature
    {
      get
      {
          return this.ViewState["Signature"] != null ? this.ViewState["Signature"].ToString() : null;
      }

        set
      {
        this.ViewState["Signature"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Highlight a Message
    /// </summary>
    /// <param name="message">
    /// The Message to Hightlight
    /// </param>
    /// <returns>
    /// The Message with the Span Tag and Css Class "highlight" that Hightlights it
    /// </returns>
    protected virtual string HighlightMessage([NotNull] string message)
    {
      if (this.HighlightWords.Count > 0)
      {
        // highlight word list
        message = this.Get<IFormatMessage>().SurroundWordList(
          message, this.HighlightWords, @"<span class=""highlight"">", @"</span>");
      }

      return message;
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      if (this.Signature.IsSet())
      {
          var sig = new MessageSignature
              {
                  Signature = this.Signature,
                  DisplayUserID = this.DisplayUserID,
                  MessageID = this.MessageID,
                  IsAlt = this.IsAlt,
                  ColSpan = this.RowColSpan
              };

        this.Controls.Add(sig);
      }

      base.OnPreRender(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      writer.BeginRender();
      writer.WriteBeginTag("div");

      writer.WriteAttribute("id", this.ClientID);
      writer.Write(HtmlTextWriter.TagRightChar);

      this.RenderMessage(writer);

      // render controls...
      base.Render(writer);

      writer.WriteEndTag("div");
      writer.EndRender();
    }

    /// <summary>
    /// The render deleted message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected virtual void RenderDeletedMessage([NotNull] HtmlTextWriter writer)
    {
      // if message was deleted then write that instead of real body
      if (this.MessageFlags.IsDeleted)
      {
          writer.Write(
              this.IsModeratorChanged
                  ? this.GetText("POSTS", "MESSAGEDELETED_MOD")
                  : this.GetText("POSTS", "MESSAGEDELETED_USER"));
      }
    }

    /// <summary>
    /// The render message.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected virtual void RenderMessage([NotNull] HtmlTextWriter writer)
    {
      if (this.MessageFlags.IsDeleted)
      {
        // deleted message text...
        this.RenderDeletedMessage(writer);
      }
      else if (this.MessageFlags.NotFormatted)
      {
        writer.Write(this.Message);
      }
      else
      {
        string formattedMessage = this.HighlightMessage(this.Get<IFormatMessage>().FormatMessage(this.Message, this.MessageFlags));

        // tha_watcha : Since html message and bbcode can be mixed now, message should be always replace bbcode
        this.RenderModulesInBBCode(writer, formattedMessage, this.MessageFlags, this.DisplayUserID, this.MessageID);

        /*if (this.MessageFlags.IsBBCode)
        {
          this.RenderModulesInBBCode(writer, formattedMessage, this.MessageFlags, this.DisplayUserID);
        }
        else
        {
          writer.Write(formattedMessage);
        }*/
      }
    }

    #endregion
  }
}