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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The bbcode.
    /// </summary>
    public partial class bbcode : AdminPage
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
            ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_BBCODE", "CONFIRM_DELETE"));
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
            add.Text = this.GetText("ADMIN_BBCODE", "ADD");
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
            export.Text = this.GetText("ADMIN_BBCODE", "EXPORT");
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
            import.Text = this.GetText("ADMIN_BBCODE", "IMPORT");
        }

        /// <summary>
        /// The get selected bb code i ds.
        /// </summary>
        /// <returns>
        /// The Id of the BB Code
        /// </returns>
        [NotNull]
        protected List<int> GetSelectedBBCodeIDs()
        {
            // get checked items....
            return (from RepeaterItem item in this.bbCodeList.Items
                    let sel = (CheckBox)item.FindControl("chkSelected")
                    where sel.Checked
                    select (HiddenField)item.FindControl("hiddenBBCodeID") into hiddenId
                    select hiddenId.Value.ToType<int>()).ToList();
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
            this.PageLinks.AddLink(this.GetText("ADMIN_BBCODE", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                  this.GetText("ADMIN_ADMIN", "Administration"),
                  this.GetText("ADMIN_BBCODE", "TITLE"));

            this.BindData();
        }

        /// <summary>
        /// The bb code list_ item command.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void bbCodeList_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "add":
                    YafBuildLink.Redirect(ForumPages.admin_bbcode_edit);
                    break;
                case "edit":
                    YafBuildLink.Redirect(ForumPages.admin_bbcode_edit, "b={0}", e.CommandArgument);
                    break;
                case "delete":
                    this.GetRepository<BBCode>().DeleteByID(e.CommandArgument.ToType<int>());
                    this.Get<IDataCache>().Remove(Constants.Cache.CustomBBCode);
                    this.BindData();
                    break;
                case "export":
                    {
                        List<int> bbCodeIds = this.GetSelectedBBCodeIDs();

                        if (bbCodeIds.Count > 0)
                        {
                            // export this list as XML...
                            DataTable dtBBCode = this.GetRepository<BBCode>().List();

                            // remove all but required bbcodes...
                            foreach (DataRow row in
                                from DataRow row in dtBBCode.Rows
                                let id = row["BBCodeID"].ToType<int>()
                                where !bbCodeIds.Contains(id)
                                select row)
                            {
                                // remove from this table...
                                row.Delete();
                            }

                            // store delete changes...
                            dtBBCode.AcceptChanges();

                            // export...
                            dtBBCode.DataSet.DataSetName = "YafBBCodeList";
                            dtBBCode.TableName = "YafBBCode";
                            dtBBCode.Columns.Remove("BBCodeID");
                            dtBBCode.Columns.Remove("BoardID");

                            this.Response.ContentType = "text/xml";
                            this.Response.AppendHeader("Content-Disposition", "attachment; filename=YafBBCodeExport.xml");
                            dtBBCode.DataSet.WriteXml(this.Response.OutputStream);
                            this.Response.End();
                        }
                        else
                        {
                            this.PageContext.AddLoadMessage(this.GetText("ADMIN_BBCODE", "MSG_NOTHING_SELECTED"));
                        }
                    }

                    break;
                case "import":
                    YafBuildLink.Redirect(ForumPages.admin_bbcode_import);
                    break;
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.bbCodeList.DataSource = this.GetRepository<BBCode>().List();
            this.DataBind();
        }

        #endregion
    }
}