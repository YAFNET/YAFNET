/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Web.Controls;

/// <summary>
/// The UserLabel
/// </summary>
public class UserLabel : BaseControl
{
    /// <summary>
    ///   Gets or sets CSS Class.
    /// </summary>
    
    public string CssClass
    {
        get => this.ViewState["CssClass"] != null ? this.ViewState["CssClass"].ToString() : string.Empty;

        set => this.ViewState["CssClass"] = value;
    }

    /// <summary>
    ///   Gets or sets The name of the user for this profile link
    /// </summary>
    public string PostfixText
    {
        get => this.ViewState["PostfixText"] != null ? this.ViewState["PostfixText"].ToString() : string.Empty;

        set => this.ViewState["PostfixText"] = value;
    }

    /// <summary>
    ///   Gets or sets The replace Crawler name of this user for the link. Attention! Use it ONLY for crawlers. 
    /// </summary>
    
    public string CrawlerName
    {
        get => this.ViewState["CrawlerName"] != null ? this.ViewState["CrawlerName"].ToString() : string.Empty;

        set => this.ViewState["CrawlerName"] = value;
    }

    /// <summary>
    ///   Gets or sets Style.
    /// </summary>
    
    public string Style
    {
        get => this.ViewState["Style"] != null ? this.ViewState["Style"].ToString() : string.Empty;

        set => this.ViewState["Style"] = value;
    }

    /// <summary>
    ///   Gets or sets Style.
    /// </summary>
    
    public string ReplaceName
    {
        get => this.ViewState["ReplaceName"] != null ? this.ViewState["ReplaceName"].ToString() : string.Empty;

        set => this.ViewState["ReplaceName"] = value;
    }

    /// <summary>
    ///   Gets or sets The User Id of this user for the link
    /// </summary>
    public int UserID
    {
        get
        {
            if (this.ViewState["UserID"] != null)
            {
                return this.ViewState["UserID"].ToType<int>();
            }

            return -1;
        }

        set => this.ViewState["UserID"] = value;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The output.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        var displayName = this.ReplaceName;

        if (this.UserID == -1)
        {
            return;
        }

        writer.BeginRender();

        writer.WriteBeginTag("span");

        this.RenderMainTagAttributes(writer);

        writer.Write(HtmlTextWriter.TagRightChar);

        displayName = this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName;

        writer.WriteEncodedText(this.CrawlerName.IsNotSet() ? displayName : this.CrawlerName);

        writer.WriteEndTag("span");

        if (this.PostfixText.IsSet())
        {
            writer.Write(this.PostfixText);
        }

        writer.EndRender();
    }

    /// <summary>
    /// Renders "id", "style" and "class"
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected void RenderMainTagAttributes(HtmlTextWriter output)
    {
        if (this.ClientID.IsSet())
        {
            output.WriteAttribute(HtmlTextWriterAttribute.Id.ToString(), this.ClientID);
        }

        if (this.Style.IsSet() && this.PageBoardContext.BoardSettings.UseStyledNicks)
        {
            var style = this.Get<IStyleTransform>().Decode(this.Style);

            output.WriteAttribute(HtmlTextWriterAttribute.Style.ToString(), this.HtmlEncode(style));
        }

        if (this.CssClass.IsSet())
        {
            output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), this.CssClass);
        }
    }
}