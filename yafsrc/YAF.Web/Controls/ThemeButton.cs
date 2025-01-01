﻿/* Yet Another Forum.NET
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

using AttributeCollection = System.Web.UI.AttributeCollection;

/// <summary>
/// The theme button.
/// </summary>
public class ThemeButton : BaseControl, IPostBackEventHandler, IButtonControl
{
    /// <summary>
    ///   The click event.
    /// </summary>
    private readonly static object ClickEvent = new();

    /// <summary>
    ///   The command event.
    /// </summary>
    private readonly static object CommandEvent = new();

    /// <summary>
    ///   The localized label.
    /// </summary>
    private readonly LocalizedLabel localizedLabel = new();

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ThemeButton" /> class.
    /// </summary>
    public ThemeButton()
    {
        this.Load += this.ThemeButtonLoad;
        this.Attributes = new AttributeCollection(this.ViewState);
    }

    public string ValidationGroup { get; set; }

    /// <summary>
    ///   The click.
    /// </summary>
    public event EventHandler Click
    {
        add => this.Events.AddHandler(ClickEvent, value);

        remove => this.Events.RemoveHandler(ClickEvent, value);
    }

    /// <summary>
    ///   The command.
    /// </summary>
    public event CommandEventHandler Command
    {
        add => this.Events.AddHandler(CommandEvent, value);

        remove => this.Events.RemoveHandler(CommandEvent, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether enabled.
    /// </summary>
    public bool Enabled
    {
        get => this.ViewState["Enabled"] == null || this.ViewState["Enabled"].ToType<bool>();

        set => this.ViewState["Enabled"] = value;
    }

    public bool IconMobileOnly
    {
        get => this.ViewState["IconMobileOnly"]?.ToType<bool>() ?? false;

        set => this.ViewState["IconMobileOnly"] = value;
    }

    /// <summary>
    /// Gets or sets the behavior mode (single-line, multiline, or password) of the <see cref="T:System.Web.UI.WebControls.TextBox" /> control.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(ButtonStyle.Primary)]
    public ButtonStyle Type
    {
        get => this.ViewState["Type"]?.ToType<ButtonStyle>() ?? ButtonStyle.Primary;

        set => this.ViewState["Type"] = value;
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [Category("Appearance")]
    [DefaultValue(ButtonSize.Normal)]
    public ButtonSize Size
    {
        get => this.ViewState["Size"]?.ToType<ButtonSize>() ?? ButtonSize.Normal;

        set => this.ViewState["Size"] = value;
    }

    /// <summary>
    ///   Gets Attributes.
    /// </summary>
    public AttributeCollection Attributes { get; }

    [DefaultValue(false)]
    public bool CausesValidation
    {
        get => this.ViewState["CausesValidation"]?.ToType<bool>() ?? false;
        set => this.ViewState["CausesValidation"] = value;
    }

    /// <summary>
    ///   Gets or sets CommandArgument.
    /// </summary>
    public string CommandArgument
    {
        get => this.ViewState["commandArgument"]?.ToString();

        set => this.ViewState["commandArgument"] = value;
    }

    /// <summary>
    ///   Gets or sets CommandName.
    /// </summary>
    public string CommandName
    {
        get => this.ViewState["commandName"]?.ToString();

        set => this.ViewState["commandName"] = value;
    }

    public string PostBackUrl { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    
    public string Text
    {
        get => this.ViewState["Text"] != null ? this.ViewState["Text"] as string : string.Empty;

        set => this.ViewState["Text"] = value;
    }

    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    
    public string CssClass
    {
        get => this.ViewState["CssClass"] != null ? this.ViewState["CssClass"] as string : string.Empty;

        set => this.ViewState["CssClass"] = value;
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    
    public string Icon
    {
        get => this.ViewState["Icon"] != null ? this.ViewState["Icon"] as string : string.Empty;

        set => this.ViewState["Icon"] = value;
    }

    /// <summary>
    /// Gets or sets the icon CSS Class.
    /// </summary>
    /// <value>
    /// The icon CSS class.
    /// </value>
    
    public string IconCssClass
    {
        get => this.ViewState["IconCssClass"] != null ? this.ViewState["IconCssClass"] as string : "fa";

        set => this.ViewState["IconCssClass"] = value;
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    
    public string IconColor
    {
        get => this.ViewState["IconColor"] != null ? this.ViewState["IconColor"] as string : string.Empty;

        set => this.ViewState["IconColor"] = value;
    }

    
    public string ReturnConfirmTag
    {
        get =>
            this.ViewState["ReturnConfirmTag"] != null
                ? this.ViewState["ReturnConfirmTag"] as string
                : string.Empty;

        set => this.ViewState["ReturnConfirmTag"] = value;
    }

    /// <summary>
    /// Gets or sets the data target.
    /// </summary>
    /// <value>
    /// The data target.
    /// </value>
    
    public string DataTarget
    {
        get => this.ViewState["DataTarget"] != null ? this.ViewState["DataTarget"] as string : string.Empty;

        set => this.ViewState["DataTarget"] = value;
    }

    /// <summary>
    /// Gets or sets the data content.
    /// </summary>
    
    public string DataContent
    {
        get => this.ViewState["DataContent"] != null ? this.ViewState["DataContent"] as string : string.Empty;

        set => this.ViewState["DataContent"] = value;
    }

    /// <summary>
    /// Gets or sets the data toggle.
    /// </summary>
    /// <value>
    /// The data toggle.
    /// </value>
    
    public string DataToggle
    {
        get => this.ViewState["DataToggle"] != null ? this.ViewState["DataToggle"] as string : string.Empty;

        set => this.ViewState["DataToggle"] = value;
    }

    /// <summary>
    /// Gets or sets the data dismiss.
    /// </summary>
    
    public string DataDismiss { get; set; }

    /// <summary>
    ///    Gets or sets the Setting the link property will make this control non post-back.
    /// </summary>
    
    public string NavigateUrl
    {
        get => this.ViewState["NavigateUrl"] != null ? this.ViewState["NavigateUrl"] as string : string.Empty;

        set => this.ViewState["NavigateUrl"] = value;
    }

    /// <summary>
    ///    Gets or sets the Localized Page for the optional button text
    /// </summary>
    public string TextLocalizedPage
    {
        get => this.localizedLabel.LocalizedPage;

        set => this.localizedLabel.LocalizedPage = value;
    }

    /// <summary>
    ///    Gets or sets the Localized Tag for the optional button text
    /// </summary>
    public string TextLocalizedTag
    {
        get => this.localizedLabel.LocalizedTag;

        set => this.localizedLabel.LocalizedTag = value;
    }

    /// <summary>
    ///    Gets or sets the Localized Page for the optional link description (title)
    /// </summary>
    
    public string TitleLocalizedPage
    {
        get =>
            this.ViewState["TitleLocalizedPage"] != null
                ? this.ViewState["TitleLocalizedPage"] as string
                : "BUTTON";

        set => this.ViewState["TitleLocalizedPage"] = value;
    }

    /// <summary>
    /// Gets or sets Parameter Title 0.
    /// </summary>
    public string ParamTitle0 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Parameter Title 1.
    /// </summary>
    public string ParamTitle1 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Parameter Title 2.
    /// </summary>
    public string ParamTitle2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets Parameter Text 0.
    /// </summary>
    public string ParamText0
    {
        get => this.localizedLabel.Param0;

        set => this.localizedLabel.Param0 = value;
    }

    /// <summary>
    /// Gets or sets Parameter Text 1.
    /// </summary>
    public string ParamText1
    {
        get => this.localizedLabel.Param1;

        set => this.localizedLabel.Param1 = value;
    }

    /// <summary>
    /// Gets or sets Parameter Text 2.
    /// </summary>
    public string ParamText2
    {
        get => this.localizedLabel.Param2;

        set => this.localizedLabel.Param2 = value;
    }

    /// <summary>
    ///    Gets or sets the Localized Tag for the optional link description (title)
    /// </summary>
    
    public string TitleLocalizedTag
    {
        get =>
            this.ViewState["TitleLocalizedTag"] != null
                ? this.ViewState["TitleLocalizedTag"] as string
                : string.Empty;

        set => this.ViewState["TitleLocalizedTag"] = value;
    }

    /// <summary>
    ///    Gets or sets the Non-localized Title for optional link description
    /// </summary>
    
    public string TitleNonLocalized
    {
        get =>
            this.ViewState["TitleNonLocalized"] != null
                ? this.ViewState["TitleNonLocalized"] as string
                : string.Empty;

        set => this.ViewState["TitleNonLocalized"] = value;
    }

    /// <summary>
    /// The i post back event handler. raise post back event.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        if (this.CausesValidation)
        {
            if (this.ValidationGroup.IsSet())
            {
                this.Page.Validate(this.ValidationGroup);
            }
            else
            {
                this.Page.Validate();
            }

            if (!this.Page.IsValid)
            {
                return;
            }

            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
            this.OnClick(EventArgs.Empty);
        }
        else
        {
            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
            this.OnClick(EventArgs.Empty);
        }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The output.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        if (this.CausesValidation)
        {
            var postBackOptions = new PostBackOptions(this)
                                      {
                                          PerformValidation = true,
                                          ValidationGroup = this.ValidationGroup,
                                          RequiresJavaScriptProtocol = true,
                                          ClientSubmit = false,
                                          AutoPostBack = false
                                      };

            this.Page.ClientScript.RegisterForEventValidation(postBackOptions);
        }

        // get the title...
        var title = this.GetLocalizedTitle();

        writer.BeginRender();
        writer.WriteBeginTag("a");
        writer.WriteAttribute(HtmlTextWriterAttribute.Id.ToString(), this.ClientID);

        var uniqueId = this.UniqueID;

        writer.WriteAttribute(HtmlTextWriterAttribute.Name.ToString(), uniqueId);

        var actionClass = GetAttributeValue(this.Type);

        var cssClass = new StringBuilder();

        cssClass.Append(actionClass);

        if (this.Size != ButtonSize.Normal)
        {
            cssClass.AppendFormat(" {0}", GetButtonSizeClass(this.Size));
        }

        if (!this.Enabled)
        {
            cssClass.Append(" disabled");

            writer.WriteAttribute("aria-disabled", "true");
        }

        if (this.CssClass.IsSet())
        {
            cssClass.AppendFormat(" {0}", this.CssClass);
        }

        writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), cssClass.ToString());

        if (title.IsSet())
        {
            writer.WriteAttribute("title", HttpUtility.HtmlEncode(title));
        }
        else if (this.TitleNonLocalized.IsSet())
        {
            writer.WriteAttribute("title", HttpUtility.HtmlEncode(this.TitleNonLocalized));
        }

        writer.WriteAttribute("role", "button");

        if (this.DataToggle.IsSet() && this.DataToggle is "dropdown" or "popover")
        {
            // this.NavigateUrl = "#";
        }
        else
        {
            writer.WriteAttribute(
                "href",
                this.NavigateUrl.IsSet()
                    ? this.NavigateUrl.Replace("&", "&amp;")
                    : this.Page.ClientScript.GetPostBackClientHyperlink(this, string.Empty));
        }

        // handle additional attributes (if any)
        if (this.Attributes.Count > 0)
        {
            // add attributes...
            this.Attributes.Keys.Cast<string>().ForEach(key =>

                {
                    // get the attribute and write it...
                    if (key.ToLower() == "onclick")
                    {
                        // special handling... add to it...
                        writer.WriteAttribute(key, $"{this.Attributes[key]};");
                    }
                    else if (key.ToLower().StartsWith("data-") || key.ToLower().StartsWith("on")
                                                               || key.ToLower() == "rel" || key.ToLower() == "target")
                    {
                        // only write javascript attributes -- and a few other attributes...
                        writer.WriteAttribute(key, this.Attributes[key]);
                    }
                });
        }

        // Write Confirm Dialog
        if (this.ReturnConfirmTag.IsSet())
        {
            this.DataToggle = "confirm";
            writer.WriteAttribute("data-title", this.GetText(this.ReturnConfirmTag));
            writer.WriteAttribute("data-yes", this.GetText("YES"));
            writer.WriteAttribute("data-no", this.GetText("NO"));
        }

        // Write Modal
        if (this.DataTarget.IsSet())
        {
            writer.WriteAttribute("data-bs-target", $"#{this.DataTarget}");

            if (this.DataTarget == "modal")
            {
                writer.WriteAttribute("aria-haspopup", "true");
            }
        }

        // Write popover content
        if (this.DataContent.IsSet())
        {
            writer.WriteAttribute("data-bs-content", this.DataContent.Replace("\"", "'"));
            writer.WriteAttribute("tabindex", "0");
        }

        if (this.DataDismiss.IsSet())
        {
            writer.WriteAttribute("data-bs-dismiss", this.DataDismiss);
        }

        // Write Dropdown
        if (this.DataToggle.IsSet())
        {
            writer.WriteAttribute("data-bs-toggle", this.DataToggle);

            writer.WriteAttribute("aria-expanded", "false");

            if (this.DataToggle is "dropdown")
            {
                writer.WriteAttribute("data-bs-auto-close", "outside");
            }
        }

        if (this.Text.IsNotSet() && this.Icon.IsSet())
        {
            writer.WriteAttribute("aria-label", this.Icon);
        }

        writer.Write(HtmlTextWriter.TagRightChar);

        if (this.Icon.IsSet())
        {
            var iconColorClass = this.IconColor.IsSet() ? $" {this.IconColor}" : this.IconColor;

            writer.Write("<i class=\"{2} fa-{0} fa-fw{1}\"></i>", this.Icon, iconColorClass, this.IconCssClass);
        }

        if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
        {
            writer.WriteBeginTag("span");

            writer.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                this.IconMobileOnly ? "ms-1 d-none d-lg-inline-block" : "ms-1");

            writer.Write(HtmlTextWriter.TagRightChar);
        }

        if (this.Text.IsSet())
        {
            writer.Write(this.Text);
        }

        // render the optional controls (if any)
        base.Render(writer);

        writer.WriteEndTag("span");

        writer.WriteEndTag("a");
        writer.EndRender();
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
                _ => throw new InvalidOperationException()
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

    /// <summary>
    /// Gets the localized title.
    /// </summary>
    /// <returns>
    /// The get localized title.
    /// </returns>
    private string GetLocalizedTitle()
    {
        if (this.Site is { DesignMode: true } && this.TitleLocalizedTag.IsSet())
        {
            return $"[TITLE:{this.TitleLocalizedTag}]";
        }

        if (this.TitleLocalizedPage.IsSet() && this.TitleLocalizedTag.IsSet())
        {
            return string.Format(
                this.GetText(this.TitleLocalizedPage, this.TitleLocalizedTag),
                this.ParamTitle0,
                this.ParamTitle1,
                this.ParamTitle2);
        }

        return this.TitleLocalizedTag.IsSet()
                   ? string.Format(
                       this.GetText(this.TitleLocalizedTag),
                       this.ParamTitle0,
                       this.ParamTitle1,
                       this.ParamTitle2)
                   : null;
    }

    /// <summary>
    /// The on click.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    private void OnClick(EventArgs e)
    {
        var handler = (EventHandler)this.Events[ClickEvent];
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// The on command.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    private void OnCommand(CommandEventArgs e)
    {
        var handler = (CommandEventHandler)this.Events[CommandEvent];

        handler?.Invoke(this, e);

        this.RaiseBubbleEvent(this, e);
    }

    /// <summary>
    /// Setup the controls before render
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void ThemeButtonLoad(object sender, EventArgs e)
    {
        // render the text if available
        if (this.localizedLabel.LocalizedTag.IsSet())
        {
            this.Controls.Add(this.localizedLabel);
        }
    }
}