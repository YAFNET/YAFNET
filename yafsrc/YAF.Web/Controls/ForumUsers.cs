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

/// <summary>
/// Displays the forum users.
/// </summary>
public class ForumUsers : BaseControl
{
    /// <summary>
    ///   The _active users.
    /// </summary>
    private readonly ActiveUsers activeUsers = new();

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumUsers" /> class.
    /// </summary>
    public ForumUsers()
    {
        this.activeUsers.ID = this.GetUniqueID("ActiveUsers");
        this.Load += this.ForumUsersLoad;
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
        // Ederon : 07/14/2007
        if (!this.PageBoardContext.BoardSettings.ShowBrowsingUsers)
        {
            return;
        }

        var topicId = this.PageBoardContext.PageTopicID > 0;

        writer.WriteLine(@"<div class=""card"">");

        writer.WriteLine(@"<div class=""card-header"">");

        writer.WriteLine(this.GetText(topicId ? "TOPICBROWSERS" : "FORUMUSERS"));

        writer.WriteLine("</div>");

        writer.WriteLine(@"<div class=""card-body"">");

        base.Render(writer);

        writer.WriteLine("</div></div>");
    }

    /// <summary>
    /// The forum users_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumUsersLoad([NotNull] object sender, [NotNull] EventArgs e)
    {
        var inTopic = this.PageBoardContext.PageTopicID > 0;

        this.activeUsers.ActiveUsersList ??= inTopic
                                                 ? this.GetRepository<Active>().ListTopic(this.PageBoardContext.PageTopicID)
                                                 : this.GetRepository<Active>().ListForum(this.PageBoardContext.PageForumID);

        // add it...
        this.Controls.Add(this.activeUsers);
    }
}