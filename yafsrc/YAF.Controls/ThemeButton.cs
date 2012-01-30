/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

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

        /// <summary>
        ///   The _theme image.
        /// </summary>
        protected ThemeImage _themeImage = new ThemeImage();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ThemeButton" /> class.
        /// </summary>
        public ThemeButton()
        {
            Load += this.ThemeButton_Load;
            this._attributeCollection = new AttributeCollection(ViewState);
        }

        #endregion

        #region Events

        /// <summary>
        ///   The click.
        /// </summary>
        public event EventHandler Click
        {
            add
            {
                Events.AddHandler(_clickEvent, value);
            }

            remove
            {
                Events.RemoveHandler(_clickEvent, value);
            }
        }

        /// <summary>
        ///   The command.
        /// </summary>
        public event CommandEventHandler Command
        {
            add
            {
                Events.AddHandler(_commandEvent, value);
            }

            remove
            {
                Events.RemoveHandler(_commandEvent, value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Attributes.
        /// </summary>
        public AttributeCollection Attributes
        {
            get
            {
                return this._attributeCollection;
            }
        }

        /// <summary>
        ///   Gets or sets CommandArgument.
        /// </summary>
        public string CommandArgument
        {
            get
            {
                return this.ViewState["commandArgument"] != null ? this.ViewState["commandArgument"].ToString() : null;
            }

            set
            {
                ViewState["commandArgument"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets CommandName.
        /// </summary>
        public string CommandName
        {
            get
            {
                return this.ViewState["commandName"] != null ? this.ViewState["commandName"].ToString() : null;
            }

            set
            {
                ViewState["commandName"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets the Defaults to "yafcssbutton"
        /// </summary>
        [CanBeNull]
        public string CssClass
        {
            get
            {
                return (ViewState["CssClass"] != null) ? ViewState["CssClass"] as string : "yafcssbutton";
            }

            set
            {
                ViewState["CssClass"] = value;
            }
        }

        /// <summary>
        ///    Gets or sets the ThemePage for the optional button image
        /// </summary>
        public string ImageThemePage
        {
            get
            {
                return this._themeImage.ThemePage;
            }

            set
            {
                this._themeImage.ThemePage = value;
            }
        }

        /// <summary>
        ///    Gets or sets the ThemeTag for the optional button image
        /// </summary>
        public string ImageThemeTag
        {
            get
            {
                return this._themeImage.ThemeTag;
            }

            set
            {
                this._themeImage.ThemeTag = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Setting the link property will make this control non-postback.
        /// </summary>
        [CanBeNull]
        public string NavigateUrl
        {
            get
            {
                return (ViewState["NavigateUrl"] != null) ? ViewState["NavigateUrl"] as string : string.Empty;
            }

            set
            {
                ViewState["NavigateUrl"] = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Localized Page for the optional button text
        /// </summary>
        public string TextLocalizedPage
        {
            get
            {
                return this._localizedLabel.LocalizedPage;
            }

            set
            {
                this._localizedLabel.LocalizedPage = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Localized Tag for the optional button text
        /// </summary>
        public string TextLocalizedTag
        {
            get
            {
                return this._localizedLabel.LocalizedTag;
            }

            set
            {
                this._localizedLabel.LocalizedTag = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Localized Page for the optional link description (title)
        /// </summary>
        [CanBeNull]
        public string TitleLocalizedPage
        {
            get
            {
                return (ViewState["TitleLocalizedPage"] != null) ? ViewState["TitleLocalizedPage"] as string : "BUTTON";
            }

            set
            {
                ViewState["TitleLocalizedPage"] = value;
            }
        }

        /// <summary>
        /// Gets or sets Param Title 0.
        /// </summary>
        public string ParamTitle0
        {
            get
            {
                return this._paramTitle0;
            }

            set
            {
                this._paramTitle0 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param Title 1.
        /// </summary>
        public string ParamTitle1
        {
            get
            {
                return this._paramTitle1;
            }

            set
            {
                this._paramTitle1 = value;
            }
        }

        /// <summary>
        /// Gets or sets Param Title 2.
        /// </summary>
        public string ParamTitle2
        {
            get
            {
                return this._paramTitle2;
            }

            set
            {
                this._paramTitle2 = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Localized Tag for the optional link description (title)
        /// </summary>
        [CanBeNull]
        public string TitleLocalizedTag
        {
            get
            {
                return (ViewState["TitleLocalizedTag"] != null) ? ViewState["TitleLocalizedTag"] as string : string.Empty;
            }

            set
            {
                ViewState["TitleLocalizedTag"] = value;
            }
        }

        /// <summary>
        ///    Gets or sets the Non-localized Title for optional link description
        /// </summary>
        [CanBeNull]
        public string TitleNonLocalized
        {
            get
            {
                return (ViewState["TitleNonLocalized"] != null) ? ViewState["TitleNonLocalized"] as string : string.Empty;
            }

            set
            {
                ViewState["TitleNonLocalized"] = value;
            }
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
        /// The get localized title.
        /// </summary>
        /// <returns>
        /// The get localized title.
        /// </returns>
        protected string GetLocalizedTitle()
        {
            if (Site != null && this.Site.DesignMode && this.TitleLocalizedTag.IsSet())
            {
                return "[TITLE:{0}]".FormatWith(this.TitleLocalizedTag);
            }

            if (this.TitleLocalizedPage.IsSet() && this.TitleLocalizedTag.IsSet())
            {
                return this.GetText(this.TitleLocalizedPage, this.TitleLocalizedTag).FormatWith(
                           this.ParamTitle0, this.ParamTitle1, this.ParamTitle2);
            }

            return this.TitleLocalizedTag.IsSet()
                       ? this.GetText(this.TitleLocalizedTag).FormatWith(
                           this.ParamTitle0, this.ParamTitle1, this.ParamTitle2)
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
            var handler = (EventHandler)Events[_clickEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The on command.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnCommand([NotNull] CommandEventArgs e)
        {
            var handler = (CommandEventHandler)Events[_commandEvent];

            if (handler != null)
            {
                handler(this, e);
            }

            RaiseBubbleEvent(this, e);
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
            string title = this.GetLocalizedTitle();

            output.BeginRender();
            output.WriteBeginTag("a");
            output.WriteAttribute("id", ClientID);
            if (this.CssClass.IsSet())
            {
                output.WriteAttribute("class", this.CssClass);
            }

            if (title.IsSet())
            {
                output.WriteAttribute("title", title);
            }
            else if (this.TitleNonLocalized.IsSet())
            {
                output.WriteAttribute("title", this.TitleNonLocalized);
            }

            output.WriteAttribute(
                "href",
                this.NavigateUrl.IsSet()
                    ? this.NavigateUrl.Replace("&", "&amp;")
                    : this.Page.ClientScript.GetPostBackClientHyperlink(this, string.Empty));

            bool wroteOnClick = false;

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
                        output.WriteAttribute(
                          key,
                          "{0};{1}".FormatWith(
                            this._attributeCollection[key], "this.blur(); this.onclick = function() { return false; }; return true;"));
                        wroteOnClick = true;
                    }
                    else if (key.ToLower().StartsWith("on") || key.ToLower() == "rel" || key.ToLower() == "target")
                    {
                        // only write javascript attributes -- and a few other attributes...
                        output.WriteAttribute(key, this._attributeCollection[key]);
                    }
                }
            }

            // IE fix
            if (!wroteOnClick)
            {
                output.WriteAttribute("onclick", "this.blur(); this.onclick = function() { return false; }; return true;");
            }

            output.Write(HtmlTextWriter.TagRightChar);

            output.WriteBeginTag("span");
            output.Write(HtmlTextWriter.TagRightChar);

            // render the optional controls (if any)
            base.Render(output);
            output.WriteEndTag("span");

            output.WriteEndTag("a");
            output.EndRender();
        }

        /// <summary>
        /// Setup the controls before render
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ThemeButton_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this._themeImage.ThemeTag.IsSet())
            {
                // add the theme image...
                Controls.Add(this._themeImage);
            }

            // render the text if available
            if (this._localizedLabel.LocalizedTag.IsSet())
            {
                Controls.Add(this._localizedLabel);
            }
        }

        #endregion
    }
}