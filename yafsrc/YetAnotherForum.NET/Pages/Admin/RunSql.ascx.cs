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

namespace YAF.Pages.Admin;

using YAF.Core.Data;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;
using YAF.Web.Editors;

/// <summary>
/// The run SQL Query Page.
/// </summary>
public partial class RunSql : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RunSql"/> class. 
    /// </summary>
    public RunSql()
        : base("ADMIN_RUNSQL", ForumPages.Admin_RunSql)
    {
    }

    /// <summary>
    ///   The editor.
    /// </summary>
    private ForumEditor editor;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.RunQuery.ClientID));

        if (this.IsPostBack)
        {
            return;
        }

        this.BindData();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit([NotNull] EventArgs e)
    {
        this.editor = new CKEditorBBCodeEditorSql
                          {
                              UserCanUpload = false,
                              MaxCharacters = int.MaxValue
                          };

        this.EditorLine.Controls.Add(this.editor);

        base.OnInit(e);
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_ADMIN", "Administration"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RUNSQL", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Runs the query click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void RunQueryClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.txtResult.Text = string.Empty;
        this.ResultHolder.Visible = true;

        this.txtResult.Text = this.Get<IDbAccess>().RunSQL(
            CommandTextHelpers.GetCommandTextReplaced(this.editor.Text.Trim()),
            Configuration.Config.SqlCommandTimeout);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.DataBind();
    }
}