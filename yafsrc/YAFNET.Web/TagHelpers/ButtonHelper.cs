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

using System.ComponentModel;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Types.Constants;

/// <summary>
/// Class ButtonHelper.
/// Implements the <see cref="TagHelper" />
/// Implements the <see cref="IHaveServiceLocator" />
/// </summary>
/// <seealso cref="TagHelper" />
/// <seealso cref="IHaveServiceLocator" />
[HtmlTargetElement("button")]
[HtmlTargetElement("a")]
public class ButtonHelper : TagHelper, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonHelper"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public ButtonHelper(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the button style.
    /// </summary>
    /// <value>The button style.</value>
    [DefaultValue(ButtonStyle.None)]
    public ButtonStyle ButtonStyle { get; set; }

    /// <summary>
    /// Gets or sets the size of the button.
    /// </summary>
    /// <value>The size of the button.</value>
    [DefaultValue(ButtonSize.Normal)]
    public ButtonSize ButtonSize { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon bade.
    /// </summary>
    /// <value>The icon bade.</value>
    public string IconBade { get; set; }

    /// <summary>
    /// Gets or sets the icon CSS Class.
    /// </summary>
    /// <value>
    /// The icon CSS class.
    /// </value>
    public string IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    public string IconColor { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [icon mobile only].
    /// </summary>
    /// <value><c>true</c> if [icon mobile only]; otherwise, <c>false</c>.</value>
    public bool IconMobileOnly { get; set; }

    /// <summary>
    /// Gets or sets the return confirm tag.
    /// </summary>
    /// <value>
    /// The return confirm tag.
    /// </value>
    public string ReturnConfirmTag { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the data target.
    /// </summary>
    /// <value>
    /// The data target.
    /// </value>
    public string BsTargetUrl { get; set; }

    /// <summary>
    /// Gets or sets the bs target.
    /// </summary>
    /// <value>The bs target.</value>
    public string BsTarget { get; set; }

    /// <summary>
    /// Gets or sets the data toggle.
    /// </summary>
    /// <value>
    /// The data toggle.
    /// </value>
    public string BsToggle { get; set; }

    /// <summary>
    /// Gets or sets the data dismiss.
    /// </summary>
    public string BsDismiss { get; set; }

    /// <summary>
    /// Gets or sets the content of the bs.
    /// </summary>
    /// <value>The content of the bs.</value>
    public string BsContent { get; set; }

    /// <summary>
    /// Gets or sets the title non localized.
    /// </summary>
    /// <value>The title non localized.</value>
    public string TitleNonLocalized { get; set; }

    /// <summary>
    ///    Gets or sets the Localized Page for the optional button text
    /// </summary>
    public string TextLocalizedPage { get; set; }

    /// <summary>
    ///    Gets or sets the Localized Tag for the optional button text
    /// </summary>
    public string TextLocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets the text parameter 0.
    /// </summary>
    public string TextParam0 { get; set; }

    /// <summary>
    ///    Gets or sets the Localized Page for the optional button title
    /// </summary>
    public string TitleLocalizedPage { get; set; }

    /// <summary>
    ///    Gets or sets the Localized Tag for the optional button title
    /// </summary>
    public string TitleLocalizedTag { get; set; }

    /// <summary>
    /// Gets or sets the title parameter 0.
    /// </summary>
    public string TitleParam0 { get; set; }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    /// 
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The HTML
    /// </summary>
    private readonly IHtmlHelper html;

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

        var actionClass = GetAttributeValue(this.ButtonStyle);

        var cssClass = new StringBuilder();

        cssClass.Append(actionClass);

        if (output.Attributes.ContainsName("class"))
        {
            cssClass.Append($" {output.Attributes[HtmlAttribute.Class].Value}");
        }

        if (output.TagName == "a" && !output.Attributes.ContainsName("role"))
        {
            output.Attributes.SetAttribute(HtmlAttribute.Role, HtmlTag.Button);
        }

        if (this.ButtonSize != ButtonSize.Normal)
        {
            cssClass.Append($" {GetButtonSizeClass(this.ButtonSize)}");
        }

        if (cssClass.Length > 0)
        {
            output.Attributes.SetAttribute(HtmlAttribute.Class, cssClass.ToString());
        }

        // Write Confirm Dialog
        if (this.ReturnConfirmTag.IsSet())
        {
            this.BsToggle = "confirm";
            output.Attributes.SetAttribute("data-title", this.Get<ILocalization>().GetText(this.ReturnConfirmTag));
            output.Attributes.SetAttribute("data-yes", this.Get<ILocalization>().GetText("YES"));
            output.Attributes.SetAttribute("data-no", this.Get<ILocalization>().GetText("NO"));
        }

        // Write Modal
        if (this.BsTargetUrl.IsSet())
        {
            output.Attributes.SetAttribute("data-url", $"{this.BsTargetUrl}");
        }

        // Write popover content
        if (this.BsContent.IsSet())
        {
            output.Attributes.SetAttribute("data-bs-content", this.BsContent.Replace("\"", "'"));
            output.Attributes.SetAttribute(HtmlAttribute.Tabindex, "0");
        }

        if (this.BsDismiss.IsSet())
        {
            output.Attributes.SetAttribute("data-bs-dismiss", this.BsDismiss);
        }

        // Write Dropdown
        if (this.BsToggle.IsSet())
        {
            output.Attributes.SetAttribute("data-bs-toggle", this.BsToggle);

            switch (this.BsToggle)
            {
                case "ajax-modal":
                    output.Attributes.SetAttribute(HtmlAttribute.Type, HtmlTag.Button);
                    break;
                case "dropdown":
                    output.Attributes.SetAttribute(HtmlAttribute.AriaExpanded, "false");
                    output.Attributes.SetAttribute(HtmlAttribute.Type, HtmlTag.Button);
                    output.Attributes.SetAttribute("data-bs-auto-close", "outside");
                    break;
                case "collapse":
                    output.Attributes.SetAttribute("data-bs-target", this.BsTarget);
                    output.Attributes.SetAttribute(HtmlAttribute.AriaExpanded, "false");
                    break;
                case "confirm":
                    if (!output.Attributes.ContainsName(HtmlAttribute.Type))
                    {
                        output.Attributes.SetAttribute(HtmlAttribute.Type, output.Attributes.ContainsName("formaction") ? "submit" : HtmlTag.Button);
                    }

                    break;
            }
        }
        else
        {
            if (!output.Attributes.ContainsName(HtmlAttribute.Type))
            {
                output.Attributes.SetAttribute(
                    HtmlAttribute.Type,
                    output.Attributes.ContainsName("formaction") ? "submit" : "button");
            }
        }

        // Render Icon or Icon with Badge
        if (this.Icon.IsSet())
        {
            output.Content.AppendHtml(this.IconBade.IsSet() ? this.html.IconBadge(this.Icon, this.IconBade) : this.RenderIcon());
        }

        // Render span
        var spanTextTag = new TagBuilder(HtmlTag.Span);

        if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
        {
            spanTextTag.AddCssClass(
                this.IconMobileOnly ? "ms-1 d-none d-lg-inline-block" : "ms-1");

            spanTextTag.TagRenderMode = TagRenderMode.Normal;
        }

        // Render Text
        if (this.TextLocalizedTag.IsSet())
        {
            if (this.TextParam0.IsSet())
            {
                spanTextTag.InnerHtml.Append(
                    this.Get<ILocalization>().GetTextFormatted(this.TextLocalizedTag, this.TextParam0));
            }
            else
            {
                var text = this.TextLocalizedPage.IsSet()
                               ? this.Get<ILocalization>().GetText(this.TextLocalizedPage, this.TextLocalizedTag)
                               : this.Get<ILocalization>().GetText(this.TextLocalizedTag);

                spanTextTag.InnerHtml.Append(text);
            }
        }

        // Render normal Text
        if (this.Text.IsSet())
        {
            spanTextTag.InnerHtml.AppendHtml(this.Text);
        }

        if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
        {
            output.Content.AppendHtml(spanTextTag);
        }

        if (this.TitleNonLocalized.IsSet())
        {
            output.Attributes.SetAttribute(HtmlAttribute.Title, this.TitleNonLocalized);
        }

        // Render Title
        if (this.TitleLocalizedTag.IsNotSet())
        {
            return;
        }

        if (this.TitleParam0.IsSet())
        {
            var title =
                this.Get<ILocalization>().GetTextFormatted(this.TitleLocalizedTag, this.TitleParam0);

            output.Attributes.SetAttribute(HtmlAttribute.Title, title);
        }
        else
        {
            var title = this.TitleLocalizedPage.IsSet()
                            ? this.Get<ILocalization>().GetText(this.TitleLocalizedPage, this.TitleLocalizedTag)
                            : this.Get<ILocalization>().GetText(this.TitleLocalizedTag);

            output.Attributes.SetAttribute(HtmlAttribute.Title, title);
        }
    }

    /// <summary>
    /// Renders the icon.
    /// </summary>
    /// <returns>TagBuilder.</returns>
    public TagBuilder RenderIcon()
    {
        var iconColorClass = this.IconColor.IsSet() ? $" {this.IconColor}" : this.IconColor;

        var iconCssClass = this.IconCssClass.IsSet() ? this.IconCssClass : "fa";

        var iconTag = new TagBuilder(HtmlTag.I);

        // space separator only for icon + text
        if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
        {
            iconTag.AddCssClass($"{iconCssClass} fa-{this.Icon} fa-fw {iconColorClass} me-1");
        }
        else
        {
            iconTag.AddCssClass($"{iconCssClass} fa-{this.Icon} fa-fw {iconColorClass}");
        }

        iconTag.TagRenderMode = TagRenderMode.Normal;

        return iconTag;
    }

    /// <summary>
    /// Gets the CSS class value.
    /// </summary>
    /// <param name="mode">The button action.</param>
    /// <returns>Returns the CSS Class for the button</returns>
    /// <exception cref="InvalidOperationException">Exception when other value</exception>
    private static string GetAttributeValue(ButtonStyle mode)
    {
        return mode switch
            {
                ButtonStyle.Primary => "btn btn-primary",
                ButtonStyle.Secondary => "btn btn-secondary",
                ButtonStyle.OutlineSecondary => "btn btn-outline-secondary",
                ButtonStyle.Success => "btn btn-success",
                ButtonStyle.OutlineSuccess => "btn btn-outline-success",
                ButtonStyle.Danger => "btn btn-danger",
                ButtonStyle.Warning => "btn btn-warning",
                ButtonStyle.Info => "btn btn-info",
                ButtonStyle.OutlineInfo => "btn btn-outline-info",
                ButtonStyle.Light => "btn btn-light",
                ButtonStyle.Dark => "btn btn-dark",
                ButtonStyle.Link => "btn btn-link",
                ButtonStyle.None => string.Empty,
                _ => string.Empty
            };
    }

    /// <summary>
    /// Gets the CSS class value.
    /// </summary>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <returns>
    /// Returns the CSS Class for the button
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Exception when other value
    /// </exception>
    private static string GetButtonSizeClass(ButtonSize size)
    {
        return size switch
            {
                ButtonSize.Normal => string.Empty,
                ButtonSize.Large => "btn-lg",
                ButtonSize.Small => "btn-sm",
                _ => throw new InvalidOperationException()
            };
    }
}