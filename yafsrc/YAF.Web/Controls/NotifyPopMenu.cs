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

    using System.Text;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.EventsArgs;

    #endregion

    /// <summary>
    /// Activity Notifications Menu
    /// </summary>
    public class NotifyPopMenu : BaseControl, IPostBackEventHandler
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

            sb.Append("<div class=\"dropdown-item px-0\">");

            sb.Append("<div id=\"AttachmentsListPager\"></div>");
            sb.Append("<div id=\"Loader\" class=\"px-2 mx-2\">");
            sb.AppendFormat(
                "<p style=\"text-align:center\"><span>{0}</span><br /><div class=\"fa-3x\"><i class=\"fas fa-spinner fa-pulse\"></i></div></p>",
                this.Get<ILocalization>().GetText("COMMON", "LOADING"));
            sb.Append("</div>");
        
            sb.AppendFormat(
                "<div id=\"NotifyListPlaceholder\" data-url=\"{0}\" data-userid=\"{1}\" style=\"clear: both;\">",
                BoardInfo.ForumClientFileRoot,
                BoardContext.Current.PageUserID);

            sb.Append("<ul class=\"list-group list-group-flush\">");
            sb.Append("</ul>"); 
            
            sb.Append("</div>");
            sb.AppendFormat("</div>");

            writer.WriteLine(sb.ToString());

            base.Render(writer);
        }

        #endregion
    }
}