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
  /// The message signature.
  /// </summary>
  public class MessageSignature : MessageBase
  {
    /// <summary>
    /// The _display user id.
    /// </summary>
    private int? _displayUserID;

    /// <summary>
    /// The _html prefix.
    /// </summary>
    private string _htmlPrefix;

    /// <summary>
    /// The _html suffix.
    /// </summary>
    private string _htmlSuffix;

    /// <summary>
    /// The _signature.
    /// </summary>
    private string _signature;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSignature"/> class.
    /// </summary>
    public MessageSignature()
      : base()
    {
    }

    /// <summary>
    /// Gets or sets Signature.
    /// </summary>
    public string Signature
    {
      get
      {
        return this._signature;
      }

      set
      {
        this._signature = value;
      }
    }

    /// <summary>
    /// Gets or sets DisplayUserID.
    /// </summary>
    public int? DisplayUserID
    {
      get
      {
        return this._displayUserID;
      }

      set
      {
        this._displayUserID = value;
      }
    }

    /// <summary>
    /// Gets or sets HtmlPrefix.
    /// </summary>
    public string HtmlPrefix
    {
      get
      {
        return this._htmlPrefix;
      }

      set
      {
        this._htmlPrefix = value;
      }
    }

    /// <summary>
    /// Gets or sets HtmlSuffix.
    /// </summary>
    public string HtmlSuffix
    {
      get
      {
        return this._htmlSuffix;
      }

      set
      {
        this._htmlSuffix = value;
      }
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
      writer.WriteAttribute("class", "yafsignature");
      writer.Write(HtmlTextWriter.TagRightChar);

      if (!String.IsNullOrEmpty(HtmlPrefix))
      {
        writer.Write(HtmlPrefix);
      }

      if (!String.IsNullOrEmpty(Signature))
      {
        RenderSignature(writer);
      }

      if (!String.IsNullOrEmpty(HtmlSuffix))
      {
        writer.Write(HtmlSuffix);
      }

      base.Render(writer);

      writer.WriteEndTag("div");
      writer.EndRender();
    }

    /// <summary>
    /// The render signature.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected void RenderSignature(HtmlTextWriter writer)
    {
      // don't allow any HTML on signatures
      var tFlags = new MessageFlags();
      tFlags.IsHtml = false;

      RenderModulesInBBCode(writer, FormatMsg.FormatMessage(Signature, tFlags), tFlags, DisplayUserID);
    }
  }
}