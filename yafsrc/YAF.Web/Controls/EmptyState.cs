/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
/// The Empty State Control
/// </summary>
[DefaultProperty("Message")]
[ToolboxData("<{0}:EmptyState runat=server></{0}:EmptyState>")]
public class EmptyState : BaseControl
{
    /// <summary>
    /// Gets or sets the Header Icon.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("")]
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the Header Text Page
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("")]
    public string HeaderTextPage { get; set; }

    /// <summary>
    /// Gets or sets the Header Text Tag
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("")]
    public string HeaderTextTag { get; set; }

    /// <summary>
    /// Gets or sets the Message Text Page
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("")]
    public string MessageTextPage { get; set; }

    /// <summary>
    /// Gets or sets the Message Text Tag
    /// </summary>
    [Category("Appearance")]
    [DefaultValue("")]
    public string MessageTextTag { get; set; }

    /// <summary>
    /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
    public override void RenderControl(HtmlTextWriter writer)
    {
        if (!this.Visible)
        {
            return;
        }

        // Render Main Div
        writer.WriteBeginTag(nameof(HtmlTextWriterTag.Div));

        writer.WriteAttribute(
            nameof(HtmlTextWriterAttribute.Class),
            "px-3 py-4 my-4 text-center");

        writer.Write(HtmlTextWriter.TagRightChar);

        // Render Icon
        writer.WriteBeginTag(nameof(HtmlTextWriterTag.I));

        writer.WriteAttribute(
            nameof(HtmlTextWriterAttribute.Class),
            $"fa fa-{this.Icon} fa-5x");

        writer.Write(HtmlTextWriter.TagRightChar);

        writer.WriteEndTag(nameof(HtmlTextWriterTag.I));

        // Render Header
        writer.WriteBeginTag(nameof(HtmlTextWriterTag.H1));

        writer.WriteAttribute(
            nameof(HtmlTextWriterAttribute.Class),
            "display-5 fw-bold");

        writer.Write(HtmlTextWriter.TagRightChar);

        writer.Write(this.GetText(this.HeaderTextPage, this.HeaderTextTag));

        writer.WriteEndTag(nameof(HtmlTextWriterTag.H1));

        // Render Message
        writer.WriteBeginTag(nameof(HtmlTextWriterTag.Div));

        writer.WriteAttribute(
            nameof(HtmlTextWriterAttribute.Class),
            "col-lg-12 mx-auto");

        writer.Write(HtmlTextWriter.TagRightChar);

        writer.WriteBeginTag(nameof(HtmlTextWriterTag.P));

        writer.WriteAttribute(
            nameof(HtmlTextWriterAttribute.Class),
            "lead mb-3");

        writer.Write(HtmlTextWriter.TagRightChar);

        writer.Write(this.GetText(this.MessageTextPage, this.MessageTextTag));

        writer.WriteEndTag(nameof(HtmlTextWriterTag.P));

        writer.WriteEndTag(nameof(HtmlTextWriterTag.Div));

        writer.WriteEndTag(nameof(HtmlTextWriterTag.Div));
    }
}