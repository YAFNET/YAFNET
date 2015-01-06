/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Attachments List Selector Menu
    /// </summary>
    public class AttachmentsPopMenu : BaseControl, IPostBackEventHandler
    {
        #region Constants and Fields

        /// <summary>
        ///   The _items.
        /// </summary>
        private readonly List<InternalAttachmentListItem> _items = new List<InternalAttachmentListItem>();

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
        /// The add client script item with Icon.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="clientScript">
        /// The client script.
        /// </param>
        /// <param name="albumImage">
        /// The icon.
        /// </param>
        public void AddClientScriptItem(
            [NotNull] string description,
            [NotNull] string clientScript,
            [NotNull] string albumImage)
        {
            this._items.Add(new InternalAttachmentListItem(description, null, clientScript, albumImage));
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
        /// The remove post back item.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public void RemovePostBackItem([NotNull] string argument)
        {
            foreach (InternalAttachmentListItem item in this._items.Where(item => item.PostBackArgument == argument))
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
            if (!this.Visible)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendFormat(
                @"<div class=""yafpopupmenu AttachmentListMenu"" id=""{0}"" style=""position:absolute;z-index:100;left:0;top:0;display:none;"">",
                this.ClientID);

            sb.Append("<div id=\"AttachmentsListBox\" class=\"content\">");
            sb.Append("<div id=\"AttachmentsListPager\"></div>");
            sb.Append("<br style=\"clear:both;\" />");
            sb.Append("<div id=\"AttachmentsPagerResult\">");
            sb.AppendFormat(
                "<p style=\"text-align:center\"><span>{1}</span><br /><img title=\"{1}\" src=\"{0}\" alt=\"{1}\" /></p>",
                YafForumInfo.GetURLToContent("images/loader.gif"),
                this.Get<ILocalization>().GetText("COMMON", "LOADING"));

            sb.Append("</div>");

            sb.Append("<div id=\"AttachmentsPagerHidden\" style=\"display:none;\">");

            sb.Append("<div class=\"result\">");
            sb.AppendFormat("<strong>{0}</strong>", this.Get<ILocalization>().GetText("ATTACHMENTS", "CURRENT_UPLOADS"));
            sb.AppendFormat("<ul class=\"AttachmentList\">");

            var rowPanel = 0;

            for (var i = 0; i < this._items.Count; i++)
            {
                var thisItem = this._items[i];

                if (rowPanel == 5 && i < this._items.Count)
                {
                    sb.Append("</ul>");
                    sb.AppendFormat(
                        "<span class=\"UploadNewFileLine\"><a class=\"OpenUploadDialog yaflittlebutton\" onclick=\"jQuery('.UploadDialog').dialog('open');\"><span>{0}</span></a></span>",
                        this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_NEW"));
                    sb.Append("</div>");
                    sb.AppendFormat(
                        "<strong>{0}</strong>",
                        this.Get<ILocalization>().GetText("ATTACHMENTS", "CURRENT_UPLOADS"));
                    sb.Append("<div class=\"result\">");

                    sb.Append("<ul class=\"AttachmentList\">");

                    rowPanel = 0;
                }

                rowPanel++;

                var iconImage = string.Empty;

                var onClick = thisItem.ClientScript.IsSet()
                                  ? thisItem.ClientScript.Replace(
                                      "{postbackcode}",
                                      this.Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument))
                                  : this.Page.ClientScript.GetPostBackClientHyperlink(this, thisItem.PostBackArgument);

                if (thisItem.File.IsSet())
                {
                    iconImage =
                        @"<img class=""popupitemIcon"" src=""{0}"" alt=""{1}"" title=""{1}"" /><span>{1}</span>"
                            .FormatWith(thisItem.File, thisItem.FileName);
                }

                sb.AppendFormat(
                    @"<li class=""popupitem AttachmentItem"" onmouseover=""mouseHover(this,true)"" onmouseout=""mouseHover(this,false)"" onclick=""{2}"" style=""white-space:nowrap"" title=""{1}""{3}>{0}</li>",
                    iconImage,
                    thisItem.FileName,
                    onClick,
                    thisItem.FileName.IsImageName() ? " data-url=\"{0}\"".FormatWith(thisItem.File) : string.Empty);
            }

            sb.Append("</ul>");
            sb.AppendFormat(
                "<span class=\"UploadNewFileLine\"><a class=\"OpenUploadDialog yaflittlebutton\" onclick=\"jQuery('.UploadDialog').dialog('open');\"><span>{0}</span></a></span>",
                this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_NEW"));
            sb.Append("</div>");

            sb.AppendFormat("</ul></div></div></div>");

            writer.WriteLine(sb.ToString());

            base.Render(writer);
        }

        #endregion
    }

    /// <summary>
    /// The internal pop menu item.
    /// </summary>
    public class InternalAttachmentListItem
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalAttachmentListItem"/> class.
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
        /// <param name="albumImage">
        /// The icon.
        /// </param>
        public InternalAttachmentListItem(
            [NotNull] string description,
            [NotNull] string postbackArgument,
            [NotNull] string clientScript,
            [NotNull] string albumImage)
        {
            this.FileName = description;
            this.PostBackArgument = postbackArgument;
            this.ClientScript = clientScript;
            this.File = albumImage;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Icon.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        ///   Gets or sets ClientScript.
        /// </summary>
        public string ClientScript { get; set; }

        /// <summary>
        ///   Gets or sets Description.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///   Gets or sets PostBackArgument.
        /// </summary>
        public string PostBackArgument { get; set; }

        #endregion
    }
}