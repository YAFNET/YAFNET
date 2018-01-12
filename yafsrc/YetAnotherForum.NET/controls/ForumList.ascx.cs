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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Forum List Control
    /// </summary>
    public partial class ForumList : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The Go to last post Image ToolTip.
        /// </summary>
        private string _altLastPost;

        /// <summary>
        /// The Data Source
        /// </summary>
        private IEnumerable _dataSource;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets Alt.
        /// </summary>
        [NotNull]
        public string AltLastPost
        {
            get
            {
                return string.IsNullOrEmpty(this._altLastPost) ? string.Empty : this._altLastPost;
            }

            set
            {
                this._altLastPost = value;
            }
        }

        /// <summary>
        ///   Gets or sets DataSource.
        /// </summary>
        public IEnumerable DataSource
        {
            get
            {
                return this._dataSource;
            }

            set
            {
                this._dataSource = value;
                DataRow[] arr;
                var t = this._dataSource.GetType();
                var arlist = new List<DataRow>();
                var subLIst = new List<DataRow>();
                var parents = new List<int>();
                if (t.Name == "DataRowCollection")
                {
                    arr = new DataRow[((DataRowCollection)this._dataSource).Count];
                    ((DataRowCollection)this._dataSource).CopyTo(arr, 0);

                    for (var i = 0; i < arr.Count(); i++)
                    {
                        // these are all subforums related to start page forums
                        if (!arr[i]["ParentID"].IsNullOrEmptyDBField())
                        {
                            if (this.SubDataSource == null)
                            {
                                this.SubDataSource = arr[i].Table.Clone();
                            }

                            var drow = this.SubDataSource.NewRow();
                            drow.ItemArray = arr[i].ItemArray;

                            parents.Add(drow["ForumID"].ToType<int>());

                            if (parents.Contains(drow["ParentID"].ToType<int>()))
                            {
                                this.SubDataSource.Rows.Add(drow);
                                subLIst.Add(drow);
                            }
                            else
                            {
                                arlist.Add(arr[i]);
                            }
                        }
                        else
                        {
                            arlist.Add(arr[i]);
                        }
                    }
                }
                else
                {
                    // (t.Name == "DataRow[]")
                    arr = (DataRow[])this._dataSource;
                    for (var i = 0; i < arr.Count(); i++)
                    {
                        if (!arr[i]["ParentID"].IsNullOrEmptyDBField())
                        {
                            if (this.SubDataSource == null)
                            {
                                this.SubDataSource = arr[i].Table.Clone();
                            }

                            var drow = this.SubDataSource.NewRow();
                            drow.ItemArray = arr[i].ItemArray;

                            this.SubDataSource.Rows.Add(drow);
                        }
                        else
                        {
                            arlist.Add(arr[i]);
                        }
                    }
                }

                if (this.SubDataSource != null)
                {
                    this.SubDataSource.AcceptChanges();
                }

                this._dataSource = arlist;

                this.ForumList1.DataSource = this._dataSource;
            }
        }

        /// <summary>
        /// Gets or sets the sub data source.
        /// </summary>
        /// <value>
        /// The sub data source.
        /// </value>
        private DataTable SubDataSource { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Provides the "Forum Link Text" for the ForumList control.
        ///   Automatically disables the link if the current user doesn't
        ///   have proper permissions.
        /// </summary>
        /// <param name="row">
        /// Current data row
        /// </param>
        /// <returns>
        /// Forum link text
        /// </returns>
        public string GetForumLink([NotNull] DataRow row)
        {
            var forumID = row["ForumID"].ToType<int>();

            // get the Forum Description
            var output = Convert.ToString(row["Forum"]);

            if (row["ReadAccess"].ToType<int>() > 0)
            {
                if (row["RemoteURL"] != DBNull.Value)
                {
                    output = "<a href=\"{0}\" title=\"{1}\" target=\"_blank\">{2}</a>".FormatWith(
                        (string)row["RemoteURL"],
                        this.GetText("COMMON", "VIEW_FORUM"),
                        this.Page.HtmlEncode(output));
                }
                else
                {
                    output = "<a href=\"{0}\" title=\"{1}\">{2}</a>".FormatWith(
                        YafBuildLink.GetLink(ForumPages.topics, "f={0}&name={1}", forumID, output),
                        this.GetText("COMMON", "VIEW_FORUM"),
                        this.Page.HtmlEncode(output));
                }
            }
            else
            {
                // no access to this forum
                output = "{0} {1}".FormatWith(output, this.GetText("NO_FORUM_ACCESS"));
            }

            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the ItemCreated event of the ForumList1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void ForumList1_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var row = (DataRow)e.Item.DataItem;
            var flags = new ForumFlags(row["Flags"]);

            var lastRead =
                this.Get<IReadTrackCurrentUser>()
                    .GetForumTopicRead(
                        forumId: row["ForumID"].ToType<int>(),
                        topicId: row["LastTopicID"].ToType<int>(),
                        forumReadOverride: row["LastForumAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime(),
                        topicReadOverride: row["LastTopicAccess"].ToType<DateTime?>() ?? DateTimeHelper.SqlDbMinTime());

            var lastPosted = row["LastPosted"].ToType<DateTime?>() ?? lastRead;

            if (string.IsNullOrEmpty(row["ImageUrl"].ToString()))
            {
                var forumIcon = e.Item.FindControl("ThemeForumIcon") as ThemeImage;
                if (forumIcon != null)
                {
                    forumIcon.ThemeTag = "FORUM";
                    forumIcon.LocalizedTitlePage = "ICONLEGEND";
                    forumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
                    forumIcon.Visible = true;

                    try
                    {
                        if (flags.IsLocked)
                        {
                            forumIcon.ThemeTag = "FORUM_LOCKED";
                            forumIcon.LocalizedTitlePage = "ICONLEGEND";
                            forumIcon.LocalizedTitleTag = "FORUM_LOCKED";
                        }
                        else if (lastPosted > lastRead && row["ReadAccess"].ToType<int>() > 0)
                        {
                            forumIcon.ThemeTag = "FORUM_NEW";
                            forumIcon.LocalizedTitlePage = "ICONLEGEND";
                            forumIcon.LocalizedTitleTag = "NEW_POSTS";
                        }
                        else
                        {
                            forumIcon.ThemeTag = "FORUM";
                            forumIcon.LocalizedTitlePage = "ICONLEGEND";
                            forumIcon.LocalizedTitleTag = "NO_NEW_POSTS";
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                var forumImage = e.Item.FindControl("ForumImage1") as HtmlImage;
                if (forumImage != null)
                {
                    forumImage.Src = "{0}{1}/{2}".FormatWith(
                        YafForumInfo.ForumServerFileRoot,
                        YafBoardFolders.Current.Forums,
                        row["ImageUrl"].ToString());

                    // TODO: vzrus: needs to be moved to css and converted to a more light control in the future.
                    // Highlight custom icon images and add tool tips to them. 
                    try
                    {
                        forumImage.Attributes.Clear();

                        if (flags.IsLocked)
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_locked");
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "FORUM_LOCKED"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "FORUM_LOCKED"));
                            forumImage.Attributes.Add(
                                "src",
                                "{0}{1}/{2}".FormatWith(
                                    YafForumInfo.ForumServerFileRoot,
                                    YafBoardFolders.Current.Forums,
                                    row["ImageUrl"].ToString()));
                        }
                        else if (lastPosted > lastRead)
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_newposts");
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "NEW_POSTS"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "NEW_POSTS"));
                            forumImage.Attributes.Add(
                                "src",
                                "{0}{1}/{2}".FormatWith(
                                    YafForumInfo.ForumServerFileRoot,
                                    YafBoardFolders.Current.Forums,
                                    row["ImageUrl"].ToString()));
                        }
                        else
                        {
                            forumImage.Attributes.Add("class", "forum_customimage_nonewposts");
                            forumImage.Attributes.Add(
                                "src",
                                "{0}{1}/{2}".FormatWith(
                                    YafForumInfo.ForumServerFileRoot,
                                    YafBoardFolders.Current.Forums,
                                    row["ImageUrl"].ToString()));
                            forumImage.Attributes.Add("alt", this.GetText("ICONLEGEND", "NO_NEW_POSTS"));
                            forumImage.Attributes.Add("title", this.GetText("ICONLEGEND", "NO_NEW_POSTS"));
                        }

                        forumImage.Visible = true;
                    }
                    catch
                    {
                    }

                    forumImage.Visible = true;
                }
            }

            if (!this.Get<YafBoardSettings>().ShowModeratorList)
            {
                return;
            }

            if (this.Get<YafBoardSettings>().ShowModeratorListAsColumn)
            {
                // hide moderator list...
                var moderatorColumn = e.Item.FindControl("ModeratorListTD") as HtmlTableCell;
                var modList = e.Item.FindControl("ModeratorList") as ForumModeratorList;

                if (modList == null)
                {
                    return;
                }

                var dra = row.GetChildRows("FK_Moderator_Forum");

                if (dra.GetLength(0) > 0)
                {
                    modList.DataSource = dra;
                    modList.Visible = true;
                    modList.DataBind();
                }

                // set them as visible...
                if (moderatorColumn != null)
                {
                    moderatorColumn.Visible = true;
                }
            }
            else
            {
                var moderatorSpan = e.Item.FindControl("ModListMob_Span") as HtmlGenericControl;
                var modList1 = e.Item.FindControl("ForumModeratorListMob") as ForumModeratorList;

                if (modList1 != null)
                {
                    var dra = row.GetChildRows("FK_Moderator_Forum");
                    if (dra.GetLength(0) > 0)
                    {
                        modList1.DataSource = dra;
                        modList1.Visible = true;
                        modList1.DataBind();

                        // set them as visible...
                        if (moderatorSpan != null)
                        {
                            moderatorSpan.Visible = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the moderated.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get moderated.
        /// </returns>
        protected bool GetModerated([NotNull] object o)
        {
            return ((DataRow)o)["Flags"].BinaryAnd(ForumFlags.Flags.IsModerated);
        }

        // Suppress rendering of footer if there is one or more 

        /// <summary>
        /// Gets the moderator link.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        /// The get moderator link.
        /// </returns>
        protected string GetModeratorLink([NotNull] DataRow row)
        {
            string output;

            if (row["IsGroup"].ToType<int>() == 0)
            {
                output =
                    "<a href=\"{0}\">{1}</a>".FormatWith(
                        YafBuildLink.GetLink(
                            ForumPages.profile,
                            "u={0}&name={1}",
                            row["ModeratorID"],
                            row["ModeratorName"]),
                        row["ModeratorName"]);
            }
            else
            {
                // TODO : group link should point to group info page (yet unavailable)
                /*output = String.Format(
                        "<strong><a href=\"{0}\">{1}</a></strong>",
                        YafBuildLink.GetLink(ForumPages.forum, "g={0}", row["ModeratorID"]),
                        row["ModeratorName"]
                        );*/
                output = "<strong>{0}</strong>".FormatWith(row["ModeratorName"]);
            }

            return output;
        }

        /// <summary>
        /// Gets the moderators footer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <returns>
        /// The get moderators footer.
        /// </returns>
        [NotNull]
        protected string GetModeratorsFooter([NotNull] Repeater sender)
        {
            if (sender.DataSource is DataRow[] && ((DataRow[])sender.DataSource).Length < 1)
            {
                return "-";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the sub Forums.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>Returns the Sub Forums</returns>
        protected IEnumerable GetSubforums([NotNull] DataRow row)
        {
            if (!this.HasSubforums(row))
            {
                return null;
            }

            var arlist = new ArrayList();

            foreach (
                var subrow in
                    this.SubDataSource.Rows.Cast<DataRow>()
                        .Where(subrow => row["ForumID"].ToType<int>() == subrow["ParentID"].ToType<int>())
                        .Where(subrow => arlist.Count < this.Get<YafBoardSettings>().SubForumsInForumList))
            {
                arlist.Add(subrow);
            }

            this.SubDataSource.AcceptChanges();

            return arlist;
        }

        /// <summary>
        /// Gets the viewing.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        /// The get viewing.
        /// </returns>
        protected string GetViewing([NotNull] object o)
        {
            var row = (DataRow)o;
            var nViewing = row["Viewing"].ToType<int>();

            return nViewing > 0 ? "&nbsp;{0}".FormatWith(this.GetTextFormatted("VIEWING", nViewing)) : string.Empty;
        }

        /// <summary>
        /// Determines whether the specified row has sub forums.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        /// The has sub forums.
        /// </returns>
        protected bool HasSubforums([NotNull] DataRow row)
        {
            return this.SubDataSource != null
                   && this.SubDataSource.Rows.Cast<DataRow>()
                          .Any(subrow => row["ForumID"].ToType<int>() == subrow["ParentID"].ToType<int>());
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AltLastPost = this.GetText("DEFAULT", "GO_LAST_POST");
        }

        /// <summary>
        /// Gets the Posts string
        /// </summary>
        /// <param name="_o">The _o.</param>
        /// <returns>Returns the Posts string</returns>
        protected string Posts([NotNull] object _o)
        {
            var row = (DataRow)_o;

            return row["RemoteURL"] == DBNull.Value ? "{0:N0}".FormatWith(row["Posts"]) : "-";
        }

        /// <summary>
        /// Gets the Topics string
        /// </summary>
        /// <param name="_o">The _o.</param>
        /// <returns>Returns the Topics string</returns>
        protected string Topics([NotNull] object _o)
        {
            var row = (DataRow)_o;
            return row["RemoteURL"] == DBNull.Value ? "{0:N0}".FormatWith(row["Topics"]) : "-";
        }

        #endregion
    }
}