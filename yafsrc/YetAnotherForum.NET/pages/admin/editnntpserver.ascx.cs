/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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
  /// Summary description for editgroup.
  /// </summary>
  public partial class editnntpserver : AdminPage
  {
    #region Methods

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_nntpservers);
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
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_NNTPSERVERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_nntpservers));
        this.PageLinks.AddLink(this.GetText("ADMIN_EDITNNTPSERVER", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
             this.GetText("ADMIN_ADMIN", "Administration"),
             this.GetText("ADMIN_NNTPSERVERS", "TITLE"),
             this.GetText("ADMIN_EDITNNTPSERVER", "TITLE"));

        this.Save.Text = this.GetText("COMMON", "SAVE");
        this.Cancel.Text = this.GetText("COMMON", "CANCEL");

        this.BindData();
        if (this.Request.QueryString.GetFirstOrDefault("s") != null)
        {
            using (
                DataTable dt = LegacyDb.nntpserver_list(
                    this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("s")))
            {
                DataRow row = dt.Rows[0];
                this.Name.Text = row["Name"].ToString();
                this.Address.Text = row["Address"].ToString();
                this.Port.Text = row["Port"].ToString();
                this.UserName.Text = row["UserName"].ToString();
                this.UserPass.Text = row["UserPass"].ToString();
            }
        }
        else
        {
            this.Port.Text = "119";
        }
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.Name.Text.Trim().Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITNNTPSERVER", "MSG_SERVER_NAME"));
        return;
      }

      if (this.Address.Text.Trim().Length == 0)
      {
        this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITNNTPSERVER", "MSG_SERVER_ADR"));
        return;
      }

      object nntpServerID = null;
      if (this.Request.QueryString.GetFirstOrDefault("s") != null)
      {
        nntpServerID = this.Request.QueryString.GetFirstOrDefault("s");
      }

      LegacyDb.nntpserver_save(
        nntpServerID, 
        this.PageContext.PageBoardID, 
        this.Name.Text, 
        this.Address.Text, 
        this.Port.Text.Length > 0 ? this.Port.Text : null, 
        this.UserName.Text.Length > 0 ? this.UserName.Text : null, 
        this.UserPass.Text.Length > 0 ? this.UserPass.Text : null);
      YafBuildLink.Redirect(ForumPages.admin_nntpservers);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.DataBind();
    }

    #endregion
  }
}