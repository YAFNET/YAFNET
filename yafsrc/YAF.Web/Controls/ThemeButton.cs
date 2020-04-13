/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    using AttributeCollection = System.Web.UI.AttributeCollection;
    using ButtonStyle = YAF.Types.Constants.ButtonStyle;

    #endregion

    /// <summary>
    /// The theme button.
    /// </summary>
    public sealed class ThemeButton : BaseControl, IPostBackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        ///   The click event.
        /// </summary>
        private static readonly object ClickEvent = new object();

        /// <summary>
        ///   The command event.
        /// </summary>
        private static readonly object CommandEvent = new object();

        /// <summary>
        ///   The localized label.
        /// </summary>
        private readonly LocalizedLabel localizedLabel = new LocalizedLabel();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ThemeButton" /> class.
        /// </summary>
        public ThemeButton()
        {
            this.Load += this.ThemeButtonLoad;
            this.Attributes = new AttributeCollection(this.ViewState);
        }

        #endregion

        #region Events

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether enabled.
        /// </summary>
        public bool Enabled
        {
            get => this.ViewState["Enabled"] == null || this.ViewState["Enabled"].ToType<bool>();

            set => this.ViewState["Enabled"] = value;
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

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [CanBeNull]
        public string Text
        {
            get => this.ViewState["Text"] != null ? this.ViewState["Text"] as string : string.Empty;

            set => this.ViewState["Text"] = value;
        }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        [CanBeNull]
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
        [CanBeNull]
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
        [CanBeNull]
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
        [CanBeNull]
        public string IconColor
        {
            get => this.ViewState["IconColor"] != null ? this.ViewState["IconColor"] as string : string.Empty;

            set => this.ViewState["IconColor"] = value;
        }

        /// <summary>
        /// Gets or sets the return confirm text.
        /// </summary>
        /// <value>
        /// The return confirm text.
        /// </value>
        [CanBeNull]
        public string ReturnConfirmText
        {
            get =>
                this.ViewState["ReturnConfirmText"] != null
                    ? this.ViewState["ReturnConfirmText"] as string
                    : string.Empty;

            set => this.ViewState["ReturnConfirmText"] = value;
        }

        /// <summary>
        /// Gets or sets the return confirm event.
        /// </summary>
        [CanBeNull]
        public string ReturnConfirmEvent
        {
            get =>
                this.ViewState["ReturnConfirmEvent"] != null
                    ? this.ViewState["ReturnConfirmEvent"] as string
                    : string.Empty;

            set => this.ViewState["ReturnConfirmEvent"] = value;
        }

        /// <summary>
        /// Gets or sets the data target.
        /// </summary>
        /// <value>
        /// The data target.
        /// </value>
        [CanBeNull]
        public string DataTarget
        {
            get => this.ViewState["DataTarget"] != null ? this.ViewState["DataTarget"] as string : string.Empty;

            set => this.ViewState["DataTarget"] = value;
        }

        /// <summary>
        /// Gets or sets the data content.
        /// </summary>
        [CanBeNull]
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
        [CanBeNull]
        public string DataToggle
        {
            get => this.ViewState["DataToggle"] != null ? this.ViewState["DataToggle"] as string : string.Empty;

            set => this.ViewState["DataToggle"] = value;
        }

        /// <summary>
        /// Gets or sets the data dismiss.
        /// </summary>
        [CanBeNull]
        public string DataDismiss { get; set; }

        /// <summary>
        ///    Gets or sets the Setting the link property will make this control non post-back.
        /// </summary>
        [CanBeNull]
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
        [CanBeNull]
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
        [CanBeNull]
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
        [CanBeNull]
        public string TitleNonLocalized
        {
            get =>
                this.ViewState["TitleNonLocalized"] != null
                    ? this.ViewState["TitleNonLocalized"] as string
                    : string.Empty;

            set => this.ViewState["TitleNonLocalized"] = value;
        }

        #endregion

        #region Implemented Interfaces

        #region IPostBackEventHandler

        /// <summary>
        /// The i post back event handler. raise post back event.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        void IPostBackEventHandler.RaisePostBackEvent([NotNull] string eventArgument)
        {
            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
            this.OnClick(EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            // get the title...
            var title = this.GetLocalizedTitle();

            output.BeginRender();
            output.WriteBeginTag("a");
            output.WriteAttribute("id", this.ClientID);

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

                output.WriteAttribute("aria-disabled", "true");
            }

            if (this.CssClass.IsSet())
            {
                cssClass.AppendFormat(" {0}", this.CssClass);
            }

            cssClass.Append(" mb-1");

            output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), cssClass.ToString());

            if (title.IsSet())
            {
                output.WriteAttribute("title", HttpUtility.HtmlEncode(title));
            }
            else if (this.TitleNonLocalized.IsSet())
            {
                output.WriteAttribute("title", HttpUtility.HtmlEncode(this.TitleNonLocalized));
            }

            output.WriteAttribute(
                "href",
                this.NavigateUrl.IsSet()
                    ? this.NavigateUrl.Replace("&", "&amp;")
                    : this.Page.ClientScript.GetPostBackClientHyperlink(this, string.Empty));

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
                        output.WriteAttribute(key, $"{this.Attributes[key]};");
                    }
                    else if (key.ToLower().StartsWith("data-") || key.ToLower().StartsWith("on")
                                                               || key.ToLower() == "rel" || key.ToLower() == "target")
                    {
                        // only write javascript attributes -- and a few other attributes...
                        output.WriteAttribute(key, this.Attributes[key]);
                    }
                });
            }

            // Write Confirm Dialog
            if (this.ReturnConfirmText.IsSet())
            {
                this.DataToggle = "confirm";
                output.WriteAttribute("data-title", this.ReturnConfirmText);
                output.WriteAttribute("data-yes", this.GetText("YES"));
                output.WriteAttribute("data-no", this.GetText("NO"));

                if (this.ReturnConfirmEvent.IsSet())
                {
                    output.WriteAttribute("data-confirm-event", this.ReturnConfirmEvent);
                }
            }

            // Write Modal
            if (this.DataTarget.IsSet())
            {
                output.WriteAttribute("data-target", $"#{this.DataTarget}");

                if (this.DataTarget == "modal")
                {
                    output.WriteAttribute("aria-haspopup", "true");
                }
            }

            // Write popover content
            if (this.DataContent.IsSet())
            {
                output.WriteAttribute("data-content", this.DataContent.Replace("\"", "'"));
                output.WriteAttribute("tabindex", "0");
            }

            if (this.DataDismiss.IsSet())
            {
                output.WriteAttribute("data-dismiss", this.DataDismiss);
            }

            // Write Dropdown
            if (this.DataToggle.IsSet())
            {
                output.WriteAttribute("data-toggle", this.DataToggle);

                output.WriteAttribute("aria-expanded", "false");
            }

            if (this.Text.IsNotSet() && this.Icon.IsSet())
            {
                output.WriteAttribute("aria-label", this.Icon);
            }

            output.Write(HtmlTextWriter.TagRightChar);

            if (this.Icon.IsSet())
            {
                var iconColorClass = this.IconColor.IsSet() ? $" {this.IconColor}" : this.IconColor;

                output.Write("<i class=\"{2} fa-{0} fa-fw{1}\"></i>", this.Icon, iconColorClass, this.IconCssClass);

                // space separator only for icon + text
                if (this.TextLocalizedTag.IsSet() || this.Text.IsSet())
                {
                    output.Write("&nbsp;");
                }
            }

            if (this.Text.IsSet())
            {
                output.Write(this.Text);
            }

            // render the optional controls (if any)
            base.Render(output);

            // output.WriteEndTag("span");
            output.WriteEndTag("a");
            output.EndRender();
        }

        /// <summary>
        /// Gets the CSS class value.
        /// </summary>
        /// <param name="mode">The button action.</param>
        /// <returns>Returns the CSS Class for the button</returns>
        /// <exception cref="InvalidOperationException">Exception when other value</exception>
        private static string GetAttributeValue(ButtonStyle mode)
        {
            switch (mode)
            {
                case ButtonStyle.Primary:
                    return "btn btn-primary";
                case ButtonStyle.Secondary:
                    return "btn btn-secondary";
                case ButtonStyle.OutlineSecondary:
                    return "btn btn-outline-secondary";
                case ButtonStyle.Success:
                    return "btn btn-success";
                case ButtonStyle.OutlineSuccess:
                    return "btn btn-outline-success";
                case ButtonStyle.Danger:
                    return "btn btn-danger";
                case ButtonStyle.Warning:
                    return "btn btn-warning";
                case ButtonStyle.Info:
                    return "btn btn-info";
                case ButtonStyle.OutlineInfo:
                    return "btn btn-outline-info";
                case ButtonStyle.Light:
                    return "btn btn-light";
                case ButtonStyle.Dark:
                    return "btn btn-dark";
                case ButtonStyle.Link:
                    return "btn btn-link";
                case ButtonStyle.None:
                    return string.Empty;
                default:
                    throw new InvalidOperationException();
            }
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
            switch (size)
            {
                case ButtonSize.Normal:
                    return string.Empty;
                case ButtonSize.Large:
                    return "btn-lg";
                case ButtonSize.Small:
                    return "btn-sm";
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the localized title.
        /// </summary>
        /// <returns>
        /// The get localized title.
        /// </returns>
        private string GetLocalizedTitle()
        {
            if (this.Site != null && this.Site.DesignMode && this.TitleLocalizedTag.IsSet())
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
        private void OnClick([NotNull] EventArgs e)
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
        private void OnCommand([NotNull] CommandEventArgs e)
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
        private void ThemeButtonLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            // render the text if available
            if (this.localizedLabel.LocalizedTag.IsSet())
            {
                this.Controls.Add(this.localizedLabel);
            }
        }

        #endregion
    }
}