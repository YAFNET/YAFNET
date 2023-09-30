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

namespace YAF.Pages.Admin;

using YAF.Types.Models;

/// <summary>
/// The Admin Retrieve NNTP Articles Page
/// </summary>
public partial class NntpRetrieve : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NntpRetrieve"/> class. 
    /// </summary>
    public NntpRetrieve()
        : base("ADMIN_NNTPRETRIEVE", ForumPages.Admin_NntpRetrieve)
    {
    }

    /// <summary>
    /// Gets the last message number
    /// </summary>
    /// <param name="forum">
    /// The forum.
    /// </param>
    /// <returns>
    /// The last message no.
    /// </returns>
    protected string LastMessageNo(object forum)
    {
        var row = (Tuple<NntpForum, NntpServer, Forum>)forum;
        return $"{row.Item1.LastMessageNo:N0}";
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Retrieves the click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void RetrieveClick(object sender, EventArgs e)
    {
        var seconds = int.Parse(this.Seconds.Text);
        if (seconds < 1)
        {
            seconds = 1;
        }

        var articleCount = this.Get<INewsreader>()
            .ReadArticles(
                this.PageBoardContext.PageBoardID,
                10,
                seconds,
                this.PageBoardContext.BoardSettings.CreateNntpUsers);

        this.PageBoardContext.Notify(
            string
                .Format(this.GetText("ADMIN_NNTPRETRIEVE", "Retrieved"), articleCount, (double)articleCount / seconds),
            MessageTypes.success);

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.List.DataSource = this.GetRepository<NntpForum>().NntpForumList(this.PageBoardContext.PageBoardID, true)
            .Where(n => (n.Item1.LastUpdate - DateTime.UtcNow).Minutes > 10);

        this.DataBind();

        if (this.List.Items.Count == 0)
        {
            this.RetrievePanel.Visible = false;
            this.Footer.Visible = false;
        }
    }
}