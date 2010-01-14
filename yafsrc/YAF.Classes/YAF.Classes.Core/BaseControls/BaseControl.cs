/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Controls
{
  using System;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using YAF.Classes.Core;

  /// <summary>
  /// Control derived from Panel that includes a reference to the <see cref="YafContext"/>.
  /// </summary>
  public class BasePanel : Panel
  {
    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        if (Site != null && Site.DesignMode)
        {
          // design-time, return null...
          return null;
        }

        return YafContext.Current;
      }
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public string GetUniqueID(string prefix)
    {
      if (!String.IsNullOrEmpty(prefix))
      {
        return prefix + Guid.NewGuid().ToString().Substring(0, 5);
      }
      else
      {
        return Guid.NewGuid().ToString().Substring(0, 10);
      }
    }

    /// <summary>
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public string GetExtendedID(string prefix)
    {
      string createdID = null;

      if (!String.IsNullOrEmpty(ID))
      {
        createdID = ID + "_";
      }

      if (!String.IsNullOrEmpty(prefix))
      {
        createdID += prefix;
      }
      else
      {
        createdID += Guid.NewGuid().ToString().Substring(0, 5);
      }

      return createdID;
    }
  }

  /// <summary>
  /// Summary description for BaseControl.
  /// </summary>
  public class BaseControl : Control
  {
    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        if (Site != null && Site.DesignMode)
        {
          // design-time, return null...
          return null;
        }

        return YafContext.Current;
      }
    }

    /// <summary>
    /// The html encode.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The html encode.
    /// </returns>
    public string HtmlEncode(object data)
    {
      return HttpContext.Current.Server.HtmlEncode(data.ToString());
    }

    /// <summary>
    /// The render anchor.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="innerText">
    /// The inner text.
    /// </param>
    public void RenderAnchor(HtmlTextWriter writer, string href, string cssClass, string innerText)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (!String.IsNullOrEmpty(cssClass))
      {
        writer.WriteAttribute("class", cssClass);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(innerText);
      writer.WriteEndTag("a");
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public string GetUniqueID(string prefix)
    {
      if (!String.IsNullOrEmpty(prefix))
      {
        return prefix + Guid.NewGuid().ToString().Substring(0, 5);
      }
      else
      {
        return Guid.NewGuid().ToString().Substring(0, 10);
      }
    }

    /// <summary>
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public string GetExtendedID(string prefix)
    {
      string createdID = null;

      if (!String.IsNullOrEmpty(ID))
      {
        createdID = ID + "_";
      }

      if (!String.IsNullOrEmpty(prefix))
      {
        createdID += prefix;
      }
      else
      {
        createdID += Guid.NewGuid().ToString().Substring(0, 5);
      }

      return createdID;
    }

    /// <summary>
    /// The render img tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="src">
    /// The src.
    /// </param>
    /// <param name="alt">
    /// The alt.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    public void RenderImgTag(HtmlTextWriter writer, string src, string alt, string title)
    {
      // this will output the start of the img element - <img
      writer.WriteBeginTag("img");

      writer.WriteAttribute("src", src);
      writer.WriteAttribute("alt", alt);

      if (!String.IsNullOrEmpty(title))
      {
        writer.WriteAttribute("title", title);
      }

      writer.Write(HtmlTextWriter.SelfClosingTagEnd);
    }

    #region Render Anchor Begin Functions

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    public void RenderAnchorBegin(HtmlTextWriter writer, string href)
    {
      RenderAnchorBegin(writer, href, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    public void RenderAnchorBegin(HtmlTextWriter writer, string href, string cssClass)
    {
      RenderAnchorBegin(writer, href, cssClass, null, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    public void RenderAnchorBegin(HtmlTextWriter writer, string href, string cssClass, string title)
    {
      RenderAnchorBegin(writer, href, cssClass, title, null, null);
    }

    /// <summary>
    /// The render anchor begin.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="href">
    /// The href.
    /// </param>
    /// <param name="cssClass">
    /// The css class.
    /// </param>
    /// <param name="title">
    /// The title.
    /// </param>
    /// <param name="onclick">
    /// The onclick.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    public void RenderAnchorBegin(HtmlTextWriter writer, string href, string cssClass, string title, string onclick, string id)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (!String.IsNullOrEmpty(cssClass))
      {
        writer.WriteAttribute("class", cssClass);
      }

      if (!String.IsNullOrEmpty(title))
      {
        writer.WriteAttribute("title", HtmlEncode(title));
      }

      if (!String.IsNullOrEmpty(onclick))
      {
        writer.WriteAttribute("onclick", onclick);
      }

      if (!String.IsNullOrEmpty(id))
      {
        writer.WriteAttribute("id", id);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
    }

    #endregion
  }
}