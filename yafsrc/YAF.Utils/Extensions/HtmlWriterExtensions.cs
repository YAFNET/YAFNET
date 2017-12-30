/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Utils
{
  using System.Web;
  using System.Web.UI;

  using YAF.Types.Extensions;
  using YAF.Utils.Helpers.StringUtils;

  public static class HtmlWriterExtensions
  {
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
    public static void RenderAnchor(this HtmlTextWriter writer, string href, string cssClass, string innerText)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
      writer.Write(innerText);
      writer.WriteEndTag("a");
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
    public static void RenderImgTag(this HtmlTextWriter writer, string src, string alt, string title)
    {
      RenderImgTag(writer, src, alt, title, null);
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
    public static void RenderImgTag(this HtmlTextWriter writer, string src, string alt, string title, string cssClass)
    {
      // this will output the start of the img element - <img
      writer.WriteBeginTag("img");

      writer.WriteAttribute("src", src);
      writer.WriteAttribute("alt", alt);

      if (title.IsSet())
      {
        writer.WriteAttribute("title", title);
      }

      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
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
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href)
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
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass)
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
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass, string title)
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
    public static void RenderAnchorBegin(this HtmlTextWriter writer, string href, string cssClass, string title, string onclick, string id)
    {
      writer.WriteBeginTag("a");
      writer.WriteAttribute("href", href);
      if (cssClass.IsSet())
      {
        writer.WriteAttribute("class", cssClass);
      }

      if (title.IsSet())
      {
        writer.WriteAttribute("title", HttpContext.Current.Server.HtmlEncode(title));
      }

      if (onclick.IsSet())
      {
        writer.WriteAttribute("onclick", onclick);
      }

      if (id.IsSet())
      {
        writer.WriteAttribute("id", id);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
    }

    #endregion    

    /// <summary>
    /// The write begin td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    /// <param name="classId">
    /// The class id.
    /// </param>
    public static void WriteBeginTD(this HtmlTextWriter writer, string classId)
    {
      writer.WriteBeginTag("td");
      if (classId.IsSet())
      {
        writer.WriteAttribute("class", classId);
      }

      writer.Write(HtmlTextWriter.TagRightChar);
    }

    /// <summary>
    /// The write begin td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public static void WriteBeginTD(this HtmlTextWriter writer)
    {
      WriteBeginTD(writer, null);
    }

    /// <summary>
    /// The write end td.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public static void WriteEndTD(this HtmlTextWriter writer)
    {
      writer.WriteEndTag("td");
    }
  }
}