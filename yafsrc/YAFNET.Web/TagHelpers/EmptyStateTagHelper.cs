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

namespace YAF.Web.TagHelpers;

/// <summary>
/// The Empty State tag helper.
/// </summary>
[HtmlTargetElement("empty")]
public class EmptyTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyTagHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public EmptyTagHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    [HtmlAttributeName("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the Header Text Page
    /// </summary>
    public string HeaderTextPage { get; set; }

    /// <summary>
    /// Gets or sets the Header Text Tag
    /// </summary>
    public string HeaderTextTag { get; set; }

    /// <summary>
    /// Gets or sets the Message Text Page
    /// </summary>
    public string MessageTextPage { get; set; }

    /// <summary>
    /// Gets or sets the Message Text Tag
    /// </summary>
    public string MessageTextTag { get; set; }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = HtmlTag.Div;

        output.Attributes.Add(HtmlAttribute.Class, "px-3 py-4 my-4 text-center");

        // Render Icon
        var iconTag = new TagBuilder(HtmlTag.I);

        iconTag.AddCssClass($"fa fa-{this.Icon} fa-5x text-secondary");

        output.Content.AppendHtml(iconTag);

        // Render Header
        var headerTag = new TagBuilder(HtmlTag.H1);

        headerTag.AddCssClass("display-5 fw-bold");

        headerTag.InnerHtml.Append(this.GetText(this.HeaderTextPage, this.HeaderTextTag));

        output.Content.AppendHtml(headerTag);

        // Render Message
        var messageDivTag = new TagBuilder(HtmlTag.Div);

        messageDivTag.AddCssClass("col-lg-12 mx-auto");

        var messageContentTag = new TagBuilder(HtmlTag.P);

        messageContentTag.AddCssClass("lead mb-3");

        if (this.MessageTextTag.IsNotSet())
        {
            return Task.CompletedTask;
        }

        messageContentTag.InnerHtml.AppendHtml(this.GetText(this.MessageTextPage, this.MessageTextTag));

        messageDivTag.InnerHtml.AppendHtml(messageContentTag);

        output.Content.AppendHtml(messageDivTag);

        return Task.CompletedTask;
    }
}