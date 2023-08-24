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
namespace YAF.Web.Controls;

using System.Text;
using System.Web.UI;

using YAF.Core.BaseControls;
using YAF.Types;
using YAF.Types.Interfaces;
using YAF.Web.EventsArgs;

/// <summary>
/// Albums Image List Selector Menu
/// </summary>
public class AlbumListPopMenu : BaseControl, IPostBackEventHandler
{
    /// <summary>
    ///   The item click.
    /// </summary>
    public event PopMenu.PopEventHandler ItemClick;

    /// <summary>
    ///   Gets or sets Control.
    /// </summary>
    public string Control { get; set; } = string.Empty;

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
            @"<div class=""AlbumsListMenu dropdown-item"" id=""{0}"">",
            this.ClientID);

        sb.Append("<div id=\"AlbumsListBox\" class=\"content\">");

        sb.Append("<div id=\"AlbumsListPager\"></div>");
        sb.Append("<div id=\"PostAlbumsLoader\">");
        sb.AppendFormat(
            "<p style=\"text-align:center\"><span>{0}</span><br /><div class=\"fa-3x\"><i class=\"fas fa-spinner fa-pulse\"></i></div></p>",
            this.Get<ILocalization>().GetText("COMMON", "LOADING"));
        sb.Append("</div>");
        sb.Append("<div id=\"AlbumsListBox\" class=\"content\">");
        sb.AppendFormat(
            "<div id=\"PostAlbumsListPlaceholder\" data-url=\"{0}\" data-userid=\"{1}\" data-notext=\"{2}\" style=\"clear: both;\">",
            BoardInfo.ForumClientFileRoot,
            BoardContext.Current.PageUserID,
            this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"));
        sb.Append("<ul class=\"AlbumsList list-group\">");
        sb.Append("</ul>");
        sb.Append("</div>");

        sb.Append("</div>");
        sb.AppendFormat("</div>");

        writer.WriteLine(sb.ToString());

        base.Render(writer);
    }
}