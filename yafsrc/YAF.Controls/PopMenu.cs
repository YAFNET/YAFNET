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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// PopMenu Control
    /// </summary>
    public class PopMenu : BaseControl, IPostBackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        ///   The _items.
        /// </summary>
        private readonly List<InternalPopMenuItem> _items = new List<InternalPopMenuItem>();

        /// <summary>
        ///   The _control.
        /// </summary>
        private string _control = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PopMenu" /> class.
        /// </summary>
        public PopMenu()
        {
            Init += this.PopMenu_Init;
        }

        #endregion

        #region Events

        /// <summary>
        ///   The item click.
        /// </summary>
        public event PopEventHandler ItemClick;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Control.
        /// </summary>
        public string Control
        {
            get
            {
                return this._control;
            }

            set
            {
                this._control = value;
            }
        }

        /// <summary>
        ///   Gets ControlOnClick.
        /// </summary>
        public string ControlOnClick
        {
            get
            {
                return "yaf_popit('{0}')".FormatWith(this.ClientID);
            }
        }

        /// <summary>
        ///   Gets ControlOnMouseOver.
        /// </summary>
        public string ControlOnMouseOver
        {
            get
            {
                return "yaf_mouseover('{0}')".FormatWith(this.ClientID);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add client script item.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        public void AddClientScriptItem([NotNull] string description, [NotNull] string clientScript)
        {
            this._items.Add(new InternalPopMenuItem(description, null, clientScript, null));
        }

        /// <summary>
        /// The add client script item with Icon.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public void AddClientScriptItem([NotNull] string description, [NotNull] string clientScript, [NotNull] string icon)
        {
            this._items.Add(new InternalPopMenuItem(description, null, clientScript, icon));
        }

        /// <summary>
        /// Add a item with a client script and post back option. (Use {postbackcode} in the <paramref name="clientScript"/> code for the postback code.)
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="argument">
        /// post back argument
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        public void AddClientScriptItemWithPostback([NotNull] string description, [NotNull] string argument, [NotNull] string clientScript)
        {
            this._items.Add(new InternalPopMenuItem(description, argument, clientScript, null));
        }

        /// <summary>
        /// The add post back item.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public void AddPostBackItem([NotNull] string argument, [NotNull] string description)
        {
            this._items.Add(new InternalPopMenuItem(description, argument, null, null));
        }

        /// <summary>
        /// The add post back item with an Icon
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public void AddPostBackItem([NotNull] string argument, [NotNull] string description, [NotNull] string icon)
        {
            this._items.Add(new InternalPopMenuItem(description, argument, null, icon));
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="ctl">
        /// The ctl.
        /// </param>
        public void Attach([NotNull] WebControl ctl)
        {
            ctl.Attributes["onclick"] = this.ControlOnClick;
            ctl.Attributes["onmouseover"] = this.ControlOnMouseOver;
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="userLinkControl">
        /// The user link control.
        /// </param>
        public void Attach([NotNull] UserLink userLinkControl)
        {
            userLinkControl.OnClick = this.ControlOnClick;
            userLinkControl.OnMouseOver = this.ControlOnMouseOver;
        }

        /// <summary>
        /// The remove post back item.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public void RemovePostBackItem([NotNull] string argument)
        {
            foreach (InternalPopMenuItem item in this._items.Where(item => item.PostBackArgument == argument))
            {
                this._items.Remove(item);
                break;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IPostBackEventHandler

        /// <summary>
        /// The raise post back event.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        public void RaisePostBackEvent([NotNull] string eventArgument)
        {
            if (this.ItemClick != null)
            {
                this.ItemClick(this, new PopEventArgs(eventArgument));
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (!Visible)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendFormat(
              @"<div class=""yafpopupmenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;display:none;"">",
              ClientID);
            sb.Append("<ul>");

            // add the items
            foreach (InternalPopMenuItem thisItem in this._items)
            {
                string onClick;
                string iconImage = string.Empty;

                if (thisItem.ClientScript.IsSet())
                {
                    // js style
                    onClick = thisItem.ClientScript.Replace(
                      "{postbackcode}", Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument));
                }
                else
                {
                    onClick = Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument);
                }

                if (thisItem.Icon.IsSet())
                {
                    iconImage = @"<img class=""popupitemIcon"" src=""{0}"" alt=""{1}"" title=""{1}"" />&nbsp;".FormatWith(
                        thisItem.Icon, thisItem.Description);
                }

                sb.AppendFormat(
                  @"<li class=""popupitem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{2}"" style=""white-space:nowrap"" title=""{1}"">{0}{1}</li>",
                  iconImage,
                  thisItem.Description,
                  onClick);
            }

            sb.AppendFormat("</ul></div>");

            writer.WriteLine(sb.ToString());

            base.Render(writer);
        }

        /// <summary>
        /// The pop menu_ init.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PopMenu_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // init the necessary js...
            PageContext.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
        }

        #endregion
    }

    /// <summary>
    /// The pop event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void PopEventHandler(object sender, PopEventArgs e);

    /// <summary>
    /// The internal pop menu item.
    /// </summary>
    public class InternalPopMenuItem
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalPopMenuItem"/> class.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="postbackArgument">
        /// The postback argument.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public InternalPopMenuItem([NotNull] string description, [NotNull] string postbackArgument, [NotNull] string clientScript, [NotNull] string icon)
        {
            this.Description = description;
            this.PostBackArgument = postbackArgument;
            this.ClientScript = clientScript;
            this.Icon = icon;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Icon.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        ///   Gets or sets ClientScript.
        /// </summary>
        public string ClientScript { get; set; }

        /// <summary>
        ///   Gets or sets Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///   Gets or sets PostBackArgument.
        /// </summary>
        public string PostBackArgument { get; set; }

        #endregion
    }
}