/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for ranks.
  /// </summary>
  public partial class nntpretrieve : AdminPage
  {
    #region Methods

    /// <summary>
    /// The last message no.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The last message no.
    /// </returns>
    protected string LastMessageNo([NotNull] object _o)
    {
      var row = (DataRowView)_o;
      return "{0:N0}".FormatWith(row["LastMessageNo"]);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.IsPostBack)
      {
        return;
      }

      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(
        this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
      this.PageLinks.AddLink(this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
        this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"));

      this.Retrieve.Text = this.GetText("ADMIN_NNTPRETRIEVE", "RETRIEVE");

      this.BindData();
    }

    /// <summary>
    /// The retrieve_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Retrieve_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      int nSeconds = int.Parse(this.Seconds.Text);
      if (nSeconds < 1)
      {
        nSeconds = 1;
      }

      int nArticleCount = this.Get<INewsreader>().ReadArticles(
        this.PageContext.PageBoardID, 10, nSeconds, this.PageContext.BoardSettings.CreateNntpUsers);

      this.PageContext.AddLoadMessage(
        this.GetText("ADMIN_NNTPRETRIEVE", "Retrieved").FormatWith(nArticleCount, (double)nArticleCount / nSeconds));
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = LegacyDb.nntpforum_list(this.PageContext.PageBoardID, 10, null, true);
      this.DataBind();
    }

    #endregion
  }
}