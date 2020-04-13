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
    using System.Data;
    using System.Linq;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// Provides an Online/Offline/Suspended status for a YAF User
    /// </summary>
    public class OnlineStatusIcon : BaseControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UserLink"/> is suspended.
        /// </summary>
        /// <value>
        ///   <c>true</c> if suspended; otherwise, <c>false</c>.
        /// </value>
        [NotNull]
        public System.DateTime? Suspended { get; set; }

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
            if (!this.Visible)
            {
                return;
            }

            var activeUsers = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.UsersOnlineStatus,
                () => this.Get<DataBroker>().GetActiveList(
                    false,
                    BoardContext.Current.BoardSettings.ShowCrawlersInActiveList),
                TimeSpan.FromMilliseconds(BoardContext.Current.BoardSettings.OnlineStatusCacheTimeout));

            output.BeginRender();
            output.WriteBeginTag(HtmlTextWriterTag.Span.ToString());
            output.WriteAttribute(HtmlTextWriterAttribute.Id.ToString(), this.ClientID);

            if (this.Suspended.HasValue)
            {
                // suspended
                output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-warning");
                output.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetTextFormatted("USERSUSPENDED", this.Suspended.Value));
                output.WriteAttribute("data-toggle", "tooltip");
            }
            else
            {
                if (activeUsers.AsEnumerable()
                    .Any(x => x.Field<int>("UserId") == this.UserId && !x.Field<bool>("IsHidden")))
                {
                    // online
                    output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-success");
                    output.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetText("USERONLINESTATUS"));
                    output.WriteAttribute("data-toggle", "tooltip");
                }
                else
                {
                    // offline
                    output.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-danger");
                    output.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetText("USEROFFLINESTATUS"));
                    output.WriteAttribute("data-toggle", "tooltip");
                }
            }

            output.Write(HtmlTextWriter.TagRightChar);

            output.Write(@"<i class=""fas fa-user-circle"" style=""font-size: 1.5em""></i>");

            // render the optional controls (if any)
            base.Render(output);
            output.WriteEndTag(HtmlTextWriterTag.Span.ToString());
            output.EndRender();
        }

        #endregion
    }
}