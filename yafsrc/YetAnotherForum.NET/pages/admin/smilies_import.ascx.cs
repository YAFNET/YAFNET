/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using System.IO;
    using System.Text.RegularExpressions;

    using YAF.Classes;
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

    #endregion

    /// <summary>
    /// Summary description for smilies_import.
    /// </summary>
    public partial class smilies_import : AdminPage
    {
        #region Methods

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.import.Click += this.import_Click;
            this.cancel.Click += this.cancel_Click;

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

            this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
            this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
            this.PageLinks.AddLink(this.GetText("ADMIN_SMILIES", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_smilies));
            this.PageLinks.AddLink(this.GetText("ADMIN_SMILIES_IMPORT", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                  this.GetText("ADMIN_ADMIN", "Administration"),
                  this.GetText("ADMIN_SMILIES", "TITLE"),
                  this.GetText("ADMIN_SMILIES_IMPORT", "TITLE"));

            this.import.Text = "<i class=\"fa fa-upload fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("ADMIN_SMILIES_IMPORT", "IMPORT"));
            this.cancel.Text = "<i class=\"fa fa-remove fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("CANCEL"));

            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            using (var dt = new DataTable("Files"))
            {
                dt.Columns.Add("FileID", typeof(long));
                dt.Columns.Add("FileName", typeof(string));
                DataRow dr = dt.NewRow();
                dr["FileID"] = 0;
                dr["FileName"] = this.GetText("ADMIN_SMILIES_IMPORT", "SELECT_FILE");
                dt.Rows.Add(dr);

                var dir =
                  new DirectoryInfo(
                    this.Request.MapPath(
                      "{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Emoticons)));
                FileInfo[] files = dir.GetFiles("*.pak");
                long nFileID = 1;
                foreach (FileInfo file in files)
                {
                    dr = dt.NewRow();
                    dr["FileID"] = nFileID++;
                    dr["FileName"] = file.Name;
                    dt.Rows.Add(dr);
                }

                this.File.DataSource = dt;
                this.File.DataValueField = "FileID";
                this.File.DataTextField = "FileName";
            }

            this.DataBind();
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
            YafBuildLink.Redirect(ForumPages.admin_smilies);
        }

        /// <summary>
        /// The import_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void import_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (long.Parse(this.File.SelectedValue) < 1)
            {
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_SMILIES_IMPORT", "ERROR_SELECT_FILE"));
                return;
            }

            string fileName =
              this.Request.MapPath(
                "{0}{1}/{2}".FormatWith(
                  YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, this.File.SelectedItem.Text));
            string split = Regex.Escape("=+:");

            using (var file = new StreamReader(fileName))
            {
                int sortOrder = 1;

                // Delete existing smilies?
                if (this.DeleteExisting.Checked)
                {
                    this.GetRepository<Smiley>().DeleteAll();
                }
                else
                {
                    // Get max value of SortOrder
                    using (DataView dv = this.GetRepository<Smiley>().ListUnique().DefaultView)
                    {
                        dv.Sort = "SortOrder desc";
                        if (dv.Count > 0)
                        {
                            DataRowView dr = dv[0];
                            if (dr != null)
                            {
                                object o = dr["SortOrder"];
                                if (int.TryParse(o.ToString(), out sortOrder))
                                {
                                    sortOrder++;
                                }
                            }
                        }
                    }
                }

                do
                {
                    string line = file.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    string[] lineSplit = Regex.Split(line, split, RegexOptions.None);

                    if (lineSplit.Length != 3)
                    {
                        continue;
                    }

                    this.GetRepository<Smiley>().Save(null, lineSplit[2], lineSplit[0], lineSplit[1], (byte)sortOrder, 0);
                    sortOrder++;
                }
                while (true);

                file.Close();
            }

            YafBuildLink.Redirect(ForumPages.admin_smilies);
        }

        #endregion
    }
}