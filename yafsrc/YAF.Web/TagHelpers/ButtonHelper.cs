/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using YAF.Types.Attributes;

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

    [DefaultValue(ButtonStyle.None)]
    public ButtonStyle ButtonStyle { get; set; }

    [DefaultValue(ButtonSize.Normal)]
    public ButtonSize ButtonSize { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    [CanBeNull]
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the icon CSS Class.
    /// </summary>
    /// <value>
    /// The icon CSS class.
    /// </value>
    [CanBeNull]
    public string IconCssClass { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    [CanBeNull]
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
    [CanBeNull]
    public string ReturnConfirmTag { get; set; }

    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the data target.
    /// </summary>
    /// <value>
    /// The data target.
    /// </value>
    [CanBeNull]
    public string BsTargetUrl { get; set; }

    [CanBeNull]
    public string BsTarget { get; set; }

    /// <summary>
    /// Gets or sets the data toggle.
    /// </summary>
    /// <value>
    /// The data toggle.
    /// </value>
    [CanBeNull]
    public string BsToggle { get; set; }

    /// <summary>
    /// Gets or sets the data dismiss.
    /// </summary>
    [CanBeNull]
    public string BsDismiss { get; set; }

    [CanBeNull]
    public string BsContent { get; set; }

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
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var actionClass = GetAttributeValue(this.ButtonStyle);

        var cssClass = new StringBuilder();

        cssClass.Append(actionClass);

        if (output.Attributes.ContainsName("class"))
        {
            cssClass.Append($" {output.Attributes["class"].Value}");
        }

        if (output.TagName == "a" && !output.Attributes.ContainsName("role"))
        {
            output.Attributes.SetAttribute("role", "button");
        }

        if (this.ButtonSize != ButtonSize.Normal)
        {
            cssClass.AppendFormat(" {0}", GetButtonSizeClass(this.ButtonSize));
        }

        if (cssClass.Length > 0)
        {
            output.Attributes.SetAttribute("class", cssClass.ToString());
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
            output.Attributes.SetAttribute("data-bs-url", $"{this.BsTargetUrl}");
        }

        // Write popover content
        if (this.BsContent.IsSet())
        {
            output.Attributes.SetAttribute("data-bs-content", this.BsContent.Replace("\"", "'"));
            output.Attributes.SetAttribute("tabindex", "0");
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
                    output.Attributes.SetAttribute("type", "button");
                    break;
                case "dropdown":
                    output.Attributes.SetAttribute("aria-expanded", "false");
                    output.Attributes.SetAttribute("type", "button");
                    break;
                case "collapse":
                    output.Attributes.SetAttribute("data-bs-target", this.BsTarget);
                    output.Attributes.SetAttribute("aria-expanded", "false");
                    break;
                case "confirm":
                    if (!output.Attributes.ContainsName("type"))
                    {
                        output.Attributes.SetAttribute("type", output.Attributes.ContainsName("formaction") ? "submit" : "button");
                    }

                    break;
            }
        }
        else
        {
            if (!output.Attributes.ContainsName("type"))
            {
                output.Attributes.SetAttribute(
                    "type",
                    output.Attributes.ContainsName("formaction") ? "submit" : "button");
            }
        }

        // Render Icon
        if (this.Icon.IsSet())
        {
            var iconColorClass = this.IconColor.IsSet() ? $" {this.IconColor}" : this.IconColor;

            var iconCssClass = this.IconCssClass.IsSet() ? this.IconCssClass : "fa";

            var iconTag = new TagBuilder("i");

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

            output.Content.AppendHtml(iconTag);
        }

        // Render span
        var spanTextTag = new TagBuilder("span");

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
            spanTextTag.InnerHtml.Append(this.Text);
        }

        if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
        {
            output.Content.AppendHtml(spanTextTag);
        }

        if (this.TitleNonLocalized.IsSet())
        {
            output.Attributes.SetAttribute("title", this.TitleNonLocalized);
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

            output.Attributes.SetAttribute("title", title);
        }
        else
        {
            var title = this.TitleLocalizedPage.IsSet()
                            ? this.Get<ILocalization>().GetText(this.TitleLocalizedPage, this.TitleLocalizedTag)
                            : this.Get<ILocalization>().GetText(this.TitleLocalizedTag);

            output.Attributes.SetAttribute("title", title);
        }
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