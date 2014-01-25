/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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

  using YAF.Classes;
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
  /// Summary description for bannedip_edit.
  /// </summary>
  public partial class replacewords_edit : AdminPage
  {
    #region Methods

      /// <summary>
      /// Check if Valid Expression
      /// </summary>
      /// <param name="newExpression">
      /// The new Expression to Check.
      /// </param>
      /// <returns>
      /// Returns if Valid Expression
      /// </returns>
      protected bool IsValidWordExpression([NotNull] string newExpression)
      {
          if (newExpression.Equals("*"))
          {
              this.PageContext.AddLoadMessage(this.GetText("ADMIN_REPLACEWORDS_EDIT", "MSG_REGEX_BAD"));
              return false;
          }

          return true;
      }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.save.Click += this.add_Click;
      this.cancel.Click += this.cancel_Click;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      this.InitializeComponent();
      base.OnInit(e);
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
      string strAddEdit = (this.Request.QueryString.GetFirstOrDefault("i") == null) ? "Add" : "Edit";

      if (!this.IsPostBack)
      {
          this.PageLinks.AddRoot();
          this.PageLinks.AddLink(
              this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
          this.PageLinks.AddLink(this.GetText("ADMIN_REPLACEWORDS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_replacewords));
          this.PageLinks.AddLink(this.GetText("ADMIN_REPLACEWORDS_EDIT", "TITLE"), string.Empty);

          this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_REPLACEWORDS", "TITLE"),
              this.GetText("ADMIN_REPLACEWORDS_EDIT", "TITLE"));

          this.save.Text = this.GetText("COMMON", "SAVE");
          this.cancel.Text = this.GetText("COMMON", "CANCEL");

          this.BindData();
      }

        this.badword.Attributes.Add("style", "width:250px");
      this.goodword.Attributes.Add("style", "width:250px");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      int id;

        if (this.Request.QueryString.GetFirstOrDefault("i") == null ||
            !int.TryParse(this.Request.QueryString.GetFirstOrDefault("i"), out id))
        {
            return;
        }

        DataRow row = LegacyDb.replace_words_list(this.PageContext.PageBoardID, id).Rows[0];
        this.badword.Text = (string)row["badword"];
        this.goodword.Text = (string)row["goodword"];
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// The add_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void add_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.IsValidWordExpression(badword.Text.Trim()))
        {
            this.BindData();
        }
        else
        {
            LegacyDb.replace_words_save(
                this.PageContext.PageBoardID,
                this.Request.QueryString.GetFirstOrDefault("i"),
                this.badword.Text,
                this.goodword.Text);

            this.Get<IDataCache>().Remove(Constants.Cache.ReplaceWords);
            YafBuildLink.Redirect(ForumPages.admin_replacewords);
        }
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_replacewords);
    }

    #endregion
  }
}