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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
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