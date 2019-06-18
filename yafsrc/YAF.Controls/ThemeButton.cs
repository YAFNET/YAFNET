/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.ComponentModel;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    using AttributeCollection = System.Web.UI.AttributeCollection;

    #endregion

    /// <summary>
    /// The theme button.
    /// </summary>
    public class ThemeButton : BaseControl, IPostBackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The _param Title 0.
        /// </summary>
        protected string _paramTitle0 = string.Empty;

        /// <summary>
        /// The _param Title 1.
        /// </summary>
        protected string _paramTitle1 = string.Empty;

        /// <summary>
        /// The _param Title 2.
        /// </summary>
        protected string _paramTitle2 = string.Empty;

        /// <summary>
        ///   The _click event.
        /// </summary>
        protected static object _clickEvent = new object();

        /// <summary>
        ///   The _command event.
        /// </summary>
        protected static object _commandEvent = new object();

        /// <summary>
        ///   The _attribute collection.
        /// </summary>
        protected AttributeCollection _attributeCollection;

        /// <summary>
        ///   The _localized label.
        /// </summary>
        protected LocalizedLabel _localizedLabel = new LocalizedLabel();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ThemeButton" /> class.
        /// </summary>
        public ThemeButton()
        {
            this.Load += this.ThemeButtonLoad;
            this._attributeCollection = new AttributeCollection(this.ViewState);
        }

        #endregion

        #region Events

        /// <summary>
        ///   The click.
        /// </summary>
        public event EventHandler Click
        {
            add => this.Events.AddHandler(_clickEvent, value);

            remove => this.Events.RemoveHandler(_clickEvent, value);
        }

        /// <summary>
        ///   The command.
        /// </summary>
        public event CommandEventHandler Command
        {
            add => this.Events.AddHandler(_commandEvent, value);

            remove => this.Events.RemoveHandler(_commandEvent, value);
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
        /// [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonAction.Primary)]
        public ButtonAction Type
        {
            get => this.ViewState["Type"] != null ? this.ViewState["Type"].ToType<ButtonAction>() : ButtonAction.Primary;

            set => this.ViewState["Type"] = value;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(ButtonSize.Normal)]
        public ButtonSize Size
        {
            get => this.ViewState["Size"] != null ? this.ViewState["Size"].ToType<ButtonSize>() : ButtonSize.Normal;

            set => this.ViewState["Size"] = value;
        }

        /// <summary>
        ///   Gets Attributes.
        /// </summary>
        public AttributeCollection Attributes => this._attributeCollection;

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
        ///   Gets or sets the Defaults to "yafcssbutton"
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
        /// Gets or sets the return confirm text.
        /// </summary>
        /// <value>
        /// The return confirm text.
        /// </value>
        [CanBeNull]
        public string ReturnConfirmText
        {
            get => this.ViewState["ReturnConfirmText"] != null ? this.ViewState["ReturnConfirmText"] as string : string.Empty;

            set => this.ViewState["ReturnConfirmText"] = value;
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
        ///    Gets or sets the Setting the link property will make this control non postback.
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
            get => this._localizedLabel.LocalizedPage;

            set => this._localizedLabel.LocalizedPage = value;
        }

        /// <summary>
        ///    Gets or sets the Localized Tag for the optional button text
        /// </summary>
        public string TextLocalizedTag
        {
            get => this._localizedLabel.LocalizedTag;

            set => this._localizedLabel.LocalizedTag = value;
        }

        /// <summary>
        ///    Gets or sets the Localized Page for the optional link description (title)
        /// </summary>
        [CanBeNull]
        public string TitleLocalizedPage
        {
            get => this.ViewState["TitleLocalizedPage"] != null ? this.ViewState["TitleLocalizedPage"] as string : "BUTTON";

            set => this.ViewState["TitleLocalizedPage"] = value;
        }

        /// <summary>
        /// Gets or sets Param Title 0.
        /// </summary>
        public string ParamTitle0
        {
            get => this._paramTitle0;

            set => this._paramTitle0 = value;
        }

        /// <summary>
        /// Gets or sets Param Title 1.
        /// </summary>
        public string ParamTitle1
        {
            get => this._paramTitle1;

            set => this._paramTitle1 = value;
        }

        /// <summary>
        /// Gets or sets Param Title 2.
        /// </summary>
        public string ParamTitle2
        {
            get => this._paramTitle2;

            set => this._paramTitle2 = value;
        }

        /// <summary>
        /// Gets or sets Param Text 0.
        /// </summary>
        public string ParamText0
        {
            get => this._localizedLabel.Param0;

            set => this._localizedLabel.Param0 = value;
        }

        /// <summary>
        /// Gets or sets Param Text 1.
        /// </summary>
        public string ParamText1
        {
            get => this._localizedLabel.Param1;

            set => this._localizedLabel.Param1 = value;
        }

        /// <summary>
        /// Gets or sets Param Text 2.
        /// </summary>
        public string ParamText2
        {
            get => this._localizedLabel.Param2;

            set => this._localizedLabel.Param2 = value;
        }

        /// <summary>
        ///    Gets or sets the Localized Tag for the optional link description (title)
        /// </summary>
        [CanBeNull]
        public string TitleLocalizedTag
        {
            get => this.ViewState["TitleLocalizedTag"] != null ? this.ViewState["TitleLocalizedTag"] as string : string.Empty;

            set => this.ViewState["TitleLocalizedTag"] = value;
        }

        /// <summary>
        ///    Gets or sets the Non-localized Title for optional link description
        /// </summary>
        [CanBeNull]
        public string TitleNonLocalized
        {
            get => this.ViewState["TitleNonLocalized"] != null ? this.ViewState["TitleNonLocalized"] as string : string.Empty;

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
        /// Gets the localized title.
        /// </summary>
        /// <returns>
        /// The get localized title.
        /// </returns>
        protected string GetLocalizedTitle()
        {
            if (this.Site != null && this.Site.DesignMode && this.TitleLocalizedTag.IsSet())
            {
                return $"[TITLE:{this.TitleLocalizedTag}]";
            }

            if (this.TitleLocalizedPage.IsSet() && this.TitleLocalizedTag.IsSet())
            {
                return string.Format(
                           this.GetText(this.TitleLocalizedPage, this.TitleLocalizedTag), this.ParamTitle0,this.ParamTitle1, this.ParamTitle2);
            }

            return this.TitleLocalizedTag.IsSet()
                       ? string.Format(
                           this.GetText(this.TitleLocalizedTag), this.ParamTitle0,this.ParamTitle1, this.ParamTitle2)
                       : null;
        }

        /// <summary>
        /// The on click.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnClick([NotNull] EventArgs e)
        {
            var handler = (EventHandler)this.Events[_clickEvent];
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// The on command.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnCommand([NotNull] CommandEventArgs e)
        {
            var handler = (CommandEventHandler)this.Events[_commandEvent];

            handler?.Invoke(this, e);

            this.RaiseBubbleEvent(this, e);
        }

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

            var actionClass = this.GetAttributeValue(this.Type);

            var cssClass = new StringBuilder();

            cssClass.Append(actionClass);

            if (this.Size != ButtonSize.Normal)
            {
                cssClass.AppendFormat(" {0}", this.GetButtonSizeClass(this.Size));
            }

            if (!this.Enabled)
            {
                cssClass.Append(" disabled");

                output.WriteAttribute(
                    "aria-disabled",
                    "true");
            }

            if (this.CssClass.IsSet())
            {
                cssClass.AppendFormat(" {0}", this.CssClass);
            }

            output.WriteAttribute(
                HtmlTextWriterAttribute.Class.ToString(),
                cssClass.ToString());


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
            if (this._attributeCollection.Count > 0)
            {
                // add attributes...
                foreach (string key in this._attributeCollection.Keys)
                {
                    // get the attribute and write it...
                    if (key.ToLower() == "onclick")
                    {
                        // special handling... add to it...
                        output.WriteAttribute(key, $"{this._attributeCollection[key]};");
                    }
                    else if (key.ToLower().StartsWith("data-") || key.ToLower().StartsWith("on")
                                                               || key.ToLower() == "rel" || key.ToLower() == "target")
                    {
                        // only write javascript attributes -- and a few other attributes...
                        output.WriteAttribute(key, this._attributeCollection[key]);
                    }
                }
            }

            // Write Confirm Dialog
            if (this.ReturnConfirmText.IsSet())
            {
                output.WriteAttribute("onclick", $"return confirm('{this.ReturnConfirmText}')");
            }

            // Write Modal
            if (this.DataTarget.IsSet())
            {
                output.WriteAttribute("data-toggle", "modal");
                output.WriteAttribute("data-target", $"#{this.DataTarget}");
            }

            // Write Dropdown
            if (this.DataToggle.IsSet())
            {
                output.WriteAttribute("data-toggle", "dropdown");
                output.WriteAttribute("aria-haspopup", "true");
                output.WriteAttribute("aria-expanded", "false");
            }

            output.Write(HtmlTextWriter.TagRightChar);

            // output.WriteBeginTag("span");
            // output.Write(HtmlTextWriter.TagRightChar);
            if (this.Icon.IsSet())
            {
                if (this.CssClass.IsSet() && this.CssClass.Equals("fab"))
                {
                    output.Write("<i class=\"fab fa-{0} fa-fw\"></i>&nbsp;", this.Icon);
                }
                else
                {
                    output.Write("<i class=\"fa fa-{0} fa-fw align-middle\"></i>&nbsp;", this.Icon);
                }
            }

            // render the optional controls (if any)
            base.Render(output);

            // output.WriteEndTag("span");
            output.WriteEndTag("a");
            output.EndRender();
        }

        /// <summary>
        /// Setup the controls before render
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ThemeButtonLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            // render the text if available
            if (this._localizedLabel.LocalizedTag.IsSet())
            {
               this.Controls.Add(this._localizedLabel);
            }
        }

        /// <summary>
        /// Gets the css class value.
        /// </summary>
        /// <param name="mode">The button action.</param>
        /// <returns>Returns the Css Class for the button</returns>
        /// <exception cref="InvalidOperationException">Exception when other value</exception>
        private string GetAttributeValue(ButtonAction mode)
        {
            switch (mode)
            {
                case ButtonAction.Primary:
                    return "btn btn-primary";
                case ButtonAction.Secondary:
                    return "btn btn-secondary";
                case ButtonAction.OutlineSecondary:
                    return "btn btn-outline-secondary";
                case ButtonAction.Success:
                    return "btn btn-success";
                case ButtonAction.OutlineSuccess:
                    return "btn btn-outline-success";
                case ButtonAction.Danger:
                    return "btn btn-danger";
                case ButtonAction.Warning:
                    return "btn btn-warning";
                case ButtonAction.Info:
                    return "btn btn-info";
                case ButtonAction.OutlineInfo:
                    return "btn btn-outline-info";
                case ButtonAction.Light:
                    return "btn btn-light";
                case ButtonAction.Dark:
                    return "btn btn-dark";
                case ButtonAction.Link:
                    return "btn btn-link";
                case ButtonAction.None:
                    return string.Empty;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets the css class value.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// Returns the Css Class for the button
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Exception when other value
        /// </exception>
        private string GetButtonSizeClass(ButtonSize size)
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

        #endregion
    }
}