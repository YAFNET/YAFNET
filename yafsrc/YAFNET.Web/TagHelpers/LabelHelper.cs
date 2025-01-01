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

using Microsoft.AspNetCore.Mvc.ViewFeatures;

/// <summary>
/// The label helper.
/// </summary>
[HtmlTargetElement("label", Attributes = "localized-tag")]
public class LabelHelper : TagHelper, IHaveServiceLocator, ILocalizationSupport
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LabelHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="htmlHelper"></param>
    public LabelHelper(IServiceLocator serviceLocator, IHtmlHelper htmlHelper)
    {
        this.ServiceLocator = serviceLocator;
        this.html = htmlHelper;
    }

    /// <summary>
    /// Gets or sets a value indicating whether enable bb code.
    /// </summary>
    public bool EnableBBCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether show [information button].
    /// </summary>
    /// <value><c>true</c> if [information button]; otherwise, <c>false</c>.</value>
    public bool InfoButton { get; set; }

    /// <summary>
    /// Gets or sets the localized page.
    /// </summary>
    public string LocalizedPage { get; set; }

    /// <summary>
    /// Gets or sets the localized tag.
    /// </summary>
    public string LocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets the parameter 0.
    /// </summary>
    public string Param0 { get; set; }

    /// <summary>
    /// Gets or sets the parameter 1.
    /// </summary>
    public string Param1 { get; set; }

    /// <summary>
    /// Gets or sets the parameter 2.
    /// </summary>
    public string Param2 { get; set; }

    private readonly IHtmlHelper html;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
    /// <value>The view context.</value>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        (this.html as IViewContextAware)?.Contextualize(this.ViewContext);

        // Render Text
        if (this.LocalizedTag.IsNotSet())
        {
            return;
        }

        output.Content.Clear();

        var text = this.LocalizedPage.IsSet()
                       ? this.Get<ILocalization>().GetText(this.LocalizedPage, this.LocalizedTag)
                       : this.Get<ILocalization>().GetText(this.LocalizedTag);

        output.Content.Append(text.FormatWith(this.Param0, this.Param1, this.Param2));

        if (!this.InfoButton)
        {
            return;
        }

        // Render help Label (info tooltip)
        var tooltip = this.Get<ILocalization>().GetText(this.LocalizedPage, $"{this.LocalizedTag}_HELP");

        if (!tooltip.IsSet())
        {
            return;
        }

        var button = new TagBuilder(HtmlTag.Button);

        button.AddCssClass("btn btn-sm");
        button.MergeAttribute(HtmlAttribute.Type, HtmlTag.Button);
        button.MergeAttribute(HtmlAttribute.Title, tooltip);
        button.MergeAttribute("data-bs-html", "true");
        button.MergeAttribute("data-bs-toggle", "tooltip");
        button.MergeAttribute("data-bs-placement", "right");

        button.InnerHtml.AppendHtml(this.html.Icon("info-circle", "text-info", iconSize: "fa-lg"));

        output.Content.AppendHtml(button);
    }
}