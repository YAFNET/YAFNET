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
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Controls;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

    #endregion

  /// <summary>
  /// Topic Status Admin Edit Page.
  /// </summary>
  public partial class topicstatus : AdminPage
  {
    #region Methods

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        ((ThemeButton)sender).Attributes["onclick"] =
            "return (confirm('{0}'));"
                .FormatWith(this.GetText("ADMIN_TOPICSTATUS", "CONFIRM_DELETE"));
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void addLoad(object sender, EventArgs e)
    {
        var add = (Button)sender;
        add.Text = this.GetText("ADMIN_TOPICSTATUS", "ADD");
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void exportLoad(object sender, EventArgs e)
    {
        var export = (Button)sender;
        export.Text = this.GetText("ADMIN_TOPICSTATUS", "EXPORT");
    }

    /// <summary>
    /// Add Localized Text to Button
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void importLoad(object sender, EventArgs e)
    {
        var import = (Button)sender;
        import.Text = this.GetText("ADMIN_TOPICSTATUS", "IMPORT");
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      this.list.ItemCommand += this.list_ItemCommand;

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
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddRoot();
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_TOPICSTATUS", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"),
              this.GetText("ADMIN_TOPICSTATUS", "TITLE"));

        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.list.DataSource = LegacyDb.TopicStatus_List(this.PageContext.PageBoardID);
      this.DataBind();
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void list_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                YafBuildLink.Redirect(ForumPages.admin_topicstatus_edit);
                break;
            case "edit":
                YafBuildLink.Redirect(ForumPages.admin_topicstatus_edit, "i={0}", e.CommandArgument);
                break;
            case "delete":
                LegacyDb.TopicStatus_Delete(e.CommandArgument);
                this.BindData();
                break;
            case "export":
                {
                    // export this list as XML...
                    var topicStatusList = LegacyDb.TopicStatus_List(this.PageContext.PageBoardID);
                    topicStatusList.DataSet.DataSetName = "YafTopicStatusList";
                    topicStatusList.TableName = "YafTopicStatus";
                    topicStatusList.Columns.Remove("TopicStatusID");
                    topicStatusList.Columns.Remove("BoardID");

                    this.Response.ContentType = "text/xml";
                    this.Response.AppendHeader("Content-Disposition", "attachment; filename=YafTopicStatusList.xml");
                    topicStatusList.DataSet.WriteXml(this.Response.OutputStream);
                    this.Response.End();
                }

                break;
            case "import":
                YafBuildLink.Redirect(ForumPages.admin_topicstatus_import);
                break;
        }
    }

      #endregion
  }
}