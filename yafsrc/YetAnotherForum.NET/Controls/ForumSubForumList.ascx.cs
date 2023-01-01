/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Controls;

using System.Collections;

/// <summary>
/// The forum sub forum list.
/// </summary>
public partial class ForumSubForumList : BaseUserControl
{
    /// <summary>
    ///   Sets DataSource.
    /// </summary>
    public IEnumerable DataSource
    {
        set => this.SubforumList.DataSource = value;
    }

    /// <summary>
    /// Provides the "Forum Link Text" for the ForumList control.
    ///   Automatically disables the link if the current user doesn't
    ///   have proper permissions.
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// Forum link text
    /// </returns>
    public string GetForumLink([NotNull] ForumRead forum)
    {
        // get the Forum Description
        var output = forum.Forum;

        output = forum.ReadAccess
                     ? $"<a class=\"card-link small\" href=\"{this.Get<LinkBuilder>().GetForumLink(forum.ForumID, output)}\" title=\"{this.GetText("COMMON", "VIEW_FORUM")}\" >{output}</a>"
                     : $"{output} {this.GetText("NO_FORUM_ACCESS")}";

        return output;
    }

    /// <summary>
    /// Handles the ItemCreated event of the SubForumList control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
    protected void SubForumList_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
        if (!e.Item.ItemType.Equals(ListItemType.Footer))
        {
            return;
        }

        if (this.SubforumList.Items.Count >=
            this.PageBoardContext.BoardSettings.SubForumsInForumList)
        {
            e.Item.FindControl("CutOff").Visible = true;
        }
    }

    protected void SubForumList_OnPreRender(object sender, EventArgs e)
    {
        if (this.SubforumList.Items.Count == 0)
        {
            this.Visible = false;
        }
    }

    /// <summary>
    /// Gets the Posts string
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Returns the Posts string
    /// </returns>
    protected string Posts([NotNull] ForumRead item)
    {
        return item.RemoteURL.IsNotSet() ? $"{item.Posts:N0}" : "-";
    }

    /// <summary>
    /// Gets the Topics string
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Returns the Topics string
    /// </returns>
    protected string Topics([NotNull] ForumRead item)
    {
        return item.RemoteURL.IsNotSet() ? $"{item.Topics:N0}" : "-";
    }
}