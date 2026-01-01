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

namespace YAF.Web.TagHelpers;

/// <summary>
/// The Bootstrap alert tag helper.
/// </summary>
[HtmlTargetElement("alert")]
public class AlertTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlertTagHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public AlertTagHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => field ??= this.Get<ILocalization>();

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    [HtmlAttributeName("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon text color.
    /// </summary>
    [HtmlAttributeName("icon-text-color")]
    public string IconTextColor { get; set; }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    [HtmlAttributeName("class")]
    public string CssClass { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    [HtmlAttributeName("type")]
    public MessageTypes Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether dismissible.
    /// </summary>
    [HtmlAttributeName("dismissible")]
    public bool Dismissible { get; set; }

    /// <summary>
    /// Gets or sets the localized page.
    /// </summary>
    public string LocalizedPage { get; set; }

    /// <summary>
    /// Gets or sets the localized tag.
    /// </summary>
    public string LocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets the localized param0.
    /// </summary>
    /// <value>The localized param0.</value>
    public string LocalizedParam0 { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string Message { get; set; }

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
    public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = HtmlTag.Div;

        var cssClassAttribute = this.CssClass.IsSet() ? $" {this.CssClass}" :
                                    string.Empty;
        output.Attributes.Add(
            HtmlAttribute.Class,
            this.Dismissible
                ? $"text-break alert alert-{this.Type} alert-dismissible fade show{cssClassAttribute}"
                : $"text-break alert alert-{this.Type}{cssClassAttribute}");

        output.Attributes.Add(HtmlAttribute.Role, "alert");

        if (this.Icon.IsSet())
        {
            var iconTag = new TagBuilder(HtmlTag.I);

            iconTag.AddCssClass($"fa fa-{this.Icon} {this.IconTextColor} me-1");

            output.Content.AppendHtml(iconTag);
        }

        var childContent = await output.GetChildContentAsync();

        if (childContent is {IsEmptyOrWhiteSpace: false})
        {
            output.Content = childContent;
        }
        else
        {
            if (this.Message.IsSet())
            {
                output.Content.AppendHtml(this.Message);
            }
            else
            {
                if (this.LocalizedTag.IsSet())
                {
                    var text = this.LocalizedPage.IsSet()
                        ? this.GetText(this.LocalizedPage, this.LocalizedTag).FormatWith(this.LocalizedParam0)
                        : this.GetText(this.LocalizedTag).FormatWith(this.LocalizedParam0);

                    output.Content.AppendHtml(text);
                }
            }
        }

        if (!this.Dismissible)
        {
            return;
        }

        var closeButton = new TagBuilder(HtmlTag.Button);

        closeButton.MergeAttribute(HtmlAttribute.Type, HtmlTag.Button);
        closeButton.AddCssClass("btn-close");
        closeButton.MergeAttribute("data-bs-dismiss", "alert");
        closeButton.MergeAttribute(HtmlAttribute.AriaLabel, "close");

        output.Content.AppendHtml(closeButton);
    }
}