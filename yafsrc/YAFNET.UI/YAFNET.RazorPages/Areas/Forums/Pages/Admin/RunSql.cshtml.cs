/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using YAF.Core.Data;
using YAF.Core.Extensions;
using YAF.Types.Extensions;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;

/// <summary>
/// The run SQL Query Page.
/// </summary>
public class RunSqlModel : AdminPage
{
    [BindProperty]
    public string Editor { get; set; }

    [BindProperty]
    public string Result { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RunSqlModel"/> class.
    /// </summary>
    public RunSqlModel()
        : base("ADMIN_RUNSQL", ForumPages.Admin_RunSql)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_ADMIN", "Administration"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Admin));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RUNSQL", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Runs the query click.
    /// </summary>
    public void OnPostRunQuery()
    {
        this.Result = string.Empty;

        if (this.Editor.IsSet())
        {
            this.Result = this.Get<IDbAccess>().RunSql(
                CommandTextHelpers.GetCommandTextReplaced(this.Editor.Trim()),
                Configuration.Config.SqlCommandTimeout);
        }
    }
}