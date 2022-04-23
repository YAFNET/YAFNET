/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System;
using System.Linq;
using System.Web.UI;

using YAF.Core.BaseControls;
using YAF.Core.Context;
using YAF.Core.Model;
using YAF.Types;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Models;

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
    public DateTime? Suspended { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        if (!this.Visible)
        {
            return;
        }

        var activeUsers = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.UsersOnlineStatus,
            () => this.GetRepository<Active>().List(
                false,
                this.PageBoardContext.BoardSettings.ShowCrawlersInActiveList,
                this.PageBoardContext.BoardSettings.ActiveListTime),
            TimeSpan.FromMilliseconds(BoardContext.Current.BoardSettings.OnlineStatusCacheTimeout));

        writer.BeginRender();
        writer.WriteBeginTag(HtmlTextWriterTag.Span.ToString());
        writer.WriteAttribute(HtmlTextWriterAttribute.Id.ToString(), this.ClientID);

        if (this.Suspended.HasValue)
        {
            // suspended
            writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-warning user-suspended me-1");
            writer.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetTextFormatted("USERSUSPENDED", this.Suspended.Value));
             
        }
        else
        {
            if (activeUsers.Any(x => x.UserID == this.UserId && !x.IsActiveExcluded))
            {
                // online
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-success user-online me-1");
                writer.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetText("USERONLINESTATUS"));
            }
            else
            {
                // offline
                writer.WriteAttribute(HtmlTextWriterAttribute.Class.ToString(), "align-middle text-danger user-offline me-1");
                writer.WriteAttribute(HtmlTextWriterAttribute.Title.ToString(), this.GetText("USEROFFLINESTATUS"));
            }
        }

        writer.Write(HtmlTextWriter.TagRightChar);

        writer.Write(@"<i class=""fas fa-user-circle"" style=""font-size: 1.5em""></i>");

        // render the optional controls (if any)
        base.Render(writer);
        writer.WriteEndTag(HtmlTextWriterTag.Span.ToString());
        writer.EndRender();
    }

    #endregion
}