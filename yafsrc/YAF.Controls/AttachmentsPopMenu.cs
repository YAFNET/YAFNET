/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
        #region Events

        /// <summary>
        ///   The item click.
        /// </summary>
        public event PopMenu.PopEventHandler ItemClick;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Control.
        /// </summary>
        public string Control { get; set; } = string.Empty;

        /// <summary>
        ///   Gets ControlOnClick.
        /// </summary>
        public string ControlOnClick => "yaf_popit('{0}')".FormatWith(this.ClientID);

        /// <summary>
        ///   Gets ControlOnMouseOver.
        /// </summary>
        public string ControlOnMouseOver => "yaf_mouseover('{0}')".FormatWith(this.ClientID);

        #endregion

        #region Public Methods

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void Attach([NotNull] WebControl control)
        {
            control.Attributes["onclick"] = this.ControlOnClick;
            control.Attributes["onmouseover"] = this.ControlOnMouseOver;
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
            this.ItemClick?.Invoke(this, new PopEventArgs(eventArgument));
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
                @"<div class=""AttachmentListMenu dropdown-item"" id=""{0}"">",
                this.ClientID);

            sb.Append("<div id=\"AttachmentsListBox\" class=\"content\">");

            sb.Append("<div id=\"AttachmentsListPager\"></div>");
            sb.Append("<div id=\"PostAttachmentLoader\">");
            sb.AppendFormat(
                "<p style=\"text-align:center\"><span>{1}</span><br /><img title=\"{1}\" src=\"{0}\" alt=\"{1}\" /></p>",
                YafForumInfo.GetURLToContent("images/loader.gif"),
                this.Get<ILocalization>().GetText("COMMON", "LOADING"));
            sb.Append("</div>");
            sb.Append("<div id=\"AttachmentsListBox\" class=\"content\">");
            sb.AppendFormat(
                "<div id=\"PostAttachmentListPlaceholder\" data-url=\"{0}\" data-userid=\"{1}\" data-notext=\"{2}\" style=\"clear: both;\">",
                YafForumInfo.ForumClientFileRoot,
                YafContext.Current.PageUserID,
                this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"));
            sb.Append("<ul class=\"AttachmentList\">");
            sb.Append("</ul>");
            sb.Append("</div>");
            sb.Append("<div class=\"m-x-auto OpenUploadDialog\">");
            sb.AppendFormat(
                "<button type=\"button\" class=\"btn btn-primary btn-sm\" data-toggle=\"modal\" data-target=\".UploadDialog\">{0}</button>",
                this.Get<ILocalization>().GetText("ATTACHMENTS", "UPLOAD_NEW"));
            sb.Append("</div>");

            sb.Append("</div>");
            sb.AppendFormat("</div>");

            writer.WriteLine(sb.ToString());

            base.Render(writer);
        }

        #endregion
    }
}