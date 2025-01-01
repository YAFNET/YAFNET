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
/// The Breadcrumb tag helper.
/// </summary>
[HtmlTargetElement("breadcrumb")]
public class BreadCrumbTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (BoardContext.Current.PageLinks.NullOrEmpty())
        {
            return Task.CompletedTask;
        }

        output.TagName = HtmlTag.Nav;

        output.Attributes.Add(HtmlAttribute.AriaLabel, "breadcrumb");

        var ol = new TagBuilder(HtmlTag.Ol);

        ol.AddCssClass("breadcrumb");

        BoardContext.Current.PageLinks.ForEach(
            link =>
                {
                    var encodedTitle = BoardContext.Current.CurrentForumPage.HtmlEncode(link.Title);
                    var url = link.URL;

                    var listElement = new TagBuilder(HtmlTag.Li);

                    listElement.AddCssClass(url.IsNotSet() ? "breadcrumb-item active" : "breadcrumb-item");

                    if (url.IsNotSet())
                    {
                        listElement.InnerHtml.AppendHtml(encodedTitle);
                    }
                    else
                    {
                        var linkAnchor = new TagBuilder(HtmlTag.A);

                        linkAnchor.MergeAttribute(HtmlAttribute.Href, url);

                        linkAnchor.InnerHtml.AppendHtml(encodedTitle);

                        listElement.InnerHtml.AppendHtml(linkAnchor);
                    }

                    ol.InnerHtml.AppendHtml(listElement);
                });

        output.Content.AppendHtml(ol);
        return Task.CompletedTask;
    }
}